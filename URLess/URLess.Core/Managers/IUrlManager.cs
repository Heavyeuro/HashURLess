namespace URLess.Core.Managers;

public interface IUrlManager
{
    Task<string> GetInitialUrl(string shortenedUrl);
    Task<string> GetShortenedPath(string fullUrl);
}
