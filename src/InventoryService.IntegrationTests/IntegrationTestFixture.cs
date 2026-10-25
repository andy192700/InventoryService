using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace InventoryService.IntegrationTests;

/// <summary>
/// Test fixture standing up a real SQL docker container,
/// which runs the flyway migrations before the tests commence.
/// Upon the tests completing, the container is destroyed.
/// </summary>
public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private const int ExposedServerPort = 45000;
    private const int InternalServerPort = 1433;

    private MsSqlContainer _sqlContainer = default!;

    /// <summary>
    /// Obtain a connection string, which points to the correct database for the integration test's database docker container.
    /// </summary>
    public string ConnectionString => new SqlConnectionStringBuilder(_sqlContainer.GetConnectionString())
    {
        InitialCatalog = "Inventory"
    }.ConnectionString;

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrong!Passw0rd")
            .WithPortBinding(ExposedServerPort, InternalServerPort)
            .Build();

        await _sqlContainer.StartAsync();

        var migrationsPath =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "..",
                    "docker",
                    "migrations"));

        var flywayContainer =
            new ContainerBuilder("flyway/flyway:latest")
                .WithBindMount(migrationsPath, "/flyway/sql")
                .WithCommand(
                    "migrate",
                    $"-url=jdbc:sqlserver://host.docker.internal:{_sqlContainer.GetMappedPublicPort(InternalServerPort)};databaseName=master;encrypt=false;trustServerCertificate=true",
                    "-user=sa",
                    "-password=YourStrong!Passw0rd",
                    "-schemas=dbo",
                    "-connectRetries=60")
                .Build();

        await flywayContainer.StartAsync();

        var exitCode = await flywayContainer.GetExitCodeAsync();

        if (exitCode != 0)
        {
            throw new Exception("Flyway migration failed.");
        }
    }

    /// <inheritdoc/>
    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }
}
