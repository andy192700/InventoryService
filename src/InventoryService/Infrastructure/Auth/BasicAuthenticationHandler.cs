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
    private readonly IComputeHash _computeHash;

    private readonly IGetUserPermissionsDBQuery _verifyUserQuery;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IComputeHash computeHash,
        IGetUserPermissionsDBQuery verifyUserQuery)
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
            return Fail("Missing Authorization Header");
        }

        try
        {
            var header = AuthenticationHeaderValue.Parse(authHeader!);

            if (!string.Equals(header.Scheme, AuthenticationConstants.AuthenticationType, StringComparison.OrdinalIgnoreCase)
             || string.IsNullOrWhiteSpace(header.Parameter))
            {
                return Fail("Invalid Authorization Scheme");
            }
            
            var credentialBytes = Convert.FromBase64String(header.Parameter);
            
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);

            if (credentials.Length != 2)
            {
                return Fail("Invalid Authorization Header");
            }

            var username = credentials[0];

            var passwordHash = _computeHash.Compute(credentials[1]);

            var validCredentials = await _verifyUserQuery.GetPermissionsAsync(username, passwordHash, Context.RequestAborted);

            if (string.IsNullOrWhiteSpace(validCredentials.UserName))
            {
                return Fail("Invalid Username or Password");
            }
            
            var claims = new[] { new Claim(ClaimTypes.Name, username) };
            
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            
            var principal = new ClaimsPrincipal(identity);
            
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            
            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return Fail("Invalid Authorization Header");
        }
    }

    private AuthenticateResult Fail(string reason)
    {
        return AuthenticateResult.Fail(new AuthenticationFailureException(reason));
    }
}
