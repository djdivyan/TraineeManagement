using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace TrainingDirectory.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyHeaderName = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (context.Request.Path.StartsWithSegments("/api/health"))
        {
            await _next(context);
            return;
        }

        //Extract the expected key from configurations
        var expectedApiKey = configuration["ServiceAuth:ApiKey"];
        if (string.IsNullOrEmpty(expectedApiKey))
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("API Key configuration is missing on the server.");
            return;
        }

        //Extract and validate the incoming request header
        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey) 
            || !string.Equals(expectedApiKey, extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized: Invalid or missing API key.");
            return;
        }

        //Token is valid; proceed to the controller
        await _next(context);
    }
}
