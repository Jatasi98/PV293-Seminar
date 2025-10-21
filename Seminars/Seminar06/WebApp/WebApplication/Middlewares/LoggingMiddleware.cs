using System.Diagnostics;

namespace PV293WebApplication.Middlewares;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(httpContext);

        stopwatch.Stop();

        _logger.LogInformation("[{Method}]: {Path}{QueryString} responded {StatusCode} in {Elapsed} ms",
                httpContext.Request.Method,
                httpContext.Request.Path,
                httpContext.Request.QueryString,
                httpContext.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
    }
}

public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}
