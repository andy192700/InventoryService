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

public class RequestHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_ReturnsValidationProblem_WhenValidationFails(
        [Frozen] Mock<IValidator<TestRequest>> mockValidator,
        TestRequest request,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        CancellationToken cancellation,
        string failReason)
    {
        // Arrange
        var validationResult = new ValidationResult([
            new ValidationFailure(nameof(TestRequest.Name), failReason)
        ]);

        mockValidator.Setup(x => x.ValidateAsync(request, cancellation))
            .ReturnsAsync(validationResult);

        mockWriter.Setup(x => x.Write(validationResult))
            .Returns(Results.ValidationProblem([]));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            default!,
            mockValidator.Object,
            default,
            mockWriter.Object,
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
    public async Task HandleAsync_ReturnsDataValidationProblem_WhenValidationFails(
        [Frozen] Mock<IDataIntegrityEvaluator<TestRequest>> mockDataEvaluator,
        TestRequest request,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        CancellationToken cancellation,
        string failReason)
    {
        // Arrange
        var dataValidationResult = new ValidationResult([
            new ValidationFailure(nameof(TestRequest.Name), failReason)
        ]);

        mockDataEvaluator.Setup(x => x.EvaluateAsync(request, cancellation))
            .ReturnsAsync(dataValidationResult);

        mockWriter.Setup(x => x.Write(dataValidationResult))
            .Returns(Results.ValidationProblem([]));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            default!,
            default,
            mockDataEvaluator.Object,
            mockWriter.Object,
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
        [Frozen] Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(RequestHandlerResponses.Ok(response));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
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
        [Frozen] Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation)
    {
        // Arrange
        mockValidator
            .Setup(x => x.ValidateAsync(request, cancellation))
            .ReturnsAsync(new ValidationResult());

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(RequestHandlerResponses.Ok(response));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            mockValidator.Object,
            default,
            mockWriter.Object,
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
        [Frozen] Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        TestRequest request,
        TestResponse response,
        CancellationToken cancellation,
        string location)
    {
        // Arrange
        var responseValue = RequestHandlerResponses.Created(response, location);

        mockHandler.Setup(x => x.Handle(request, cancellation))
            .ReturnsAsync(responseValue);

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
            cancellation);

        // Assert
        result.Should().BeAssignableTo<Microsoft.AspNetCore.Http.HttpResults.Created<TestResponse>>();
        
        var particular = result as Microsoft.AspNetCore.Http.HttpResults.Created<TestResponse>;
        particular.Should().NotBeNull();
        particular.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns200_WhenSuccessResultIsUnknownType(
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        CancellationToken cancellation)
    {
        // Arrange
        var response = new TestResult(new TestResponse(""));

        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
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
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        string message,
        CancellationToken cancellation)
    {
        // Arrange
        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestHandlerResponses.Notfound(message));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
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
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        string message,
        CancellationToken cancellation)
    {
        // Arrange
        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestHandlerResponses.InternalServerError(message));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
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
    public async Task HandleAsync_Returns401_WhenUnAuthorizedErrorReturned(
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        CancellationToken cancellation)
    {
        // Arrange
        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(RequestHandlerResponses.UnauthorizedError());

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
            cancellation);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ProblemHttpResult>();

        var problemDetails = result as ProblemHttpResult;
        problemDetails.Should().NotBeNull();
        problemDetails.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Theory]
    [AutoMoqData]
    public async Task HandleAsync_Returns500_WhenUnknownErrorReturned(
        TestRequest request,
        Mock<IRequestHandler<TestRequest, Result<TestResponse>>> mockHandler,
        [Frozen] Mock<IValidationProblemWriter> mockWriter,
        CancellationToken cancellation,
        IError error)
    {
        // Arrange
        mockHandler.Setup(x => x.Handle(It.IsAny<TestRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(error));

        // Act
        var result = await RequestHandler.HandleAsync<TestRequest, TestResponse>(
            request,
            mockHandler.Object,
            default,
            default,
            mockWriter.Object,
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

    public class TestResult : Result<TestResponse>
    {
        public TestResult(TestResponse response)
        {
            WithValue(response);
        }
    }

    public class TestHandler : IRequestHandler<TestRequest, Result<TestResponse>>
    {
        public virtual Task<Result<TestResponse>> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return default!;
        }
    }
}