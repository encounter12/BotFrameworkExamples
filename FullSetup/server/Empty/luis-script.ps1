# Given directory with lu files, crawls the bot's sourceDirectory to find the recognizers for them 
# and returns a new list of luModels that match the given recognizer kind.
function Get-LUModels
{
    param 
    (
        [string] $recognizerType,
        [string] $crossTrainedLUDirectory,
        [string] $sourceDirectory
    )

    Write-Host "crossTrainedLUDirectory : $crossTrainedLUDirectory"
    Write-Host "sourceDirectory : $sourceDirectory"

    # Get a list of the cross trained lu models to process
    $crossTrainedLUModels = Get-ChildItem -Path $crossTrainedLUDirectory -Filter "*.lu" -file -name

    # Get a list of all the dialog recognizers (exclude bin and obj just in case)
    $luRecognizerDialogs = Get-ChildItem -Path $sourceDirectory -Filter "*??-??.lu.dialog" -file -name -Recurse | Where-Object { $_ -notmatch '^bin*|^obj*' }

    Write-Host "crossTrainedLUModels : $crossTrainedLUModels"
    Write-Host "luRecognizerDialogs : $luRecognizerDialogs"
        
    # Create a list of the models that match the given recognizer
    $luModels = @()
    foreach ($luModel in $crossTrainedLUModels) {
        # Load the dialog JSON and find the recognizer kind
        $luDialog = $luRecognizerDialogs | Where-Object { $_ -match "/$luModel.dialog" }

        Write-Host "Source directory: $sourceDirectory"
        Write-Host "Lu model: $luModel"
        Write-Host "Lu dialog: $luDialog"
        Write-Host "Lu dialog relative path: $sourceDirectory/$luDialog"

        if ([string]::IsNullOrEmpty($luDialog))
        {
            continue
        }

        $dialog = Get-Content -Path "$sourceDirectory/$luDialog" | ConvertFrom-Json
        $recognizerKind = ($dialog | Select-Object -ExpandProperty "`$kind")

        # Add it to the list if it is the expected type
        if ( $recognizerKind -eq $recognizerType)
        {
            $luModels += "$luModel"
        }
    }

    # return the models found
    return $luModels
}

# Creates luConfigFile for a list of lu models
function New-LuConfigFile
{
    param
    (
        [string] $luConfig,
        [string[]] $luModels,
        [string[]] $path
    )

    $luConfigLuis = "{
        models:[]
    }" | ConvertFrom-Json
    
    foreach($model in $models)
    {
        $luConfigLuis.models += "$path/$model"
    }
    
    $luConfigLuis | ConvertTo-Json | Out-File -FilePath $luConfigFile
}

$authoringKey = "14f7a9a669ce46169fc44d34a93872ab"

$authoringRegion = "westeurope"

$botName = "BotDevIvo"

$sourceDirectory = "./"

$generatedDirectory = $sourceDirectory + "generated"

Write-Host $generatedDirectory

# Prepare working folders
# Clean and recreate the generated directory
if (Test-Path $generatedDirectory)
{
    Remove-Item -Path $generatedDirectory -Force -Recurse
}

$outputDirectory = "$generatedDirectory/interruption"
New-Item -ItemType "directory" -Path $outputDirectory

# Cross train LU models

$crossTrainConfigPath = $sourceDirectory + "settings/cross-train.config.json"

# Cross train models
bf luis:cross-train --in $sourceDirectory --out $outputDirectory --config $crossTrainConfigPath  --force

# List generated files
# Set-Location $outputDirectory
ls $outputDirectory -R

Get-ChildItem -Path $outputDirectory -Recurse -File -Name

# Set-Location $sourceDirectory

# Find the lu models for the dialogs configured to use a LUIS recognizer
$models = Get-LUModels -recognizerType "Microsoft.LuisRecognizer" -crossTrainedLUDirectory $outputDirectory -sourceDirectory $sourceDirectory
if ($models.Count -eq 0)
{
    Write-Host "No LUIS models found."
    exit 0
}

# Create luConfig file with a list of the LUIS models
$luConfigFile = "$outputDirectory/luConfigLuis.json"
Write-Host "Creating $luConfigFile..."
New-LuConfigFile -luConfig $luConfigFile -luModels $models -path "."

# Output the generated settings
Get-Content $luConfigFile

# Publish and train LUIS models
bf luis:build --out $outputDirectory --authoringKey $authoringKey --region $authoringRegion --botName $botName --suffix composer --force --log --luConfig $luConfigFile
