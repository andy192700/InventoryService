using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Infrastructure;

/// <summary>
/// Problem details middleware, to capture all none handler,
/// infrastructure derived error responses and create problem details to
/// write to the response.
/// </summary>
public class ProblemDetailsMiddleware
{
    private const string DefaultTitle = "An error occurred";
    private const string DefaultType = "about:blank";

    private readonly RequestDelegate _next;

    public ProblemDetailsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <inheritdoc/>
    public async Task InvokeAsync(
        HttpContext context,
        IProblemDetailsService problemDetailsService)
    {
        await _next(context);

        if (context.Response.HasStarted)
        {
            return;
        }

        if (context.Response.StatusCode < 400)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(context.Response.ContentType) || context.Response.ContentLength > 0)
        {
            return;
        }

        var headlines = GetHeadlines(context.Response.StatusCode);

        await problemDetailsService.WriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = headlines.Title,
                    Type = headlines.Type
                }
            });
    }

    private (string Title, string Type) GetHeadlines(int statusCode)
    {
        return statusCode switch
        {
            StatusCodes.Status400BadRequest => ("Bad Request", "https://tools.ietf.org/html/rfc9110#section-15.5.1"),
            StatusCodes.Status401Unauthorized => ("Unauthorized", "https://tools.ietf.org/html/rfc9110#section-15.5.2"),
            StatusCodes.Status403Forbidden => ("Forbidden", "https://tools.ietf.org/html/rfc9110#section-15.5.4"),
            StatusCodes.Status404NotFound => ("Not Found", "https://tools.ietf.org/html/rfc9110#section-15.5.5"),
            StatusCodes.Status500InternalServerError => ("Internal Server Error", "https://tools.ietf.org/html/rfc9110#section-15.6.1"),
            _ => (DefaultTitle, DefaultType)
        };
    }
}
