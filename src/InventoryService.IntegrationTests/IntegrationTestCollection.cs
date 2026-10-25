namespace InventoryService.IntegrationTests;

/// <summary>
/// The integration test collection, covering all tests in this project.
/// </summary>
[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFixture> { }