using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryService.UnitTests;

public static class LoggerMockExtensions
{
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