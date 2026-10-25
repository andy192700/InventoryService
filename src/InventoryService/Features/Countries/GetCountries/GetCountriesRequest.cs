using FluentResults;
using MediatR;

namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// A request class for retrieving Countries.
/// </summary>
public record GetCountriesRequest() : IRequest<Result<ICollection<GetCountriesResponse>>>;
