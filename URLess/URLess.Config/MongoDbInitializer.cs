using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using URLess.Domain;

namespace URLess.Config;

public static class MongoDbServiceCollectionExtensions
{
    public static void AddMongoDb(this IServiceCollection services, MongoDbSettings mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
        var database = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

        var collection = database.GetCollection<UrlEntity>(mongoDbSettings.CollectionName);

        // Create a unique index on the InitialUrl field
        var indexKeysDefinition = Builders<UrlEntity>.IndexKeys.Ascending(u => u.InitialUrl);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<UrlEntity>(indexKeysDefinition, indexOptions);
        collection.Indexes.CreateOne(indexModel);

        services.AddSingleton(collection);
    }
}
