using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.HealthChecks;

public static class Extensions
{
    private const string MongoCheckName = "mongodb";
    private const string ReadyTagName = "ready";
    private const string LiveTagName = "live";
    private const string HealthEndpoint = "health";
    private const int DefaultSeconds = 3;
    
    /// <summary>
    /// This registers a Mongo DB Health Check
    /// </summary>
    public static IHealthChecksBuilder AddMongoDB(this IHealthChecksBuilder builder, TimeSpan? timeSpan = default)
    {
        return builder.Add(new HealthCheckRegistration(MongoCheckName, serviceProvider =>
            {
                var configuration = serviceProvider.GetService<IConfiguration>();
                var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return new MongoDbHealthCheck(mongoClient);
            },
            HealthStatus.Unhealthy, // unhealthy status
            new[] { ReadyTagName }, // tags
            TimeSpan.FromSeconds(DefaultSeconds) // timeout if it's not healthy
        ));
    }

    public static void MapPlayEconomyHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks($"/{HealthEndpoint}/{ReadyTagName}", new HealthCheckOptions()
        {
            Predicate = (check) => check.Tags.Contains(ReadyTagName)
        });
        endpoints.MapHealthChecks($"/{HealthEndpoint}/{LiveTagName}", new HealthCheckOptions()
        {
            Predicate = (check) => false // Interpret this as: "let me know if you're alive or not"
        });
    }
}