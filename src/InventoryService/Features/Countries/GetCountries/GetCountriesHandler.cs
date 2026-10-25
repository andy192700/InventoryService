using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Countries.GetCountries;

/// <summary>
/// Handles a request for retrieving Countries, where the request is a <see cref="GetCountriesRequest"/>.
/// </summary>
public class GetCountriesHandler : IRequestHandler<GetCountriesRequest, Result<ICollection<GetCountriesResponse>>>
{
    private IGetCountriesDBQuery _dbQuery;

    private IGetCountriesResponseMapper _mapper;

    public GetCountriesHandler(
        IGetCountriesDBQuery dbQuery,
        IGetCountriesResponseMapper mapper)
    {
        _dbQuery = dbQuery;
        _mapper = mapper;
    }

    /// <inheritdoc/>
    public async Task<Result<ICollection<GetCountriesResponse>>> Handle(GetCountriesRequest request, CancellationToken cancellationToken)
    {
        var dbResponse = await _dbQuery.GetCountriesAsync(cancellationToken);

        var response = _mapper.Map(dbResponse);

        return HttpResponses.Ok(response);
    }
}
