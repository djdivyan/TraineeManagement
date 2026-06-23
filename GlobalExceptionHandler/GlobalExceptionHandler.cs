using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TraineeManagementApi.Exceptions;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        (int statusCode, string title) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        ProblemDetails problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Type = GetProblemType(statusCode),
            Instance = httpContext.Request.Path,
            Detail = GetSafeErrorMessage(exception, httpContext)
        };
        
        Console.Write("HEllo");
        Console.WriteLine(problemDetails);
        Console.WriteLine(httpContext.Request.Path);
        Console.WriteLine(GetSafeErrorMessage(exception, httpContext));


        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        bool isHandled = await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });

        
        if (!isHandled)
        {
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        AppException appEx => ((int)appEx.StatusCode, appEx.Message),
        ArgumentNullException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
        ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument provided"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };

    private static string GetProblemType(int statusCode) => statusCode switch
    {
        400 => "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        401 => "https://tools.ietf.org/html/rfc9110#section-15.5.2",
        403 => "https://tools.ietf.org/html/rfc9110#section-15.5.4",
        404 => "https://tools.ietf.org/html/rfc9110#section-15.5.5",
        409 => "https://tools.ietf.org/html/rfc9110#section-15.5.10",
        _ => "https://tools.ietf.org/html/rfc9110#section-15.6.1"
    };

    private static string? GetSafeErrorMessage(Exception exception, HttpContext context)
    {
        IHostEnvironment env = context.RequestServices.GetRequiredService<IHostEnvironment>();
        if (env.IsDevelopment())
        {
            return exception.Message;
        }

        return exception is AppException ? exception.Message : null;
    }
}
