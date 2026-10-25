using FluentResults;
using InventoryService.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;

namespace InventoryService.Features.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductRequest, Result<CreateProductResponse>>
{
    private readonly ICreateProductDBParametersMapper _dbParametersMapper;
    
    private readonly ICreateProductResponseMapper _responseMapper;

    private readonly ICreateProductDBCommand _createProductDBCommand;

    private readonly ILocationBuilder _locationBuilder;

    public CreateProductHandler(
        ICreateProductDBParametersMapper dbParametersMapper,
        ICreateProductResponseMapper responseMapper,
        ICreateProductDBCommand createProductDBCommand,
        ILocationBuilder locationBuilder)
    {
        _dbParametersMapper = dbParametersMapper;
        _responseMapper = responseMapper;
        _createProductDBCommand = createProductDBCommand;
        _locationBuilder = locationBuilder;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _dbParametersMapper.Map(request);

        var dbResponse = await _createProductDBCommand.CreateProductAsync(dbParams, cancellationToken);

        var response = _responseMapper.Map(dbResponse);

        var location = _locationBuilder.Build(
            "/v1/inventory/product",
            QueryString.Create("productId", response.ProductId.ToString()));

        return HttpResponses.Created(response, location);
    }
}
