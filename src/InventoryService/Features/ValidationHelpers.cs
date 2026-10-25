namespace InventoryService.Features;

public static class ValidationHelpers
{
    /// <summary>
    /// Validate an integer is positive and not empty.
    /// </summary>
    /// <param name="value">The supplied string.</param>
    /// <returns>A boolean indicating success.</returns>
    public static bool IsValidIntegerId(string? value)
    {
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var id))
        {
            return id > 0;
        }

        return false;
    }

    /// <summary>
    /// Validate an double is positive and not empty.
    /// </summary>
    /// <param name="value">The supplied string.</param>
    /// <returns>A boolean indicating success.</returns>
    public static bool IsValidDoubleId(string? value)
    {
        if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var id))
        {
            return id > 0;
        }

        return false;
    }
}
