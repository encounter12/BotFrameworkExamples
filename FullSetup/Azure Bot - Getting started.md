# MS Bot Framework Composer - Bot Full Setup  

## Provision Azure Resources  

1. Sign up in Azure Portal

2. Create subscription
(You can create a Free trial having $200 for free or if you don't apply for the free trial you can create Pay-As-You-Go subscription)

3. Create a resource group

Setup region / location for the resource group, make sure when you create all the resources inside the group to use the group's region/location. Otherwise, some resources wouldn't be able to find the data from other resources.

The best option is to create Azure Resource Group for each deployment environment. For a single environment e.g. Development, create the following Azure Resources below: 

4. Create Azure Storage account
and copy the account Id (we will need it for the Azure Function App below)

5. Create Azure Function App
make sure you specify the already created Storage account

5. Create Microsoft Application

Azure Portal Home page - Azure Active Directory - (Manage) App Registrations - type Name, Supported account types: Accounts in any organizational directory (Any Azure AD Directory - Multitenant) , click on "Register" button.

Go back to "Overview" - (Essentials) Application (client) ID - Copy the Microsoft App Id (we will need it for step 6. Create Azure Bot and also when creating Publish Profile in Bot Framework Composer)

6. Create Azure Bot

6.1. Link it to the Microsoft Application Id (Azure Active Directory - App Registrations)

6.2. Create Microsoft App Password (so called: Client Secret) and COPY IT

Azure Bot resource - (Settings tab) - Configuration - Microsoft App ID - Click on Manage - Client secrets tab - New client secret

or:

Azure Portal - Azure Active Directory - App registrations - Owned applications tab - Click on the Chatbot display name - (Manage) Certificates and Secrets - Client secrets tab - New client secret

IMPORTANT: Copy the client secret right after it is created before leaving the screen. If you leave the screen and come back you won't be able to get it. If this case happens, you can delete the current client secret and create a new one from the '+ New client secret' button.

We will need the Client Secret value for step 8.2. Setup Microsoft Application credentials (in Bot Framework Composer - Configure - Development resources)

6.3. Setup the messages endpoint in the Azure Bot

Copy the main url from the Azure Function App - Overview page - Endpoint and add to it /api/messages, e.g.
https://botdevivo.azurewebsites.net/api/messages

7. Create Azure Language Understanding (LUIS) authoring and prediction resources
Create the resources in the same resource group for the particular deployment environment and make sure the region/location is the same as the resource group's region/location.
(You can choose Free tier option for both resource, have in mind that Free tier is currently unavailable for the Prediction resource for region: West US (westus))

## Create Bot in Composer

8. Create a bot (using Bot Framework Composer)

Home - + Create New - Empty bot - Next - Setup name and choose a location - Create

9. Configure the bot

Configure (wrench icon) - Development resources tab:

9.1. Setup language understanding (LUIS)

Language Understanding authoring key : Copy the value from Azure portal - Resource group - open Language understanding Authoring resource - Keys & Secrets - Copy the first key

Language Understanding region : Copy the value from Azure portal - Resource group - open Language understanding Authoring resource - location

You can fill in the above values if you click on "Set up Language Understanding" button in the Azure Language Understanding section - Use existing resources - login to Azure - Select the particular subscription and resource group - then select the Language Understanding Authoring resource.

9.2. Setup Microsoft Application credentials

In the section Microsoft App ID:

- Microsoft App Id : 

Option A. Copy it from - Azure Bot resource - (Settings tab) Configuration - Microsoft App ID
or:
Option B. Copy it from - Azure Portal Home - Azure Active Directory icon - App registrations - Owned applications tab - click on the Application Display name - Overview section - Application (client) ID

- Microsoft App Password: Paste it where you stored it on step: 6.2. Create Microsoft App Password (as part of 6. Create Azure Bot)

10. Add a trigger to the bot

10.1. Add new trigger (On intent recognized) to the bot: SearchForJobs with single trigger phrase (make sure you copy the dash too):
- find job 

10.2. Add 'Send a response' action to the SearchForJobs trigger with the informative text:
"SearchForJobs trigger invoked"

10.3. Start the bot and test it

Click on "Start bot" at the top right - Open Web Chat. 

When the bot is starting it is cross-training the language understanding (LUIS) model with the new trigger phrase: "find jobs". 

You will see a message: "Welcome to your bot." (or something similar). Type: find job

You should see: "SearchForJobs trigger invoked"

Voila!

## Create Publish Profile

11. Create Publish Profile (in Bot Framework Composer)

Bot Framework Composer - (sidebar) Publish - Publishing profile tab - Add new - Type name, from Publishing target dropdown select: Publish bot to Azure, click on Next. On "Create a publishing profile" step - select radio button "Import existing resources" - click on "Next". Fill in the values below

In our case we haven't created applicationInsights, cosmosDb, blobStorage, qna - so we can remove them from the JSON (or we can leave them like that for future additions)

```json
{
  "name": "<azure-bot-resourece-name>",
  "environment": "dev",
  "tenantId": "<tenant id of your microsoft app, go to: Azure Portal - Azure Active Directory - App registrations - Microsoft App - Directory (tenant) ID>",
  "hostname": "<Azure web app / function app name>",
  "runtimeIdentifier": "win-x64",
  "resourceGroup": "<name of your resource group>",
  "botName": "<your Azure Bot resource name>",
  "subscriptionId": "<id of your Azure subscription>",
  "region": "<region of your resource group>",
  "appServiceOperatingSystem": "windows",
  "scmHostDomain": "scm.azurewebsites.net",
  "luisResource": "<name of your Azure Language Understanding (LUIS) prediction resource>",
  "settings": {
    "applicationInsights": {
      "InstrumentationKey": "<Instrumentation Key>",
      "connectionString": "<connection string>"
    },
    "cosmosDb": {
      "cosmosDBEndpoint": "<endpoint url>",
      "authKey": "<auth key>",
      "databaseId": "botstate-db",
      "containerId": "botstate-container"
    },
    "blobStorage": {
      "connectionString": "<connection string>",
      "container": "<container>"
    },
    "luis": {
      "authoringKey": "<your LUIS authoring resource key>",
      "authoringEndpoint": "<your LUIS authoring resource endpoint>",
      "endpointKey": "<your LUIS prediction resource first key>",
      "endpoint": "<your LUIS prediction resource endpoint>",
      "region": "<your LUIS authoring and prediction resources region - they should match, make sure you get the region code, Overview - top right: JSON View - "location">"
    },
    "qna": {
      "subscriptionKey": "<subscription key>",
      "endpoint": "<endpoint>"
    },
    "MicrosoftAppId": "<App Id - from Azure Bot resource - (Settings tab) - Configuration - Microsoft App ID>",
    "MicrosoftAppPassword": "<app password copied on Microsoft App registration, remember: Azure Portal Home - Azure Active Directory - (Manage) App registrations - +New registration>"
  }
}
```

Paste the JSON and click on Create

12. Publish the bot

(sidebar) Publish - Publish tab - mark the bot - from Publish target dropdown select the Publish profile e.g. "Development" - click on "Publish selected bots" - type comment and click on Publish.

Finally the Status column value should be: Success

13. Test the bot in Azure portal:

Azure Portal Home page - Azure Bot Resource - (Settings) Test in Web Chat. You should see a message similar to: "Welcome to your bot." Type: "find job" phrase we have added on step 10.1

You should see a response: "SearchForJobs trigger invoked"

Voila!

14. Create a Web HTML client

14.1. Get Secret Key for Web Chat

Azure Bot - (Settings) Channels - Web Chat - Default Site - Secret keys - 1st key - Show - Copy the key

12. Create a Bot Web cLient

Paste the copied Web Chat Secret Key as createDirectLie() parameter (see placeholder below)

```html
<!DOCTYPE html>
<html>
  <head>
    <script
      crossorigin="anonymous"
      src="https://cdn.botframework.com/botframework-webchat/latest/webchat.js"
    ></script>
    <style>
      html,
      body {
         height: 100%;
      }

      body {
        margin: 0;
      }

      #webchat {
        height: 100%;
        width: 100%;
      }
    </style>
  </head>
  <body>
    <div id="webchat" role="main"></div>
    <script>
      window.WebChat.renderWebChat(
        {
          directLine: window.WebChat.createDirectLine({
            secret: '<Azure Portal Home - Resource group - Azure Bot resource - (Settings) Channels - Web Chat - Default Site - Secret keys - 1st key>'
          }),
          userID: 'YOUR_USER_ID',
          username: 'Web Chat User',
          locale: 'en-US'
        },
        document.getElementById('webchat')
      );
    </script>
  </body>
</html>
```

NB: passing the Web Chat Secret Key is just for demo purposes. In more production-ready setups it should be replaced with JWT token.

14.2. Open the webpage in Browser

Open the html page in browser. You should see a initial Greeting e.g. "Welcome to your bot."  
Type: "find job" and press Enter. You should see a message: "SearchForJobs trigger invoked".  