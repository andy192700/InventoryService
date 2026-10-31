using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace InventoryService.Infrastructure.Health;

/// <summary>
/// A health check response.
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// The overall health check status.
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// A collection of individual health check results.
    /// </summary>
    public required ICollection<HealthCheckResponseCheck> Checks { get; set; }
}

/// <summary>
/// An individual health check result.
/// </summary>
public class HealthCheckResponseCheck
{
    /// <summary>
    /// The health check's tag.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// This health check's description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// This health check's status.
    /// </summary>
    public required string Value { get; set; }
}