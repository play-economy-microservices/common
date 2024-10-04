using System;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Play.Common.Settings;

namespace Play.Common.Configuration;

public static class Extensions
{
    /// <summary>
    /// This method configures Azure Key Vault integration to store and retrieve secrets during runtime.
    /// It applies only in the production environment. In this case, it uses the default Azure credentials
    /// available for the environment to authenticate to the Key Vault.
    /// </summary>
    public static IHostBuilder ConfigureAzureKeyVault(this IHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            // Only use Key vault in production and use default
            // Azure credentials available for the environment to authenticate
            if (context.HostingEnvironment.IsProduction())
            {
                // Get the keyvault name (this is general for all services)
                var configuration = configurationBuilder.Build();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                
                configurationBuilder.AddAzureKeyVault(
                    new Uri($"https://{serviceSettings.KeyVaultName}.vault.azure.net/"),
                    new DefaultAzureCredential());
            }
        });
    }
}