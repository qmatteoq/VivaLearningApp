using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using VivaLearningFunction.Services;

[assembly: FunctionsStartup(typeof(VivaLearningFunction.Startup))]

namespace VivaLearningFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ICsvService, CsvService>();
            builder.Services.AddSingleton<IMicrosoftGraphService, MicrosoftGraphService>();
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder
                  .SetBasePath(Environment.CurrentDirectory)
                  .AddJsonFile("local.settings.json", true)
                  .AddUserSecrets(Assembly.GetExecutingAssembly(), true)
                  .AddEnvironmentVariables()
                  .Build();
        }
    }
}
