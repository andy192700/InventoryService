using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InventoryService.Infrastructure.Health;

public static class HealthCheckResponseWriter
{
    /// <summary>
    /// Writes a <see cref="HealthReport"/> instance to the current <see cref="HttpContext"/>
    /// </summary>
    /// <returns>A delegate encapsulating the writing asynchronous task.</returns>
    public static Func<HttpContext, HealthReport, Task> WriteHealthResponse()
    {
        return async (context, report) =>
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            
            context.Response.ContentType = "application/problem+json";
            
            var response = MapHealthResponse(report);

            await context.Response.WriteAsJsonAsync(response);
        };
    }

    private static HealthCheckResponse MapHealthResponse(HealthReport report)
    {
        var checks = report.Entries.Select(e => new HealthCheckResponseCheck
        {
            Key = e.Key,
            Description = e.Value.Description,
            Value = e.Value.Status.ToString()
        }).ToList();

        return new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Checks = checks
        };
    }
}
