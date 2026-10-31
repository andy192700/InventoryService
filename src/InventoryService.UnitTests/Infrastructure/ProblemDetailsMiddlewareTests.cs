using FluentAssertions;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;

namespace InventoryService.UnitTests.Infrastructure;

public class ProblemDetailsMiddlewareTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task InvokeAsync_Returns_WhenRequestHasNotStarted(
        Mock<IProblemDetailsService> mockProblemDetailsService,
        Mock<HttpContext> mockContext,
        Mock<HttpResponse> mockResponse,
        TestRequestDelegate testRequestDelegate)
    {
        // Arrange
        mockResponse.Setup(x => x.HasStarted)
            .Returns(true);

        mockContext.Setup(x => x.Response)
            .Returns(mockResponse.Object);

        var sut = new ProblemDetailsMiddleware(testRequestDelegate.Next());

        // Act
        await sut.InvokeAsync(mockContext.Object, mockProblemDetailsService.Object);

        // Assert
        testRequestDelegate.NextCalled.Should().BeTrue();

        mockProblemDetailsService.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task InvokeAsync_Returns_WhenRequestHasANoneErrorResponseCode(
        Mock<IProblemDetailsService> mockProblemDetailsService,
        Mock<HttpContext> mockContext,
        Mock<HttpResponse> mockResponse,
        TestRequestDelegate testRequestDelegate)
    {
        // Arrange
        mockResponse.Setup(x => x.StatusCode)
            .Returns(StatusCodes.Status200OK);

        mockContext.Setup(x => x.Response)
            .Returns(mockResponse.Object);

        var sut = new ProblemDetailsMiddleware(testRequestDelegate.Next());

        // Act
        await sut.InvokeAsync(mockContext.Object, mockProblemDetailsService.Object);

        // Assert
        testRequestDelegate.NextCalled.Should().BeTrue();

        mockProblemDetailsService.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task InvokeAsync_Returns_WhenRequestHasContentTypeSet(
        Mock<IProblemDetailsService> mockProblemDetailsService,
        Mock<HttpContext> mockContext,
        Mock<HttpResponse> mockResponse,
        TestRequestDelegate testRequestDelegate,
        string contentType)
    {
        // Arrange
        mockResponse.Setup(x => x.StatusCode)
            .Returns(StatusCodes.Status403Forbidden);

        mockResponse.Setup(x => x.ContentType)
            .Returns(contentType);

        mockContext.Setup(x => x.Response)
            .Returns(mockResponse.Object);

        var sut = new ProblemDetailsMiddleware(testRequestDelegate.Next());

        // Act
        await sut.InvokeAsync(mockContext.Object, mockProblemDetailsService.Object);

        // Assert
        testRequestDelegate.NextCalled.Should().BeTrue();

        mockProblemDetailsService.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task InvokeAsync_Returns_WhenRequestHasContentLengthSet(
        Mock<IProblemDetailsService> mockProblemDetailsService,
        Mock<HttpContext> mockContext,
        Mock<HttpResponse> mockResponse,
        TestRequestDelegate testRequestDelegate,
        int contentLength)
    {
        // Arrange
        mockResponse.Setup(x => x.StatusCode)
            .Returns(StatusCodes.Status403Forbidden);

        mockResponse.Setup(x => x.ContentLength)
            .Returns(contentLength);

        mockContext.Setup(x => x.Response)
            .Returns(mockResponse.Object);

        var sut = new ProblemDetailsMiddleware(testRequestDelegate.Next());

        // Act
        await sut.InvokeAsync(mockContext.Object, mockProblemDetailsService.Object);

        // Assert
        testRequestDelegate.NextCalled.Should().BeTrue();

        mockProblemDetailsService.Verify(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }

    [Theory]
    [InlineAutoMoqData(400, "Bad Request", "https://tools.ietf.org/html/rfc9110#section-15.5.1")]
    [InlineAutoMoqData(401, "Unauthorized", "https://tools.ietf.org/html/rfc9110#section-15.5.2")]
    [InlineAutoMoqData(403, "Forbidden", "https://tools.ietf.org/html/rfc9110#section-15.5.4")]
    [InlineAutoMoqData(404, "Not Found", "https://tools.ietf.org/html/rfc9110#section-15.5.5")]
    [InlineAutoMoqData(500, "Internal Server Error", "https://tools.ietf.org/html/rfc9110#section-15.6.1")]
    [InlineAutoMoqData(9750, "An error occurred", "about:blank")]
    public async Task InvokeAsync_WritesResponse(
        int statusCode,
        string expectedTitle,
        string expectedType,
        Mock<IProblemDetailsService> mockProblemDetailsService,
        Mock<HttpContext> mockContext,
        Mock<HttpResponse> mockResponse,
        TestRequestDelegate testRequestDelegate)
    {
        // Arrange
        mockResponse.Setup(x => x.StatusCode)
            .Returns(statusCode);

        mockContext.Setup(x => x.Response)
            .Returns(mockResponse.Object);

        ProblemDetailsContext? capturedContext = default;

        mockProblemDetailsService.Setup(x => x.WriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Returns(ValueTask.CompletedTask)
            .Callback((ProblemDetailsContext x) => capturedContext = x);

        var sut = new ProblemDetailsMiddleware(testRequestDelegate.Next());

        // Act        
        await sut.InvokeAsync(mockContext.Object, mockProblemDetailsService.Object);

        // Assert
        testRequestDelegate.NextCalled.Should().BeTrue();

        capturedContext.Should().NotBeNull();
        capturedContext.ProblemDetails.Status.Should().Be(statusCode);
        capturedContext.ProblemDetails.Type.Should().Be(expectedType);
        capturedContext.ProblemDetails.Title.Should().Be(expectedTitle);

        mockProblemDetailsService.Verify(x => x.WriteAsync(capturedContext), Times.Once);
    }
}
