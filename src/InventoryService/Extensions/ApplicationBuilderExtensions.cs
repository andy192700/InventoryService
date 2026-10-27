using InventoryService.Features;
using InventoryService.Infrastructure;

namespace InventoryService.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Register the applications middleware.
    /// </summary>
    /// <param name="applicationBuilder">The app's application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder RegisterMiddleware(this IApplicationBuilder applicationBuilder)
    {
        return applicationBuilder
            .UseExceptionHandler(e => e.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/problem+json";

                    var problemDetails = new
                    {
                        type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                        title = "Internal Server Error",
                        status = StatusCodes.Status500InternalServerError
                    };

                    await context.Response.WriteAsJsonAsync(problemDetails);
                }))
            .UseMiddleware<RequestLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints.RegisterEndpoints());
    }
}
