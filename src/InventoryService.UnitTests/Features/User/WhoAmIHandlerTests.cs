using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Features.Products.CreateProduct;
using InventoryService.Features.User.WhoAmI;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Auth;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace InventoryService.UnitTests.Features.User;

public class WhoAmIHandlerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsUnauthorized_WhenContextIsNull(
        [Frozen] Mock<IHttpContextAccessor> mockAccessor,
        WhoAmIRequest request,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockAccessor.Setup(x => x.HttpContext)
            .Returns((HttpContext?)null);

        var sut = new WhoAmIHandler(mockAccessor.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<UnauthorizedError>();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsUnauthorized_WhenIdentityIsNull(
        [Frozen] Mock<IHttpContextAccessor> mockAccessor,
        [Frozen] Mock<HttpContext> mockContext,
        WhoAmIRequest request,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockAccessor.Setup(x => x.HttpContext)
            .Returns(mockContext.Object);

        mockContext.Setup(x => x.User)
            .Returns(new ClaimsPrincipal([]));

        var sut = new WhoAmIHandler(mockAccessor.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<UnauthorizedError>();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsUnauthorized_WhenNotAuthenticated(
        [Frozen] Mock<IHttpContextAccessor> mockAccessor,
        [Frozen] Mock<HttpContext> mockContext,
        WhoAmIRequest request,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockAccessor.Setup(x => x.HttpContext)
            .Returns(mockContext.Object);

        mockContext.Setup(x => x.User)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity(authenticationType: default)));

        var sut = new WhoAmIHandler(mockAccessor.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<UnauthorizedError>();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsUnauthorized_WhenUserNameIsNull(
        [Frozen] Mock<IHttpContextAccessor> mockAccessor,
        [Frozen] Mock<HttpContext> mockContext,
        WhoAmIRequest request,
        string authType,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockAccessor.Setup(x => x.HttpContext)
            .Returns(mockContext.Object);

        mockContext.Setup(x => x.User)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity(authenticationType: authType)));

        var sut = new WhoAmIHandler(mockAccessor.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].Should().BeAssignableTo<UnauthorizedError>();
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task Handle_ReturnsOk(
        [Frozen] Mock<IHttpContextAccessor> mockAccessor,
        [Frozen] Mock<HttpContext> mockContext,
        WhoAmIRequest request,
        List<string> userPermissions,
        string userName,
        string scheme,
        CancellationToken cancellationToken)
    {
        // Arrange
        mockAccessor.Setup(x => x.HttpContext)
            .Returns(mockContext.Object);

        var claims = userPermissions.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims.Add(new Claim(ClaimTypes.Name, userName));

        var identity = new ClaimsIdentity(claims, scheme);

        var principal = new ClaimsPrincipal(identity);

        mockContext.Setup(x => x.User)
            .Returns(principal);

        var sut = new WhoAmIHandler(mockAccessor.Object);

        // Act
        var result = await sut.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Should().BeAssignableTo<IOk>();
        result.Value.UserName.Should().Be(userName);
        result.Value.Roles.Should().HaveCount(userPermissions.Count);
        result.Value.Roles.All(x => userPermissions.Contains(x)).Should().BeTrue();
    }
}
