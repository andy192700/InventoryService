using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace InventoryService.UnitTests.Infrastructure;

public class RequestLoggingMiddlewareTests
{
    [Theory]
    [AutoMoqData]
    public async Task Invoke_LogsStartAndFinish(
        [Frozen] Mock<ILogger<RequestLoggingMiddleware>> mockLogger,
        string method,
        string path,
        TestRequestDelegate requestDelegate)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Method = method;
        context.Request.Path = $"/{path}";
        context.Response.StatusCode = StatusCodes.Status200OK;

        var sut = new RequestLoggingMiddleware(
            requestDelegate.Next(),
            mockLogger.Object);

        // Act
        await sut.Invoke(context);

        // Assert
        requestDelegate.NextCalled.Should().BeTrue();

        mockLogger.VerifyLog(
            LogLevel.Information,
            $"Started request {context.Request.Method} {context.Request.Path}",
            Times.Once());

        mockLogger.VerifyLog(
            LogLevel.Information,
            $"Finished request {context.Request.Method} {context.Request.Path}",
            Times.Once());

        mockLogger.VerifyExceptionLog(
            $"Failed request {context.Request.Method} {context.Request.Path}",
            It.IsAny<Exception>(),
            Times.Never());
    }

    [Theory]
    [AutoMoqData]
    public async Task Invoke_LogsErrorAndRethrows_WhenRequestFails(
        [Frozen] Mock<ILogger<RequestLoggingMiddleware>> mockLogger,
        string exceptionMessage,
        string method,
        string path,
        TestRequestDelegate requestDelegate)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Method = method;
        context.Request.Path = $"/{path}";

        var exception = new InvalidOperationException(exceptionMessage);

        var sut = new RequestLoggingMiddleware(
            requestDelegate.Next(_ => throw exception),
            mockLogger.Object);

        // Act
        var action = () => sut.Invoke(context);

        // Assert
        await action.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage(exceptionMessage);

        mockLogger.VerifyLog(
            LogLevel.Information,
            $"Started request {context.Request.Method} {context.Request.Path}",
            Times.Once());

        mockLogger.VerifyExceptionLog(
            $"Failed request {context.Request.Method} {context.Request.Path}",
            exception,
            Times.Once());

        mockLogger.VerifyLog(
            LogLevel.Information,
            $"Finished request {context.Request.Method} {context.Request.Path}",
            Times.Never());
    }
}