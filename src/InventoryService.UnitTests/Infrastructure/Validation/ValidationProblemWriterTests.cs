using FluentAssertions;
using FluentValidation.Results;
using InventoryService.Infrastructure.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InventoryService.UnitTests.Infrastructure.Validation;

public class ValidationProblemWriterTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Write_CreatesValidationProblem(ValidationResult validationResult)
    {
        // Arrange
        var sut = new ValidationProblemWriter();

        // Act
        var result = sut.Write(validationResult);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ProblemHttpResult>();

        var asHttp = result as ProblemHttpResult;
        asHttp!.ProblemDetails.Should().NotBeNull();
        asHttp.ProblemDetails.Should().BeOfType<HttpValidationProblemDetails>();

        var asDetail = asHttp.ProblemDetails as HttpValidationProblemDetails;
        asDetail!.Errors.Should().HaveCount(validationResult.Errors.Count);
        asDetail!.Errors
            .All(x => validationResult.Errors.Any(y => y.PropertyName == x.Key && x.Value.Contains(y.ErrorMessage)))
            .Should().BeTrue();
    }
}
