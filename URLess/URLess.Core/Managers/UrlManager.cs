using System.Security.Cryptography;
using System.Text;
using URLess.DAL;
using URLess.DAL.Repository;

namespace URLess.Core.Managers;

public class UrlManager : IUrlManager
{
    private readonly IUrlEntityRepository _urlEntityRepository;

    public UrlManager(IUrlEntityRepository urlEntityRepository)
    {
        _urlEntityRepository = urlEntityRepository;
    }

    public async Task<string> GetInitialUrl(string shortenedUrl)
    {
        return await _urlEntityRepository.GetByHashedPathAsync(shortenedUrl);
    }

    public async Task<string> GetShortenedPath(string fullUrl)
    {
        var shortenedPath = CalculateSHA1Base64(fullUrl);

        var urlEntity = new Domain.UrlEntity { InitialUrl = fullUrl, HashedPath = shortenedPath };
        await _urlEntityRepository.UpsertByInitialUrlAsync(urlEntity);

        return shortenedPath;
    }

    private static string CalculateSHA1Base64(string input)
    {
        using var sha1 = SHA1.Create();

        byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        string base64String = Convert.ToBase64String(hashBytes);
        return base64String.Substring(0, Math.Min(base64String.Length, 6));
    }
}
