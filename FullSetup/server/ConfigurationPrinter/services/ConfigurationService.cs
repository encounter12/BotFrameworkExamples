using ConfigurationPrinter.models;
using Microsoft.Extensions.Options;

namespace ConfigurationPrinter.services;

public class ConfigurationService : IConfigurationService
{
    private readonly JustSettingsOptions settings;

    public ConfigurationService(IOptions<JustSettingsOptions> options)
    {
         this.settings = options.Value;
    }

    public string JustName => this.settings.JustName;
}