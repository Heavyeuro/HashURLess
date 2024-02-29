using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using URLess.Config;
using URLess.Core.Managers;
using URLess.DAL.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IUrlManager, UrlManager>();
builder.Services.AddTransient<IUrlEntityRepository, UrlEntityRepository>();

MongoDbSettings mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
builder.Services.AddMongoDb(mongoDbSettings);

//ToDiscuss: Id add listener which is responsible for db writing
// And redis added cache to service for get operations

var app = builder.Build();

app.MapGet("/{url}", async (HttpContext context,
    [FromServices] IUrlManager urlManager, [FromRoute] string url) =>
{
    var initialUrl = await urlManager.GetInitialUrl(url);

    if (!string.IsNullOrEmpty(initialUrl))
    {
        context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
        context.Response.Headers.Add("Location", initialUrl);

        return Task.CompletedTask;
    }

    context.Response.StatusCode = StatusCodes.Status404NotFound;
    return Task.CompletedTask;
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