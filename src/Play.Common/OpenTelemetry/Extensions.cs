using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Play.Common.MassTransit;
using Play.Common.Settings;
namespace Play.Common.OpenTelemetry;

public static class Extensions
{
    /// <summary>
    /// This adds OpenTelemetry tracing and exports traces to Jaeger.
    /// </summary>
    public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration config)
    {
        services.AddOpenTelemetryTracing(builder =>
            {
                var serviceSettings = config.GetSection(nameof(ServiceSettings))
                    .Get<ServiceSettings>();

                builder.AddSource(serviceSettings.ServiceName)
                    .AddSource("MassTransit")
                    .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                            .AddService(serviceName: serviceSettings.ServiceName))
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddJaegerExporter(options =>
                    {
                        var jaegerSettings = config.GetSection(nameof(JaegerSettings))
                            .Get<JaegerSettings>();
                        options.AgentHost = jaegerSettings.Host;
                        options.AgentPort = jaegerSettings.Port;
                    });
            })
            .AddConsumeObserver<ConsumeObserver>(); // observe mass transit fault messages

        return services;
    }
}