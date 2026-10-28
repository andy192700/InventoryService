namespace InventoryService.Infrastructure.Validation;

public static class ValidationHelpers
{
    /// <summary>
    /// Validate an integer is positive and not empty.
    /// </summary>
    /// <param name="testString">The supplied string.</param>
    /// <returns>A boolean indicating success.</returns>
    public static bool IsValidInteger(string? testString)
    {
        if (!string.IsNullOrEmpty(testString) && int.TryParse(testString, out var value))
        {
            return value > 0;
        }

        return false;
    }

    /// <summary>
    /// Validate an double is positive and not empty.
    /// </summary>
    /// <param name="testString">The supplied string.</param>
    /// <returns>A boolean indicating success.</returns>
    public static bool IsValidDouble(string? testString)
    {
        if (!string.IsNullOrEmpty(testString) && double.TryParse(testString, out var value))
        {
            return value > 0;
        }

        return false;
    }
}
