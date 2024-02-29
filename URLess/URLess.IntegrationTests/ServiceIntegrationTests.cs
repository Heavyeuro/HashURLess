using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace URLess.IntegrationTests
{
    [TestFixture]
    public class IntegrationTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new TestApplicationFactory();
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task Test_Get_UrlExists_Returns_Redirect()
        {
            // Arrange
            var url = "https://example.com";

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.AreEqual(HttpStatusCode.MovedPermanently, response.StatusCode);
            Assert.AreEqual("https://example.com/new-url", response.Headers.Location?.ToString());
        }

        [Test]
        public async Task Test_Post_ValidRequest_Returns_Created()
        {
            // Arrange
            var requestBody = JsonSerializer.Serialize(new { url = "http://example.com" });
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<dynamic>(jsonResponse);
            Assert.IsNotNull(responseObject.url);
            Assert.IsNotNull(responseObject.originalURL);
        }

        [Test]
        public async Task Test_Post_InvalidRequest_Returns_BadRequest()
        {
            // Arrange
            var requestBody = "{}"; // Invalid request without 'url' field
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
