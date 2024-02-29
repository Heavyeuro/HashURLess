using URLess.Domain;

namespace URLess.DAL.Repository;

public interface IUrlEntityRepository
{
    Task<string> GetByInitialUrlAsync(string initialUrl);
    Task<string> GetByHashedPathAsync(string hashedPath);
    Task UpsertByInitialUrlAsync(UrlEntity entity);
}