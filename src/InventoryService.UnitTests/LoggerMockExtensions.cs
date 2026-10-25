using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryService.UnitTests;

public static class LoggerMockExtensions
{
    /// <summary>
    /// Verify that the mock logger has indeed logged as required.
    /// </summary>
    /// <typeparam name="T">The type in which the logger is observing.</typeparam>
    /// <param name="logger">The logger.</param>
    /// <param name="level">The log level.</param>
    /// <param name="containsMessage">The message that ought to have been logged.</param>
    /// <param name="times">The number of occurrences.</param>
    public static void VerifyLog<T>(
        this Mock<ILogger<T>> logger,
        LogLevel level,
        string containsMessage,
        Times times)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v == null ? false : v.ToString()!.Contains(containsMessage)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    /// <summary>
    /// Verify that the mock logger has indeed logged an exception as required.
    /// </summary>
    /// <typeparam name="T">The type in which the logger is observing.</typeparam>
    /// <param name="logger">The logger.</param>
    /// <param name="containsMessage">The message that ought to have been logged.</param>
    /// <param name="expectedException">The exception which should have been logged.</param>
    /// <param name="times">The number of occurrences.</param>
    public static void VerifyExceptionLog<T>(
        this Mock<ILogger<T>> logger,
        string containsMessage,
        Exception expectedException,
        Times times)
    {
        logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) =>
                    v == null ? false : v.ToString()!.Contains(containsMessage)),
                It.Is<Exception>(e =>
                    ReferenceEquals(e, expectedException)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

}