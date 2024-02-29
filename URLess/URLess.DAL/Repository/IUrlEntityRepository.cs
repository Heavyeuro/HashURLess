using URLess.Domain;

namespace URLess.DAL.Repository;

public interface IUrlEntityRepository
{
    Task<string> GetByHashedPathAsync(string hashedPath);
    Task UpsertByInitialUrlAsync(UrlEntity entity);
    Task<(string, bool?)> CheckHashExistanceAsync(string fullUrl);
}