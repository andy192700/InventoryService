using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Infrastructure;
using Microsoft.AspNetCore.Http;
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
        string basePath,
        QueryString queryString,
        string scheme,
        HostString hostString)
    {
        // Arrange
        mockHttpAccessor.Setup(x => x.HttpContext)
            .Returns(mockHttpContext.Object);

        mockHttpContext.Setup(x => x.Request)
            .Returns(mockHttpRequest.Object);

        mockHttpRequest.Setup(x => x.Scheme)
            .Returns(scheme);

        mockHttpRequest.Setup(x => x.Host)
            .Returns(hostString);

        var sut = new LocationBuilder(mockHttpAccessor.Object);

        // Act
        var result = sut.Build($"/{basePath}", queryString);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be($"{scheme}://{hostString}/{basePath}{queryString}");
    }

    [Theory]
    [InlineAutoMoqData]
    public void Build_Throws_WhenHttpContextIsNull(
        [Frozen] Mock<IHttpContextAccessor> mockHttpAccessor,
        string basePath,
        QueryString queryString)
    {
        // Arrange
        mockHttpAccessor.Setup(x => x.HttpContext)
            .Returns((HttpContext?)null);

        var sut = new LocationBuilder(mockHttpAccessor.Object);

        // Act
        var func = () => sut.Build($"/{basePath}", queryString);

        // Assert
        func.Should().Throw<InvalidOperationException>().WithMessage("No HttpContext available.");
    }
}