using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Products.GetProduct;

/// <summary>
/// Handles a request for creating a product, where the request is a <see cref="GetProductRequest"/>.
/// </summary>
public class GetProductHandler : IRequestHandler<GetProductRequest, Result<GetProductResponse>>
{
    private readonly IGetProductDBParametersMapper _dbParamsMapper;
    private readonly IGetProductDBQuery _getProductDBQuery;
    private readonly IGetProductResponseMapper _getProductResponseMapper;

    public GetProductHandler(
        IGetProductDBParametersMapper dbParamsMapper,
        IGetProductDBQuery getProductsDBQuery,
        IGetProductResponseMapper getProductsResponseMapper)
    {
        _dbParamsMapper = dbParamsMapper;
        _getProductDBQuery = getProductsDBQuery;
        _getProductResponseMapper = getProductsResponseMapper;
    }

    /// <inheritdoc/>
    public async Task<Result<GetProductResponse>> Handle(GetProductRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _dbParamsMapper.Map(request);

        var dbResult = await _getProductDBQuery.GetProductAsync(dbParams, cancellationToken);

        if (dbResult.Product is null)
        {
            return HttpResponses.Notfound("Product not found");
        }

        var response = _getProductResponseMapper.Map(dbResult);

        return HttpResponses.Ok(response);
    }
}
