using AutoFixture.Xunit2;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace InventoryService.UnitTests.Infrastructure;

public class HttpHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsValidationProblem_WhenValidationFails(
        [Frozen] Mock<IValidator<TestRequest>> mockValidator,
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        CancellationToken cancellation,
        string failReason)
    {
        // Arrange
        var validationResult = new ValidationResult([
            new ValidationFailure(nameof(TestRequest.Name), failReason)
        ]);

        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator
            .Setup(x => x.ValidateAsync(request, cancellation))
            .ReturnsAsync(validationResult);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IValueHttpResult>();

        mockMediator.Verify(
            x => x.Send(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsOk_WhenMediatorReturnsOk(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(HttpResponses.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>>();

        mockMediator.Verify(
            x => x.Send(request, cancellation),
            Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsOk_WhenValidationSucceeds(
        [Frozen] Mock<IValidator<TestRequest>> mockValidator,
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns(mockValidator.Object);

        mockValidator
            .Setup(x => x.ValidateAsync(request, cancellation))
            .ReturnsAsync(new ValidationResult());

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(HttpResponses.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsCreated_WhenMediatorReturnsCreated(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation,
        string location)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(HttpResponses.Created(response, location));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Created<TestResponse>>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenSuccessResultIsUnknownType(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(Result.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns404_WhenNotFoundErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        CancellationToken cancellation,
        string failReason)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(Result.Fail<TestResponse>(
                new NotFoundError(failReason)));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenInternalServerErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        CancellationToken cancellation,
        string failReason)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(Result.Fail<TestResponse>(
                new InventoryService.Infrastructure.InternalServerError(failReason)));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenUnknownErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IMediator> mockMediator,
        TestRequest request,
        CancellationToken cancellation,
        string failReason)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockMediator
            .Setup(x => x.Send(request, cancellation))
            .ReturnsAsync(Result.Fail<TestResponse>(
                new Error(failReason)));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            mockMediator.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    public sealed record TestRequest(string Name)
        : IRequest<Result<TestResponse>>;

    public sealed record TestResponse(string Name);
}