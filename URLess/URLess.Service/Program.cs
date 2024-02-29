using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using URLess.Core.Managers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IUrlManager, UrlManager>();

var app = builder.Build();

app.MapGet("/{url}", async (HttpContext context,
    [FromServices] IUrlManager urlManager, [FromRoute] string url) =>
{
    var initialUrl = await urlManager.GetInitialUrl(url);

    if (string.IsNullOrEmpty(initialUrl))
    {
        context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
        context.Response.Headers.Add("Location", "https://example.com/new-url");
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
});

app.MapPost("/", async (HttpContext context, [FromServices] IUrlManager urlManager) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var requestBody = await reader.ReadToEndAsync();
    var request = context.Request;

    var jsonDocument = JsonDocument.Parse(requestBody);
    var root = jsonDocument.RootElement;

    if (root.TryGetProperty("url", out var urlProperty))
    {
        var originalUrl = urlProperty.GetString();

        var shortenedPath = await urlManager.GetShortenedPath(originalUrl);

        var responseJson = new
        {
            url = $"{request.Scheme}://{request.Host}/{shortenedPath}",
            originalURL = originalUrl
        };

        var jsonResponse = JsonSerializer.Serialize(responseJson);

        context.Response.StatusCode = StatusCodes.Status201Created;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(jsonResponse);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsync("Bad Request: 'url' field is missing in the request body.");
    }
});

app.Run();