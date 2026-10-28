using AutoFixture.Xunit2;
using FluentAssertions;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Validation;
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ProblemHttpResult>();

        var problemDetails = result as ProblemHttpResult;
        problemDetails.Should().NotBeNull();
        problemDetails.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(HttpResponses.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>>();

        var particular = result as Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>;
        particular.Should().NotBeNull();
        particular.StatusCode.Should().Be(StatusCodes.Status200OK);
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(HttpResponses.Ok(response));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>>();

        var particular = result as Microsoft.AspNetCore.Http.HttpResults.Ok<TestResponse>;
        particular.Should().NotBeNull();
        particular.StatusCode.Should().Be(StatusCodes.Status200OK);
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        var responseValue = HttpResponses.Created(response, location);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(responseValue);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Created<TestResponse>>();
        
        var particular = result as Microsoft.AspNetCore.Http.HttpResults.Created<TestResponse>;
        particular.Should().NotBeNull();
        particular.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsNotFound_WhenHandlerNotRegistered(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns((IRequestHandler<TestRequest, Result<TestResponse>>)null!);

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ProblemHttpResult>();

        var problemDetails = result as ProblemHttpResult;
        problemDetails.Should().NotBeNull();
        problemDetails.StatusCode.Should().Be(StatusCodes.Status404NotFound);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns200_WhenSuccessResultIsUnknownType(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        CancellationToken cancellation,
        Result<TestResponse> response)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<Ok>();

        var asParticular = result as Ok;
        asParticular.Should().NotBeNull();
        asParticular.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns404_WhenNotFoundErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        string message,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResponses.Notfound(message));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ProblemHttpResult>();

        var problemDetails = result as ProblemHttpResult;
        problemDetails.Should().NotBeNull();
        problemDetails.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        problemDetails.ProblemDetails.Detail.Should().Be(message);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenInternalServerErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        string message,
        CancellationToken cancellation)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResponses.InternalServerError(message));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ProblemHttpResult>();

        var problemDetails = result as ProblemHttpResult;
        problemDetails.Should().NotBeNull();
        problemDetails.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.ProblemDetails.Detail.Should().Be(message);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenUnknownErrorReturned(
        [Frozen] Mock<IServiceProvider> mockServiceProvider,
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        CancellationToken cancellation,
        IError error)
    {
        // Arrange
        mockServiceProvider
            .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
            .Returns((object?)null);

        mockServiceProvider.Setup(x => x.GetService(typeof(IRequestHandler<TestRequest, Result<TestResponse>>)))
            .Returns(mockHandler.Object);

        mockServiceProvider.Setup(x => x.GetService(typeof(IDataIntegrityEvaluator<TestRequest>)))
            .Returns((IDataIntegrityEvaluator<TestRequest>?)null);

        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(error));

        // Act
        var result = await HttpHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockServiceProvider.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ProblemHttpResult>();

        var problemDetails = result as ProblemHttpResult;
        problemDetails.Should().NotBeNull();
        problemDetails.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
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