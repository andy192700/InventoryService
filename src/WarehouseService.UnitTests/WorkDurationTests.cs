using FluentAssertions;

namespace WarehouseService.UnitTests;

public class WorkDurationTests
{
    [Fact]
    public void GetDuration_ReturnsDuration()
    {
        // Arrange
        var sut = new WorkDuration();

        // Act
        var result = sut.GetDuration();

        // Assert
        result.Should().BeGreaterThan(0);
    }
}
