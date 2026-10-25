namespace InventoryService.Infrastructure;

/// <summary>
/// Middleware which will intercept all incoming requests and log accordingly.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The <see cref="HttpContext"/> for the current request.</param>
    /// <returns>A task encapsulating the middleware's invocation.</returns>
    public async Task Invoke(HttpContext context)
    {
        _logger.LogInformation(
            "Started request {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        var started = DateTime.UtcNow;

        try
        {
            await _next(context);

            _logger.LogInformation(
                "Finished request {Method} {Path} StatusCode={StatusCode} DurationMs={DurationMs}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                (DateTime.UtcNow - started).TotalMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed request {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            throw;
        }
    }
}