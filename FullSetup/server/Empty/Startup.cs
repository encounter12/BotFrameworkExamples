using System;
using System.IO;
using ConfigurationPrinter.models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Runtime.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Empty.Startup))]

namespace Empty
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _ = builder.Services.AddOptions<JustSettingsOptions>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration
                    .GetSection("JustSettings")
                    .Bind(settings);
            });

            builder.Services.AddBotRuntime(builder.GetContext().Configuration);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder configurationBuilder)
        {
            FunctionsHostBuilderContext context = configurationBuilder.GetContext();

            string applicationRoot = context.ApplicationRootPath;
            string environmentName = context.EnvironmentName;
            string settingsDirectory = "settings";

            configurationBuilder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "mysettings.json"), optional: true, reloadOnChange: false)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"mysettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
                .AddEnvironmentVariables();

            configurationBuilder.ConfigurationBuilder.AddBotRuntimeConfiguration(
                applicationRoot,
                settingsDirectory,
                environmentName);
        }
    }
}