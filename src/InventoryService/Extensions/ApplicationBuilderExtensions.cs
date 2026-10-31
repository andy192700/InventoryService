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
            .UseMiddleware<ProblemDetailsMiddleware>()
            .UseExceptionHandler(e => e.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }))
            .UseMiddleware<RequestLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => endpoints.RegisterEndpoints());
    }
}
