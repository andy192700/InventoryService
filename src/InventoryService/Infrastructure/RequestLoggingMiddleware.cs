namespace InventoryService.Infrastructure;

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