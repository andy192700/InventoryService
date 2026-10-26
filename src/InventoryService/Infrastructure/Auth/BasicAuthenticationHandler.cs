using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace InventoryService.Infrastructure.Auth;

/// <summary>
/// Handles authenticaion to this application, returning 401 if authentication is not satisfied.
/// </summary>
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private string? _failureMessage;

    private readonly IComputeHash _computeHash;

    private readonly IVerifyUserQuery _verifyUserQuery;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IComputeHash computeHash,
        IVerifyUserQuery verifyUserQuery)
        : base(options, logger, encoder) 
    {
        _computeHash = computeHash;
        _verifyUserQuery = verifyUserQuery;
    }

    /// <inheritdoc/>
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(AuthenticationConstants.AuthorizationHeader, out var authHeader))
        {
            _failureMessage = "Missing Authorization Header";
            
            return AuthenticateResult.Fail(_failureMessage);
        }

        try
        {
            var header = AuthenticationHeaderValue.Parse(authHeader!);

            if (!string.Equals(header.Scheme, AuthenticationConstants.AuthenticationType, StringComparison.OrdinalIgnoreCase)
             || string.IsNullOrWhiteSpace(header.Parameter))
            {
                _failureMessage = "Invalid Authorization Scheme";

                return AuthenticateResult.Fail(_failureMessage);
            }
            
            var credentialBytes = Convert.FromBase64String(header.Parameter);
            
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

            if (credentials.Length != 2)
            {
                _failureMessage = "Invalid Authorization Header";

                return AuthenticateResult.Fail(_failureMessage);
            }

            var username = credentials[0];

            var passwordHash = _computeHash.Compute(credentials[1]);

            var validCredentials = await _verifyUserQuery.VerifyUserAsync(username, passwordHash, CancellationToken.None);

            if (!validCredentials)
            {
                _failureMessage = "Invalid Username or Password";

                return AuthenticateResult.Fail(_failureMessage);
            }
            
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            
            var principal = new ClaimsPrincipal(identity);
            
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            
            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            _failureMessage = "Invalid Authorization Header";

            return AuthenticateResult.Fail(_failureMessage);
        }
    }

    /// <inheritdoc/>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/problem+json";

        var problemDetails = new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            title = "Unauthorized",
            detail = _failureMessage ?? "Valid credentials are required to access this resource.",
            status = StatusCodes.Status401Unauthorized
        };

        await Response.WriteAsJsonAsync(problemDetails);
    }
}
