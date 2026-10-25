using Microsoft.AspNetCore.Http.Extensions;

namespace InventoryService.Infrastructure;

/// <summary>
/// Creates a URI for a get request pointing to the newly created entity.
/// </summary>
public interface ILocationBuilder
{
    /// <summary>
    /// Builds the URI.
    /// </summary>
    /// <param name="pathBase">The base path.</param>
    /// <param name="queryString">The URI's query string.</param>
    /// <returns>The URI.</returns>
    string Build(string pathBase, QueryString queryString);
}

/// <summary>
/// Creates a URI for a get request pointing to the newly created entity.
/// </summary>
public class LocationBuilder : ILocationBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocationBuilder(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc/>
    public string Build(string pathBase, QueryString queryString)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext available.");

        return UriHelper.BuildAbsolute(
            httpContext.Request.Scheme,
            httpContext.Request.Host,
            pathBase,
            query: queryString);
    }
}
