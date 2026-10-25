using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Customers.CreateCustomer;

/// <summary>
/// Handles a request for retrieving Countries, where the request is a <see cref="CreateCustomerRequest"/>.
/// </summary>
public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, Result<CreateCustomerResponse>>
{
    private readonly ICreateCustomerDBCommand _dBCommand;

    private readonly ICreateCustomerResponseMapper _responseMapper;

    private readonly ICreateCustomerDBParametersMapper _parametersMapper;

    private readonly ILocationBuilder _locationBuilder;

    public CreateCustomerHandler(
        ICreateCustomerDBCommand dBCommand, 
        ICreateCustomerResponseMapper responseMapper, 
        ICreateCustomerDBParametersMapper parametersMapper,
        ILocationBuilder locationBuilder)
    {
        _dBCommand = dBCommand;
        _responseMapper = responseMapper;
        _parametersMapper = parametersMapper;
        _locationBuilder = locationBuilder;
    }

    /// <inheritdoc/>
    public async Task<Result<CreateCustomerResponse>> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _parametersMapper.Map(request);

        var dbResponse = await _dBCommand.CreateCustomerAsync(dbParams, cancellationToken);

        var response = _responseMapper.Map(dbResponse);

        var location = _locationBuilder.Build(
            "/v1/inventory/customer",
            QueryString.Create("customerId", response.CustomerId.ToString()));

        return HttpResponses.Created(response, location);
    }
}
