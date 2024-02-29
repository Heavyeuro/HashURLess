using System.Security.Cryptography;
using System.Text;

namespace URLess.Core.Managers;

public class UrlManager : IUrlManager
{

    public Task<string> GetInitialUrl(string shortenedUrl)
    {
        return Task.FromResult(shortenedUrl);
    }

    public Task<string> GetShortenedPath(string fullUrl)
    {
        return Task.FromResult(CalculateSHA1Base64(fullUrl));
    }

    private static string CalculateSHA1Base64(string input)
    {
        using var sha1 = SHA1.Create();

        byte[] hashBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
        string base64String = Convert.ToBase64String(hashBytes);
        return base64String.Substring(0, Math.Min(base64String.Length, 6));
    }
}
