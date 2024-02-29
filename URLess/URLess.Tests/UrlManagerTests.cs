using NUnit.Framework;
using URLess.Core.Managers;

namespace URLess.Tests;

public class UrlManagerTests
{
    [Test]
    public async Task GetShortenedUrl_ValidInput_ReturnsShortenedUrl()
    {
        // Arrange
        var inputUrl = "https://example.com";
        var expectedShortenedUrl = "L7skyw";
        var expectedShortenedUrlLength = 6;
        // This is a mocked shortened URL based on SHA1 hash

        var urlManager = new UrlManager();

        // Act
        var result = await urlManager.GetShortenedPath(inputUrl);

        // Assert
        Assert.IsFalse(string.IsNullOrEmpty(result));
        Assert.AreEqual(expectedShortenedUrl, result);
        Assert.AreEqual(expectedShortenedUrlLength, result.Length);
    }
}
