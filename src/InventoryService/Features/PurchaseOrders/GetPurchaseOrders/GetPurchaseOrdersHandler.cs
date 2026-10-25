using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrders;

/// <summary>
/// Handles a request for creating a product, where the request is a <see cref="GetPurchaseOrdersRequest"/>.
/// </summary>
public class GetPurchaseOrdersHandler : IRequestHandler<GetPurchaseOrdersRequest, Result<ICollection<GetPurchaseOrdersResponse>>>
{
    private readonly IGetPurchaseOrdersDBQuery _dbQuery;

    private readonly IGetPurchaseOrdersResponseMapper _mapper;

    private readonly IGetPurchaseOrdersDBParametersMapper _parametersMapper;

    public GetPurchaseOrdersHandler(
        IGetPurchaseOrdersDBQuery dbQuery,
        IGetPurchaseOrdersResponseMapper mapper,
        IGetPurchaseOrdersDBParametersMapper parametersMapper)
    {
        _dbQuery = dbQuery;
        _mapper = mapper;
        _parametersMapper = parametersMapper;
    }

    /// <inheritdoc/>
    public async Task<Result<ICollection<GetPurchaseOrdersResponse>>> Handle(GetPurchaseOrdersRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _parametersMapper.Map(request);

        var dbResponse = await _dbQuery.GetPurchaseOrdersAsync(dbParams, cancellationToken);

        var response = _mapper.Map(dbResponse);

        return HttpResponses.Ok(response);
    }
}
