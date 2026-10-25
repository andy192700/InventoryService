using Microsoft.AspNetCore.Http.Extensions;

namespace InventoryService.Infrastructure;

public interface ILocationBuilder
{
    string Build(string pathBase, QueryString queryString);
}

public class LocationBuilder : ILocationBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocationBuilder(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

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
