using System.Runtime.CompilerServices;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ConfigurationPrinter.models;

namespace ConfigurationPrinter;

public class ConfigurationPrinterDialog : Dialog
{
    [JsonConstructor]
    public ConfigurationPrinterDialog([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base()
    {
        // enable instances of this command as debug break point
        RegisterSourceLocation(sourceFilePath, sourceLineNumber);
    }

    /// <summary>
    /// Gets the unique name (class identifier) of this trigger.
    /// </summary>
    /// <remarks>
    /// There should be at least a .schema file of the same name.  There can optionally be a
    /// .uischema file of the same name that describes how Composer displays this trigger.
    /// </remarks>
    [JsonProperty("$kind")]
    public const string Kind = "ConfigurationPrinterDialog";

    /// <summary>
    /// Gets or sets caller's memory path to store the result of this step in (ex: conversation.area).
    /// </summary>
    /// <value>
    /// Caller's memory path to store the result of this step in (ex: conversation.area).
    /// </value>
    [JsonProperty("resultProperty")]
    public StringExpression ResultProperty { get; set; }

    public override Task<DialogTurnResult> BeginDialogAsync(
        DialogContext dc, object options,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        JustSettingsOptions justSettingsOptions = dc.Services.Get<IOptions<JustSettingsOptions>>().Value;

        if (this.ResultProperty != null)
        {
            dc.State.SetValue(this.ResultProperty.GetValue(dc.State), justSettingsOptions.JustName);
        }

        return dc.EndDialogAsync(result: justSettingsOptions.JustName, cancellationToken: cancellationToken);
    }
}
