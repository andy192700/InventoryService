using FluentAssertions;
using InventoryService.Infrastructure.Validation;

namespace InventoryService.UnitTests.Infrastructure.Validation;

public class ValidationHelpersTests
{
    [Theory]
    [InlineAutoMoqData("", false)]
    [InlineAutoMoqData(null!, false)]
    [InlineAutoMoqData("Im a string", false)]
    [InlineAutoMoqData("0", false)]
    [InlineAutoMoqData("-1", false)]
    [InlineAutoMoqData("1", true)]
    [InlineAutoMoqData("100", true)]
    public void IsValidInteger_ReturnsAsExpected(
        string id,
        bool expectedResult)
    {
        // Act
        var result = ValidationHelpers.IsValidInteger(id);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineAutoMoqData("", false)]
    [InlineAutoMoqData(null!, false)]
    [InlineAutoMoqData("Im a string", false)]
    [InlineAutoMoqData("0.0", false)]
    [InlineAutoMoqData("-1.0", false)]
    [InlineAutoMoqData("0", false)]
    [InlineAutoMoqData("-1", false)]
    [InlineAutoMoqData("1.0", true)]
    [InlineAutoMoqData("100.0", true)]
    [InlineAutoMoqData("1", true)]
    [InlineAutoMoqData("100", true)]
    public void IsValidDouble_ReturnsAsExpected(
        string id,
        bool expectedResult)
    {
        // Act
        var result = ValidationHelpers.IsValidDouble(id);

        // Assert
        result.Should().Be(expectedResult);
    }
}
