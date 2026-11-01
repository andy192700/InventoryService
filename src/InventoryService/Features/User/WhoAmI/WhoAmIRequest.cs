using FluentResults;
using MediatR;

namespace InventoryService.Features.User.WhoAmI;

/// <summary>
/// A WhoAmI request.
/// </summary>
public record WhoAmIRequest() : IRequest<Result<WhoAmIResponse>>;
