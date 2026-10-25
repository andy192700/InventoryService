using FluentAssertions;
using InventoryService.Features;

namespace InventoryService.UnitTests.Features;

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
    public void IsValidIntegerId_ReturnsAsExpected(
        string id,
        bool expectedResult)
    {
        // Act
        var result = ValidationHelpers.IsValidIntegerId(id);

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
    public void IsValidDoubleId_ReturnsAsExpected(
        string id,
        bool expectedResult)
    {
        // Act
        var result = ValidationHelpers.IsValidDoubleId(id);

        // Assert
        result.Should().Be(expectedResult);
    }
}
