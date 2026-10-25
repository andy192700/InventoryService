using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Countries.GetCountries;

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

    public async Task<Result<ICollection<GetCountriesResponse>>> Handle(GetCountriesRequest request, CancellationToken cancellationToken)
    {
        var dbResponse = await _dbQuery.GetCountriesAsync(cancellationToken);

        var response = _mapper.Map(dbResponse);

        return HttpResponses.Ok(response);
    }
}
