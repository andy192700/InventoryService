namespace InventoryService.IntegrationTests;

[Collection(nameof(IntegrationTestCollection))]
public abstract class IntegrationTestBase
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(IntegrationTestFixture sqlFixture)
    {
        var factory = new InventoryServiceWebApplicationFactory(sqlFixture);

        Client = factory.CreateClient();
    }
}