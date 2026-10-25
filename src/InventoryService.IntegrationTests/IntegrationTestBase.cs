namespace InventoryService.IntegrationTests;

/// <summary>
/// An integration test base class, which is the basis for all integration test classes in this project.
/// </summary>
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