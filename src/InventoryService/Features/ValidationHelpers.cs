namespace InventoryService.Features;

public static class ValidationHelpers
{
    public static bool IsValidIntegerId(string? value)
    {
        if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var id))
        {
            return id > 0;
        }

        return false;
    }

    public static bool IsValidDoubleId(string? value)
    {
        if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var id))
        {
            return id > 0;
        }

        return false;
    }
}
