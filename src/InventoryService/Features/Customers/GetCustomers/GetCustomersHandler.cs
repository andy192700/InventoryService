using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Customers.GetCustomers;

/// <summary>
/// Handles a request for retrieving a list ofcustomer, where the request is a <see cref="GetCustomersRequest"/>.
/// </summary>
public class GetCustomersHandler : IRequestHandler<GetCustomersRequest, Result<ICollection<GetCustomersResponse>>>
{
    private readonly IGetCustomersDBQuery _getCustomersDBQuery;

    private readonly IGetCustomersResponseMapper _getCustomersResponseMapper;

    private readonly IGetCustomersDBParametersMapper _getCustomersDBParametersMapper;

    public GetCustomersHandler(
        IGetCustomersDBQuery getCustomersDBQuery, 
        IGetCustomersResponseMapper getCustomersResponseMapper,
        IGetCustomersDBParametersMapper getCustomersDBParametersMapper)
    {
        _getCustomersDBQuery = getCustomersDBQuery;
        _getCustomersResponseMapper = getCustomersResponseMapper;
        _getCustomersDBParametersMapper = getCustomersDBParametersMapper;
    }

    /// <inheritdoc/>
    public async Task<Result<ICollection<GetCustomersResponse>>> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _getCustomersDBParametersMapper.Map(request);

        var dbResponse = await _getCustomersDBQuery.GetCustomersAsync(dbParams, cancellationToken);

        var response = _getCustomersResponseMapper.Map(dbResponse);

        return HttpResponses.Ok(response);
    }
}
