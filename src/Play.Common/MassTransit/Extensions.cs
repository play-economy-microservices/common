using System;
using System.Reflection;
using GreenPipes;
using GreenPipes.Configurators;
using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit;

/// <summary>
/// This class maintains concrete functionality for the IServiceCollection initialization to keep
/// everything abstracted and clean within the Program.cs file and initialize any general MassTransmit instances
/// of services.
/// </summary>
public static class Extensions
{
    private const string RabbitMq = "RABBITMQ";
    private const string ServiceBus = "SERVICEBUS";

    /// <summary>
    /// This is a Helper method to configure message broker.
    /// </summary>
    public static IServiceCollection AddMassTransitWithMessageBroker(this IServiceCollection services, IConfiguration config, Action<IRetryConfigurator> configureRetries = null)
    {
        var serviceSettings = config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

        switch (serviceSettings.MessageBroker?.ToUpper())
        {
            case ServiceBus:
                services.AddMassTransitWithServiceBus(configureRetries);
                break;
            case RabbitMq:
            default:
                services.AddMassTransitWithRabbitMq(configureRetries);
                break;
        }

        return services;
    }

    /// <summary>
    /// This will configure and register MassTransmit with RabbitMQ service.
    /// </summary>
    public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services, Action<IRetryConfigurator> configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            // Any consumer classes that are in the assembly will be the a consumer
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.UsingPlayEconomyRabbitMq(configureRetries);
        });

        services.AddMassTransitHostedService();

        return services;
    }

    /// <summary>
    /// This will configure and register MassTransmit with Service Bus service.
    /// </summary>
    public static IServiceCollection AddMassTransitWithServiceBus(this IServiceCollection services, Action<IRetryConfigurator> configureRetries = null)
    {
        services.AddMassTransit(configure =>
        {
            // Any consumer classes that are in the assembly will be the a consumer
            configure.AddConsumers(Assembly.GetEntryAssembly());
            configure.UsingPlayEconomyAzureServiceBus(configureRetries);
        });

        services.AddMassTransitHostedService();

        return services;
    }

    /// <summary>
    /// This is a general helper method to configure and register MassTransit Services
    /// </summary>
    public static void UsingPlayEconomyMessageBroker(this IServiceCollectionBusConfigurator configure, IConfiguration config, Action<IRetryConfigurator> configureRetries = null)
    {
        var serviceSettings = config.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

        switch (serviceSettings.MessageBroker?.ToUpper())
        {
            case ServiceBus:
                configure.UsingPlayEconomyAzureServiceBus(configureRetries);
                break;
            case RabbitMq:
            default:
                configure.UsingPlayEconomyRabbitMq(configureRetries);
                break;
        }
    }

    /// <summary>
    /// This is used to register the RabbitMq configurations such as apply a Host and Endpoints, along with an optional retry handler.
    /// </summary>
    public static void UsingPlayEconomyRabbitMq(this IServiceCollectionBusConfigurator configure, Action<IRetryConfigurator> configureRetries = null)
    {
        configure.UsingRabbitMq((context, configurator) =>
        {
            // Get an instance of configuration using context
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            // Register an instance of the RabbitMQ Settings and apply necessary configurations
            var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
            configurator.Host(rabbitMQSettings.Host);
            configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

            // Retry 3 times and wait 5 seconds within those retries.
            if (configureRetries is null)
            {
                configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
            }

            configurator.UseMessageRetry(configureRetries);
        });
    }

    /// <summary>
    /// This is used to register Azure Service Bus along with optional retries.
    /// </summary>
    public static void UsingPlayEconomyAzureServiceBus(this IServiceCollectionBusConfigurator configure, Action<IRetryConfigurator> configureRetries = null)
    {
        configure.UsingAzureServiceBus((context, configurator) =>
        {
            // Get an instance of configuration using context
            var configuration = context.GetService<IConfiguration>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            // Register an instance of the RabbitMQ Settings and apply necessary configurations
            var serviceBusSettings = configuration.GetSection(nameof(ServiceBusSettings)).Get<ServiceBusSettings>();
            configurator.Host(serviceBusSettings.ConnectionString);
            configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));

            // Retry 3 times and wait 5 seconds within those retries.
            if (configureRetries is null)
            {
                configureRetries = (retryConfigurator) => retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
            }

            configurator.UseMessageRetry(configureRetries);
        });
    }
}
