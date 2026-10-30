using InventoryService.Infrastructure.Auth;

namespace InventoryService.IntegrationTests;

/// <summary>
/// An integration test base class, which is the basis for all integration test classes in this project.
/// </summary>
[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestBase : IAsyncDisposable
{
    private const string BasicAuthHeaderValue = $"Basic YWRtaW46TXlQYXNzd29yZCE=";

    private InventoryServiceWebApplicationFactory? _webApplicationFactory;

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

    protected void AddBasicAuthHeader()
    {
        Client.DefaultRequestHeaders.Add(AuthenticationConstants.AuthorizationHeader, BasicAuthHeaderValue);
    }

}