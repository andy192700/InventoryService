using AutoFixture.Xunit2;
using FluentAssertions;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using System.Text;

namespace InventoryService.UnitTests.Infrastructure.Auth;

public class BasicAuthenticationHandlerTests
{
    [Theory]
    [InlineAutoMoqData]
    public async Task HandleAuthenticateAsync_ReturnsFail_WhenHeaderMissing(
        [Frozen] Mock<IComputeHash> computeHashMock,
        [Frozen] Mock<IVerifyUserQuery> mockVerifyUserQuery,
        [Frozen] Mock<IOptionsMonitor<AuthenticationSchemeOptions>> mockOptionsMonitor)
    {
        // Arrange
        var context = new DefaultHttpContext();

        mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(new AuthenticationSchemeOptions());

        var loggerFactory = NullLoggerFactory.Instance;
        var encoder = System.Text.Encodings.Web.UrlEncoder.Default;

        var sut = new BasicAuthenticationHandler(
            mockOptionsMonitor.Object,
            loggerFactory,
            encoder,
            computeHashMock.Object,
            mockVerifyUserQuery.Object);

        var scheme = new AuthenticationScheme(AuthenticationConstants.AuthenticationType, AuthenticationConstants.AuthenticationType, typeof(BasicAuthenticationHandler));
        
        await sut.InitializeAsync(scheme, context);

        // Act
        var result = await sut.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Should().NotBeNull();
        result.Failure.Message.Should().Be("Missing Authorization Header");
    }

    [Theory]
    [InlineAutoMoqData(null!, "Missing Authorization Header")]
    [InlineAutoMoqData("some value", "Invalid Authorization Scheme")]
    [InlineAutoMoqData("value", "Invalid Authorization Scheme")]
    [InlineAutoMoqData("basic", "Invalid Authorization Scheme")]
    [InlineAutoMoqData("Basic", "Invalid Authorization Scheme")]
    [InlineAutoMoqData("Basic auth", "Invalid Authorization Header")]
    public async Task HandleAuthenticateAsync_ReturnsFail_WhenHeaderIsSuppliedButInvalid(
        string headerVal,
        string expectedFailReason,
        [Frozen] Mock<IComputeHash> computeHashMock,
        [Frozen] Mock<IVerifyUserQuery> mockVerifyUserQuery,
        [Frozen] Mock<IOptionsMonitor<AuthenticationSchemeOptions>> mockOptionsMonitor)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Headers[AuthenticationConstants.AuthorizationHeader] = headerVal;

        mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(new AuthenticationSchemeOptions());

        var loggerFactory = NullLoggerFactory.Instance;
        var encoder = System.Text.Encodings.Web.UrlEncoder.Default;

        var sut = new BasicAuthenticationHandler(
            mockOptionsMonitor.Object,
            loggerFactory,
            encoder,
            computeHashMock.Object,
            mockVerifyUserQuery.Object);

        var scheme = new AuthenticationScheme(AuthenticationConstants.AuthenticationType, AuthenticationConstants.AuthenticationType, typeof(BasicAuthenticationHandler));

        await sut.InitializeAsync(scheme, context);

        // Act
        var result = await sut.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Should().NotBeNull();
        result.Failure.Message.Should().Be(expectedFailReason);
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task HandleAuthenticateAsync_ReturnsFail_WhenHeaderIsSuppliedButEmptyString(
        [Frozen] Mock<IComputeHash> computeHashMock,
        [Frozen] Mock<IVerifyUserQuery> mockVerifyUserQuery,
        [Frozen] Mock<IOptionsMonitor<AuthenticationSchemeOptions>> mockOptionsMonitor)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Headers[AuthenticationConstants.AuthorizationHeader] = string.Empty;

        mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(new AuthenticationSchemeOptions());

        var loggerFactory = NullLoggerFactory.Instance;
        var encoder = System.Text.Encodings.Web.UrlEncoder.Default;

        var sut = new BasicAuthenticationHandler(
            mockOptionsMonitor.Object,
            loggerFactory,
            encoder,
            computeHashMock.Object,
            mockVerifyUserQuery.Object);

