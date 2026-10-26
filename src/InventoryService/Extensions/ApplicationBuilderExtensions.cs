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
        return app.UseMiddleware<RequestLoggingMiddleware>()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints => app.RegisterEndpoints());
    }
}
