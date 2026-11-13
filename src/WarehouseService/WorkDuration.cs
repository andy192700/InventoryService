namespace WarehouseService;

/// <summary>
/// Gets the duration of warehouse work time in seconds.
/// </summary>
public interface IWorkDuration
{
    /// <summary>
    /// Return the duration in seconds.
    /// </summary>
    /// <returns>The duration.</returns>
    int GetDuration();
}

/// <summary>
/// Gets the duration of warehouse work time in seconds.
/// </summary>
public class WorkDuration : IWorkDuration
{
    private readonly Random _random = new();

    /// <inheritdoc/>
    public int GetDuration()
    {
        return _random.Next(10, 51);
    }
}
