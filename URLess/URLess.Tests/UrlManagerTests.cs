using NUnit.Framework;
using Moq;
using URLess.Core.Managers;
using URLess.DAL.Repository;

namespace URLess.Tests;

[TestFixture]
public class UrlManagerTests
{
    [Test]
    public async Task GetInitialUrl_Returns_InitialUrl_For_ShortenedUrl()
    {
        // Arrange
        var urlEntityRepositoryMock = new Mock<IUrlEntityRepository>();
        string expectedInitialUrl = "https://example.com";
        urlEntityRepositoryMock.Setup(repo => repo.GetByHashedPathAsync("shortenedUrl"))
            .ReturnsAsync(expectedInitialUrl);
        var urlManager = new UrlManager(urlEntityRepositoryMock.Object);

        // Act
        string actualInitialUrl = await urlManager.GetInitialUrl("shortenedUrl");

        // Assert
        Assert.AreEqual(expectedInitialUrl, actualInitialUrl);
    }

    [Test]
    public async Task GetShortenedPath_Calculates_ShortenedPath_And_Upserts_UrlEntity()
    {
        // Arrange
        var urlEntityRepositoryMock = new Mock<IUrlEntityRepository>();
        string fullUrl = "https://example.com";
        string expectedShortenedPath = "calculatedShortenedPath";
        urlEntityRepositoryMock.Setup(repo => repo.UpsertByInitialUrlAsync(It.IsAny<Domain.UrlEntity>()))
            .Returns(Task.CompletedTask);
        var urlManager = new UrlManager(urlEntityRepositoryMock.Object);

        // Act
        string actualShortenedPath = await urlManager.GetShortenedPath(fullUrl);

        // Assert
        Assert.AreEqual(expectedShortenedPath, actualShortenedPath);
        urlEntityRepositoryMock.Verify(repo => repo.UpsertByInitialUrlAsync(
            It.Is<Domain.UrlEntity>(entity => entity.InitialUrl == fullUrl && entity.HashedPath == expectedShortenedPath)),
            Times.Once);
    }
}
