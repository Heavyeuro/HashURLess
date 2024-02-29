using System;
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
        string shortenedPath = "";
        bool KeydoesNotExist = true;

        while (KeydoesNotExist)
        {
            (string hashedValue, bool? wasRotated) = await _urlEntityRepository.CheckHashExistanceAsync(fullUrl);
            bool recalculateCache = !string.IsNullOrWhiteSpace(hashedValue) || (wasRotated ?? false);
            shortenedPath = CalculateSHA1Base64(fullUrl, recalculateCache);
            KeydoesNotExist = !string.IsNullOrEmpty(await _urlEntityRepository.GetByHashedPathAsync(shortenedPath));
        }

        var urlEntity = new Domain.UrlEntity { InitialUrl = fullUrl, HashedPath = shortenedPath };
        await _urlEntityRepository.UpsertByInitialUrlAsync(urlEntity);

        return shortenedPath;
    }

    private static string CalculateSHA1Base64(string input, bool isCollision = false)
    {
        Random random = new Random();
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            if (isCollision)
            {
                int indexToAlter = random.Next(0, inputBytes.Length);
                byte randomByte = (byte)random.Next(0, 256);
                inputBytes[indexToAlter] = randomByte;
            }

            byte[] hashBytes = sha1.ComputeHash(inputBytes);
            string base64String = Convert.ToBase64String(hashBytes);
            return base64String.Substring(0, Math.Min(base64String.Length, 6));
        }
    }
}