        var scheme = new AuthenticationScheme(AuthenticationConstants.AuthenticationType, AuthenticationConstants.AuthenticationType, typeof(BasicAuthenticationHandler));

        await sut.InitializeAsync(scheme, context);

        // Act
        var result = await sut.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Should().NotBeNull();
        result.Failure.Message.Should().Be("Invalid Authorization Header");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task HandleAuthenticateAsync_ReturnsFail_WhenHeaderIsSuppliedButCannotAuthenticate(
        [Frozen] Mock<IComputeHash> computeHashMock,
        [Frozen] Mock<IVerifyUserQuery> mockVerifyUserQuery,
        [Frozen] Mock<IOptionsMonitor<AuthenticationSchemeOptions>> mockOptionsMonitor,
        string userName,
        string password)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Headers[AuthenticationConstants.AuthorizationHeader] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"))}";

        mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(new AuthenticationSchemeOptions());

        mockVerifyUserQuery.Setup(x => x.VerifyUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var loggerFactory = NullLoggerFactory.Instance;
        var encoder = System.Text.Encodings.Web.UrlEncoder.Default;

        var sut = new BasicAuthenticationHandler(
            mockOptionsMonitor.Object,
            loggerFactory,
            encoder,
            computeHashMock.Object,
            mockVerifyUserQuery.Object);

        var scheme = new AuthenticationScheme(AuthenticationConstants.AuthenticationType, AuthenticationConstants.AuthenticationType, typeof(BasicAuthenticationHandler));

        await sut.InitializeAsync(scheme, context);

        // Act
        var result = await sut.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Failure.Should().NotBeNull();
        result.Failure.Message.Should().Be("Invalid Username or Password");
    }

    [Theory]
    [InlineAutoMoqData]
    public async Task HandleAuthenticateAsync_ReturnsSuccess(
        [Frozen] Mock<IComputeHash> computeHashMock,
        [Frozen] Mock<IVerifyUserQuery> mockVerifyUserQuery,
        [Frozen] Mock<IOptionsMonitor<AuthenticationSchemeOptions>> mockOptionsMonitor,
        string userName,
        string password)
    {
        // Arrange
        var context = new DefaultHttpContext();

        context.Request.Headers[AuthenticationConstants.AuthorizationHeader] = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userName}:{password}"))}";

        mockOptionsMonitor.Setup(x => x.CurrentValue).Returns(new AuthenticationSchemeOptions());

        mockVerifyUserQuery.Setup(x => x.VerifyUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var loggerFactory = NullLoggerFactory.Instance;
        var encoder = System.Text.Encodings.Web.UrlEncoder.Default;

        var sut = new BasicAuthenticationHandler(
            mockOptionsMonitor.Object,
            loggerFactory,
            encoder,
            computeHashMock.Object,
            mockVerifyUserQuery.Object);

        var scheme = new AuthenticationScheme(AuthenticationConstants.AuthenticationType, AuthenticationConstants.AuthenticationType, typeof(BasicAuthenticationHandler));

        await sut.InitializeAsync(scheme, context);

        // Act
        var result = await sut.AuthenticateAsync();

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Ticket.Should().NotBeNull();
        result.Ticket.AuthenticationScheme.Should().Be(AuthenticationConstants.AuthorizationScheme);
        result.Ticket.Principal.Should().NotBeNull();
        result.Ticket.Principal.Identities.Should().HaveCount(1);
        result.Ticket.Principal.Identity.Should().NotBeNull();
        result.Ticket.Principal.Identity.Should().BeOfType<ClaimsIdentity>();

        var asClaimsIdentity = result.Ticket.Principal.Identity as ClaimsIdentity;
        asClaimsIdentity.Should().NotBeNull();
        asClaimsIdentity.AuthenticationType.Should().Be(AuthenticationConstants.AuthenticationType);
        asClaimsIdentity.Claims.Should().HaveCount(1);
        asClaimsIdentity.Claims.First().Type.Should().Be(ClaimTypes.Name);
        asClaimsIdentity.Claims.First().Value.Should().Be(userName);
    }
}