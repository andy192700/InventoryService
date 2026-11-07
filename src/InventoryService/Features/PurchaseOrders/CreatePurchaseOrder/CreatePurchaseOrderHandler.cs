using FluentResults;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;
using InventoryService.Infrastructure;
using MediatR;
using Messaging;

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

    private readonly ISNSProducer<CreatePurchaseOrderResponse, CreatePurchaseOrderTopic> _producer;

    private readonly ILogger<CreatePurchaseOrderHandler> _logger;

    public CreatePurchaseOrderHandler(
        ICreatePurchaseOrderDBParametersMapper parametersMapper,
        ICreatePurchaseOrderDBCommand dbCommand,
        ICreatePurchaseOrderResponseMapper responseMapper,
        ILocationBuilder locationBuilder,
        ISNSProducer<CreatePurchaseOrderResponse, CreatePurchaseOrderTopic> producer,
        ILogger<CreatePurchaseOrderHandler> logger)
    {
        _parametersMapper = parametersMapper;
        _dbCommand = dbCommand;
        _responseMapper = responseMapper;
        _locationBuilder = locationBuilder;
        _producer = producer;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<CreatePurchaseOrderResponse>> Handle(CreatePurchaseOrderRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _parametersMapper.Map(request);

        var dbResponse = await _dbCommand.CreatePurchaseOrderAsync(dbParams, cancellationToken);

        _logger.LogInformation($"Created Purhase Order with PurchaseOrderId = {dbResponse.PurchaseOrderId}");

        var response = _responseMapper.Map(dbResponse);

        await _producer.ProduceAsync(response, cancellationToken);

        var location = _locationBuilder.Build(
            EndpointNames.GetPurchaseOrder,
            new GetPurchaseOrderRequest(response.PurchaseOrderId.ToString()));

        return RequestHandlerResponses.Created(response, location);
    }
}
