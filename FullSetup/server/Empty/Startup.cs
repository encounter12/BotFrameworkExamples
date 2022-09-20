using System;
using System.IO;
using ConfigurationPrinter.models;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Runtime.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Bot.Builder;
using ConfigurationPrinter.services;

[assembly: FunctionsStartup(typeof(Empty.Startup))]

namespace Empty
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddBotRuntime(builder.GetContext().Configuration);

            _ = builder.Services.AddOptions<JustSettingsOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration
                        .GetSection("JustSettings")
                        .Bind(settings);
                });

            _ = builder.Services.AddSingleton<IConfigurationService, ConfigurationService>();

            _ = builder.Services.AddSingleton<IMiddleware, RegisterClassMiddleware<IConfigurationService>>(
                sp => new RegisterClassMiddleware<IConfigurationService>(sp.GetRequiredService<IConfigurationService>()));
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder configurationBuilder)
        {
            FunctionsHostBuilderContext context = configurationBuilder.GetContext();

            string applicationRoot = context.ApplicationRootPath;
            string environmentName = context.EnvironmentName;
            string settingsDirectory = "settings";

            configurationBuilder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, $"{settingsDirectory}/mysettings.json"), optional: true, reloadOnChange: true)
                //.AddJsonFile(Path.Combine(context.ApplicationRootPath, $"{settingsDirectory}/mysettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            configurationBuilder.ConfigurationBuilder.AddBotRuntimeConfiguration(
                applicationRoot,
                settingsDirectory,
                environmentName);
        }
    }
}