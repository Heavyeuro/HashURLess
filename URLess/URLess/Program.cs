using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/{url}", async (HttpContext context, string url) =>
{
    bool urlExists = false; 

    if (urlExists)
    {
        context.Response.StatusCode = StatusCodes.Status301MovedPermanently;
        context.Response.Headers.Add("Location", "https://example.com/new-url");
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
    }
});

app.MapPost("/", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var requestBody = await reader.ReadToEndAsync();
        
    var jsonDocument = JsonDocument.Parse(requestBody);
    var root = jsonDocument.RootElement;
        
    if (root.TryGetProperty("url", out var urlProperty))
    {
        string originalUrl = urlProperty.GetString();

        string shortenedUrl = "handledValue";//ShortenUrl(originalUrl);

        var responseJson = new
        {
            url = shortenedUrl,
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