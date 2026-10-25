using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Countries.GetCountries;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.Countries.GetCountries;

public class GetCountriesHandlerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetCountriesDBQuery> mockQuery,
        [Frozen] Mock<IGetCountriesResponseMapper> mockMapper,
        GetCountriesRequest request,
        GetCountriesDBResponse dbResponse,
        ICollection<GetCountriesResponse> response,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockQuery.Setup(x => x.GetCountriesAsync(cancellationToken))
            .ReturnsAsync(dbResponse);

        mockMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetCountriesHandler(mockQuery.Object, mockMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);

        mockQuery.Verify(x => x.GetCountriesAsync(cancellationToken), Times.Once);
        mockMapper.Verify(x => x.Map(dbResponse), Times.Once);
    }
}
