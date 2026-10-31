using InventoryService.Infrastructure.Auth;

namespace InventoryService.IntegrationTests;

/// <summary>
/// An integration test base class, which is the basis for all integration test classes in this project.
/// </summary>
[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestBase : IAsyncDisposable
{
    private const string BasicAuthAdminHeaderValue = $"Basic YWRtaW46TXlQYXNzd29yZCE=";

    private const string BasicAuthReadOnlyHeaderValue = $"Basic dXNlcjpNeVBhc3N3b3JkITEy";

    private const string BasicAuthNoPermissionsHeaderValue = $"Basic bm9uZXVzZXI6TXlQYXNzd29yZDEyMyE=";

    private InventoryServiceWebApplicationFactory? _webApplicationFactory;

    /// <summary>
    /// Get the test isntance's web application factory, which consumes program.cs.
    /// </summary>
    protected InventoryServiceWebApplicationFactory WebApplicationFactory
    {
        get
        {
            if (_webApplicationFactory is null)
            {
                _webApplicationFactory = new InventoryServiceWebApplicationFactory(_sqlFixture);
            }

            return _webApplicationFactory;
        }
    }

    private HttpClient? _client;

    /// <summary>
    /// Get an HttpClient capable of calling "real" endpoints, derived from the web application factory.
    /// </summary>
    protected HttpClient Client
    {
        get
        {
            if (_client is null)
            {
                _client = WebApplicationFactory.CreateClient();
            }

            return _client;
        }
    }

    private readonly IntegrationTestFixture _sqlFixture;

    protected IntegrationTestBase(IntegrationTestFixture sqlFixture)
    {
        _sqlFixture = sqlFixture;
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        Client.Dispose();

        await WebApplicationFactory.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Adds the admin user to the current <see cref="HttpClient"/>.
    /// </summary>
    protected void AddAdminBasicAuthHeader()
    {
        Client.DefaultRequestHeaders.Add(AuthenticationConstants.AuthorizationHeader, BasicAuthAdminHeaderValue);
    }

    /// <summary>
    /// Adds a read only user to the current <see cref="HttpClient"/>.
    /// </summary>
    protected void AddReadOnlyBasicAuthHeader()
    {
        Client.DefaultRequestHeaders.Add(AuthenticationConstants.AuthorizationHeader, BasicAuthReadOnlyHeaderValue);
    }

    /// <summary>
    /// Adds a read only user to the current <see cref="HttpClient"/>.
    /// </summary>
    protected void AddNoPermissionBasicAuthHeader()
    {
        Client.DefaultRequestHeaders.Add(AuthenticationConstants.AuthorizationHeader, BasicAuthNoPermissionsHeaderValue);
    }
}