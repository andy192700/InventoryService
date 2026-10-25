using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.PurchaseOrders.GetPurchaseOrder;

public class GetPurchaseOrderHandler : IRequestHandler<GetPurchaseOrderRequest, Result<GetPurchaseOrderResponse>>
{
    private readonly IGetPurchaseOrderDBParametersMapper _parametersMapper;

    private readonly IGetPurchaseOrderDBQuery _dbQuery;

    private readonly IGetPurchaseOrderResponseMapper _responseMapper;

    public GetPurchaseOrderHandler(
        IGetPurchaseOrderDBParametersMapper parametersMapper, 
        IGetPurchaseOrderDBQuery dbQuery, 
        IGetPurchaseOrderResponseMapper responseMapper)
    {
        _parametersMapper = parametersMapper;
        _dbQuery = dbQuery;
        _responseMapper = responseMapper;
    }

    public async Task<Result<GetPurchaseOrderResponse>> Handle(GetPurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _parametersMapper.Map(request);

        var dbResponse = await _dbQuery.GetPurchaseOrderAsync(dbParams, cancellationToken);

        if (dbResponse.header is null)
        {
            return Result.Fail(new NotFoundError("Purchase Order not found"));
        }

        var response = _responseMapper.Map(dbResponse);

        return HttpResponses.Ok(response);
    }
}
