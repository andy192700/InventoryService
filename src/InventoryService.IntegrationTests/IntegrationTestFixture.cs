using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace InventoryService.IntegrationTests;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private const int ExposedServerPort = 45000;
    private const int InternalServerPort = 1433;

    private MsSqlContainer _sqlContainer = default!;

    public string ConnectionString => new SqlConnectionStringBuilder(_sqlContainer.GetConnectionString())
    {
        InitialCatalog = "Inventory"
    }.ConnectionString;

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

    public async Task DisposeAsync()
    {
        await _sqlContainer.DisposeAsync();
    }
}
