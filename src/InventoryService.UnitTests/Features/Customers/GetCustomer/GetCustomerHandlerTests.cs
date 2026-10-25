using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Customers.GetCustomer;
using InventoryService.Infrastructure;
using Moq;

namespace InventoryService.UnitTests.Features.Customers.GetCustomer;

public class GetCustomerHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IGetCustomerDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetCustomerDBQuery> mockQuery,
        [Frozen] Mock<IGetCustomerResponseMapper> mockResponseMapper,
        GetCustomerDBParameters dbParameters,
        GetCustomerDBResponse dbResponse,
        GetCustomerRequest request,
        GetCustomerResponse response,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParameters);

        mockQuery.Setup(x => x.GetCustomerAsync(dbParameters, cancellationToken))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetCustomerHandler(mockParamsMapper.Object, mockQuery.Object, mockResponseMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IOk>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().Be(response);

        mockQuery.Verify(x => x.GetCustomerAsync(dbParameters, cancellationToken), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
    }

    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsNotFound(
        [Frozen] Mock<IGetCustomerDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<IGetCustomerDBQuery> mockQuery,
        [Frozen] Mock<IGetCustomerResponseMapper> mockResponseMapper,
        GetCustomerDBParameters dbParameters,
        GetCustomerDBResponse dbResponse,
        GetCustomerRequest request,
        GetCustomerResponse response,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParameters);

        mockQuery.Setup(x => x.GetCustomerAsync(dbParameters, cancellationToken))
            .ReturnsAsync(null as GetCustomerDBResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        var sut = new GetCustomerHandler(mockParamsMapper.Object, mockQuery.Object, mockResponseMapper.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<NotFoundError>();
        result.Errors[0].Message.Should().Be("Customer not found");

        mockQuery.Verify(x => x.GetCustomerAsync(dbParameters, cancellationToken), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Never);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
    }
}
