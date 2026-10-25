using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;

/// <summary>
/// Handles a request for creating a product, where the request is a <see cref="CreatePurchaseOrderRequest"/>.
/// </summary>
public class CreatePurchaseOrderHandler : IRequestHandler<CreatePurchaseOrderRequest, Result<CreatePurchaseOrderResponse>>
{
    private readonly ICreatePurchaseOrderDBParametersMapper _parametersMapper;

    private readonly ICreatePurchaseOrderDBCommand _dbCommand;

    private readonly ICreatePurchaseOrderResponseMapper _responseMapper;

    private readonly ILocationBuilder _locationBuilder;

    public CreatePurchaseOrderHandler(
        ICreatePurchaseOrderDBParametersMapper parametersMapper,
        ICreatePurchaseOrderDBCommand dbCommand,
        ICreatePurchaseOrderResponseMapper responseMapper,
        ILocationBuilder locationBuilder)
    {
        _parametersMapper = parametersMapper;
        _dbCommand = dbCommand;
        _responseMapper = responseMapper;
        _locationBuilder = locationBuilder;
    }

    /// <inheritdoc/>
    public async Task<Result<CreatePurchaseOrderResponse>> Handle(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _parametersMapper.Map(request);

        var dbResponse = await _dbCommand.CreatePurchaseOrderAsync(dbParams, cancellationToken);
        
        var response = _responseMapper.Map(dbResponse);

        var location = _locationBuilder.Build(
            "/v1/inventory/purchase-order",
            QueryString.Create("purchaseOrderId", response.PurchaseOrderId.ToString()));

        return HttpResponses.Created(response, location);
    }
}
