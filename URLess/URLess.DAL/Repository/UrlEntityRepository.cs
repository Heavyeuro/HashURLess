using MongoDB.Driver;
using SharpCompress.Common;
using URLess.DAL.Models;
using URLess.Domain;

namespace URLess.DAL.Repository;

public class UrlEntityRepository : IUrlEntityRepository
{
    private readonly IMongoCollection<UrlEntityDal> _collection;

    public UrlEntityRepository(IMongoCollection<UrlEntityDal> mongoCollection)
    {
        _collection = mongoCollection;
    }

    public async Task<string> GetByHashedPathAsync(string hashedPath)
    {
        var filter = Builders<UrlEntityDal>.Filter.Eq(x => x.HashedPath, hashedPath);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();

        return entity?.InitialUrl;
    }

    public async Task<(string, bool?)> CheckHashExistanceAsync(string fullUrl)
    {
        var filterSearch = Builders<UrlEntityDal>.Filter.Eq(x => x.InitialUrl, fullUrl);
        var entityFound = await _collection.Find(filterSearch).FirstOrDefaultAsync();

        return (entityFound?.InitialUrl, entityFound?.WasRotated);
    }

    public async Task UpsertByInitialUrlAsync(UrlEntity entity)
    {
        var filterSearch = Builders<UrlEntityDal>.Filter.Eq(x => x.InitialUrl, entity.InitialUrl);
        var entityFound = await _collection.Find(filterSearch).FirstOrDefaultAsync();

        if (entityFound != null)
        {
            var filter = Builders<UrlEntityDal>.Filter.Eq(u => u.Id, entityFound.Id);
            entityFound.HashedPath = entity.HashedPath;
            entityFound.WasRotated = true;
            var result = await _collection.ReplaceOneAsync(filter, entityFound);
        }
        else
        {
            await _collection.InsertOneAsync(new UrlEntityDal 
            { HashedPath = entity.HashedPath, InitialUrl = entity.InitialUrl});
        }
    }
}
