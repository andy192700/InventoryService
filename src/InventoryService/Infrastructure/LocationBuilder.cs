namespace InventoryService.Infrastructure;

/// <summary>
/// Creates a URI for a get request pointing to the newly created entity.
/// </summary>
public interface ILocationBuilder
{
    /// <summary>
    /// Builds the URI.
    /// </summary>
    /// <param name="endpointName">The endpoint's name.</param>
    /// <param name="requestobject">The request object.</param>
    /// <returns>The URI.</returns>
    string Build(string endpointName, object requestobject);
}

/// <summary>
/// Creates a URI for a get request pointing to the newly created entity.
/// </summary>
public class LocationBuilder : ILocationBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly LinkGenerator _linkGenerator;

    public LocationBuilder(
        IHttpContextAccessor httpContextAccessor,
        LinkGenerator linkGenerator)
    {
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
    }

    /// <inheritdoc/>
    public string Build(string endpointName, object requestobject)
    {
        var httpContext = _httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("No HttpContext available.");

        return _linkGenerator.GetUriByName(
            httpContext,
            endpointName,
            requestobject,
            options: new LinkOptions
            {
                LowercaseQueryStrings = true,
                LowercaseUrls = true
            }) ?? throw new InvalidOperationException($"Endpoint '{endpointName}' could not be resolved into a URI.");
    }
}
