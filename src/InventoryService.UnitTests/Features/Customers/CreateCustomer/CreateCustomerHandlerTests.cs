using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Customers.CreateCustomer;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;

namespace InventoryService.UnitTests.Features.Customers.CreateCustomer;

public class CreateCustomerHandlerTests
{
    [Theory]
    [AutoMoqData]
    public async Task Handle_ReturnsCreated(
        [Frozen] Mock<ICreateCustomerDBCommand> mockCommand,
        [Frozen] Mock<ICreateCustomerResponseMapper> mockResponseMapper,
        [Frozen] Mock<ICreateCustomerDBParametersMapper> mockParamsMapper,
        [Frozen] Mock<ILocationBuilder> mockLocationBuilder,
        CreateCustomerDBParameters dbParams,
        CreateCustomerRequest request,
        CreateCustomerResponse response,
        CreateCustomerDBResponse dbResponse,
        string location,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockCommand.Setup(x => x.CreateCustomerAsync(dbParams, cancellationToken))
            .ReturnsAsync(dbResponse);

        mockResponseMapper.Setup(x => x.Map(dbResponse))
            .Returns(response);

        mockParamsMapper.Setup(x => x.Map(request))
            .Returns(dbParams);

        mockLocationBuilder.Setup(x => x.Build(It.IsAny<string>(), It.IsAny<QueryString>()))
            .Returns(location);

        var sut = new CreateCustomerHandler(
            mockCommand.Object,
            mockResponseMapper.Object,
            mockParamsMapper.Object,
            mockLocationBuilder.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<ICreated>();
        result.IsSuccess.Should().BeTrue();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().BeEquivalentTo(response);

        var asCreatedResult = result as Created<CreateCustomerResponse>;
        asCreatedResult.Should().NotBeNull();
        asCreatedResult.Location.Should().Be(location);

        mockCommand.Verify(x => x.CreateCustomerAsync(dbParams, cancellationToken), Times.Once);
        mockResponseMapper.Verify(x => x.Map(dbResponse), Times.Once);
        mockParamsMapper.Verify(x => x.Map(request), Times.Once);
        mockLocationBuilder.Verify(x => x.Build(It.IsAny<string>(), It.IsAny<QueryString>()), Times.Once);
    }
}
