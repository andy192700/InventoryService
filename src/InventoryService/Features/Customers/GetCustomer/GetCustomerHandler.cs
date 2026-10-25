using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Customers.GetCustomer;

/// <summary>
/// Handles a request for retrieving a customer, where the request is a <see cref="GetCustomerRequest"/>.
/// </summary>
public class GetCustomerHandler : IRequestHandler<GetCustomerRequest, Result<GetCustomerResponse>>
{
    private readonly IGetCustomerDBParametersMapper _parametersMapper;

    private readonly IGetCustomerDBQuery _dbQuery;

    private readonly IGetCustomerResponseMapper _responseMapper;

    public GetCustomerHandler(
        IGetCustomerDBParametersMapper parametersMapper, 
        IGetCustomerDBQuery dbQuery, 
        IGetCustomerResponseMapper responseMapper)
    {
        _parametersMapper = parametersMapper;
        _dbQuery = dbQuery;
        _responseMapper = responseMapper;
    }

    /// <inheritdoc/>
    public async Task<Result<GetCustomerResponse>> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _parametersMapper.Map(request);

        var dbResponse = await _dbQuery.GetCustomerAsync(dbParams, cancellationToken);

        if (dbResponse is null)
        { 
            return HttpResponses.Notfound("Customer not found");
        }

        var response = _responseMapper.Map(dbResponse);

        return HttpResponses.Ok(response);
    }
}
