using AutoFixture.Xunit2;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;

namespace InventoryService.UnitTests.Infrastructure;

public class HttpHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsValidationProblem_WhenValidationFails(
        [Frozen] Mock<IValidator<TestRequest>> mockValidator,
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        IRequestHandler<TestRequest, Result<TestResponse>> handler,
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(handler);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IValueHttpResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsOk_WhenMediatorReturnsOk(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(HttpResponses.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsOk_WhenValidationSucceeds(
        [Frozen] Mock<IValidator<TestRequest>> mockValidator,
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(HttpResponses.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsCreated_WhenMediatorReturnsCreated(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        [Frozen] Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation,
        string location)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(HttpResponses.Created(response, location));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Created<TestResponse>>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenSuccessResultIsUnknownType(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        IRequestHandler<TestRequest, Result<TestResponse>> handler,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(handler);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns404_WhenNotFoundErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        IRequestHandler<TestRequest, Result<TestResponse>> handler,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(handler);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenInternalServerErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        IRequestHandler<TestRequest, Result<TestResponse>> handler,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(handler);


        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenUnknownErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        IRequestHandler<TestRequest, Result<TestResponse>> handler,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(handler);            

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<IResult>();
    }

    public sealed record TestRequest(string Name)
        : IRequest<Result<TestResponse>>;

    public sealed record TestResponse(string Name);

    public class TestHandler : IRequestHandler<TestRequest, Result<TestResponse>>
    {
        public virtual Task<Result<TestResponse>> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return default!;
        }
    }
}