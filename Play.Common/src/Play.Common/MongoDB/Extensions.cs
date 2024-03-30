using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Common.MongoDB;

/// <summary>
/// This class maintains concrete functionality for the IServiceCollection initialization to keep
/// everything abstracted and clean within the Program.cs file and initialize any general Mongo instances
/// of services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// This will create a IMongoDatabase that will be used to initiate a generic
    /// MongoRepository for any Entity.
    /// </summary>
	public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        // Keep original types when inserting MongoDB Docouments
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        // Construct the MongoDB Client
        services.AddSingleton(serviceProvider =>
        {
            // Get configuration service from infrastructure
            var configuration = serviceProvider.GetService<IConfiguration>();

            // Bindings
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            // Init MongoClient
            var mongoClient = new MongoClient(mongoDbSettings?.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings?.ServiceName);
        });

        return services;
    }

    /// <summary>
    /// Generic MongoRepository for Entities who implement IEntity 
    /// </summary>
    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            // This call will work because we've registered the IMongoDatabase beforehand
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database, collectionName);
        });

        return services;
    }
}
