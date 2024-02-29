using MongoDB.Driver;
using URLess.Domain;

namespace URLess.DAL.Repository;

public class UrlEntityRepository : IUrlEntityRepository
{
    private readonly IMongoCollection<UrlEntity> _collection;

    public UrlEntityRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<UrlEntity>(collectionName);
    }

    public async Task<string> GetByInitialUrlAsync(string initialUrl)
    {
        var filter = Builders<UrlEntity>.Filter.Eq(x => x.InitialUrl, initialUrl);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();

        return entity.HashedPath;
    }

    public async Task<string> GetByHashedPathAsync(string hashedPath)
    {
        var filter = Builders<UrlEntity>.Filter.Eq(x => x.HashedPath, hashedPath);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();

        return entity.InitialUrl;
    }

    public async Task UpsertByInitialUrlAsync(UrlEntity entity)
    {
        var filter = Builders<UrlEntity>.Filter.Eq(u => u.InitialUrl, entity.InitialUrl);
        var options = new ReplaceOptions { IsUpsert = true };
        await _collection.ReplaceOneAsync(filter, entity, options);
    }
}
