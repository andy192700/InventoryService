using InventoryService.Infrastructure.Auth;
using System.Text;

namespace InventoryService.IntegrationTests;

/// <summary>
/// An integration test base class, which is the basis for all integration test classes in this project.
/// </summary>
[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestBase : IAsyncDisposable
{
    private const string BasicAuthHeaderValue = $"Basic YWRtaW46TXlQYXNzd29yZCE=";

    protected readonly InventoryServiceWebApplicationFactory WebApplicationFactory;
    
    protected readonly HttpClient Client;

    protected IntegrationTestBase(IntegrationTestFixture sqlFixture)
    {
        WebApplicationFactory = new InventoryServiceWebApplicationFactory(sqlFixture);

        Client = WebApplicationFactory.CreateClient();
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