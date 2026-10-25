using FluentResults;
using MediatR;

namespace InventoryService.Features.Countries.GetCountries;

public record GetCountriesRequest() : IRequest<Result<ICollection<GetCountriesResponse>>>;
