using FluentResults;
using InventoryService.Infrastructure;
using MediatR;
using System.Security.Claims;

namespace InventoryService.Features.User.WhoAmI;

/// <summary>
/// Handle for returning authenticated user details such as username and roles.
/// </summary>
public class WhoAmIHandler : IRequestHandler<WhoAmIRequest, Result<WhoAmIResponse>>
{
    private readonly IHttpContextAccessor _contextAccessor;

    public WhoAmIHandler(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    /// <inheritdoc/>
    public Task<Result<WhoAmIResponse>> Handle(WhoAmIRequest request, CancellationToken cancellationToken)
    {
        var context = _contextAccessor.HttpContext;

        if (context is null)
        {
            return Task.FromResult((Result<WhoAmIResponse>)RequestHandlerResponses.UnauthorizedError());
        }

        var identity = context.User.Identity as ClaimsIdentity;

        if (identity is null || !identity.IsAuthenticated)
        {
            return Task.FromResult((Result<WhoAmIResponse>)RequestHandlerResponses.UnauthorizedError());
        }

        var userName = identity.Name;

        if (string.IsNullOrWhiteSpace(userName))
        {
            return Task.FromResult((Result<WhoAmIResponse>)RequestHandlerResponses.UnauthorizedError());
        }

        var roles = identity.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

        return Task.FromResult((Result<WhoAmIResponse>)RequestHandlerResponses.Ok(new WhoAmIResponse
        {
            UserName = userName,
            Roles = roles
        }));
    }
}
