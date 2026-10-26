using Microsoft.AspNetCore.Http;

namespace InventoryService.UnitTests;

/// <summary>
/// A test delegate to assist us testing middleware
/// </summary>
public class TestRequestDelegate
{
    public bool NextCalled { get; set; }

    /// <summary>
    /// A positive next middleware step, leaving behind an easy to access indicator that its done its business and left.
    /// </summary>
    /// <returns>The delegate representing the upcoming works.</returns>
    public RequestDelegate Next()
    {
        return ctx =>
        {
            NextCalled = true;

            return Task.CompletedTask;
        };
    }

    /// <summary>
    /// A "test level" controllable next middleware step.
    /// </summary>
    /// <returns>The delegate representing the upcoming works.</returns>
    public RequestDelegate Next(Func<HttpContext, Task> operation)
    {
        return ctx =>
        {
            return operation.Invoke(ctx);
        };
    }
}
