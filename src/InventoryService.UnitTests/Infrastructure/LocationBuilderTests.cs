using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace InventoryService.UnitTests.Infrastructure;

public class LocationBuilderTests
{
    [Theory]
    [InlineAutoMoqData]
    public void Build_CreatesURIString(
        [Frozen] Mock<IHttpContextAccessor> mockHttpAccessor,
        [Frozen] Mock<HttpContext> mockHttpContext,
        [Frozen] Mock<HttpRequest> mockHttpRequest,
        [Frozen] Mock<LinkGenerator> mockLinkGenerator,
        string endPointName,
        object requestObject,
        string location)
    {
        // Arrange
        mockHttpAccessor.Setup(x => x.HttpContext)
            .Returns(mockHttpContext.Object);

        mockLinkGenerator.Setup(x => x.GetUriByAddress(
            mockHttpContext.Object,
            endPointName,
            It.IsAny<RouteValueDictionary>(),
            default,
            default,
            default,
            default,
            default,
            It.IsAny<LinkOptions>()))
            .Returns(location);

        var sut = new LocationBuilder(mockHttpAccessor.Object, mockLinkGenerator.Object);

        // Act
        var result = sut.Build(endPointName, requestObject);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(location);
    }

    [Theory]
    [InlineAutoMoqData]
    public void Build_Throws_WhenHttpContextIsNull(
        [Frozen] Mock<IHttpContextAccessor> mockHttpAccessor,
        [Frozen] Mock<LinkGenerator> mockLinkGenerator,
        string endPointName,
        object requestObject)
    {
        // Arrange
        mockHttpAccessor.Setup(x => x.HttpContext)
            .Returns((HttpContext?)null);

        var sut = new LocationBuilder(mockHttpAccessor.Object, mockLinkGenerator.Object);

        // Act
        var func = () => sut.Build(endPointName, requestObject);

        // Assert
        func.Should().Throw<InvalidOperationException>().WithMessage("No HttpContext available.");
    }
}