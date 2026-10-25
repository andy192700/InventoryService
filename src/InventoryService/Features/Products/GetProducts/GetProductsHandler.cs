using FluentResults;
using InventoryService.Infrastructure;
using MediatR;

namespace InventoryService.Features.Products.GetProducts;

/// <summary>
/// Handles a request for creating a product, where the request is a <see cref="GetProductsRequest"/>.
/// </summary>
public class GetProductsHandler : IRequestHandler<GetProductsRequest, Result<ICollection<GetProductsResponse>>>
{
    private readonly IGetProductsDBQuery _getProductsDBQuery;
    private readonly IGetProductsResponseMapper _getProductsResponseMapper;

    private readonly IGetProductsDBParametersMapper _getProductsDBParametersMapper;

    public GetProductsHandler(
        IGetProductsDBQuery getProductsDBQuery,
        IGetProductsResponseMapper getProductsResponseMapper,
        IGetProductsDBParametersMapper getProductsDBParametersMapper)
    {
        _getProductsDBQuery = getProductsDBQuery;
        _getProductsResponseMapper = getProductsResponseMapper;
        _getProductsDBParametersMapper = getProductsDBParametersMapper;
    }

    /// <inheritdoc/>
    public async Task<Result<ICollection<GetProductsResponse>>> Handle(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var dbParams = _getProductsDBParametersMapper.Map(request);

        var dbResponse = await _getProductsDBQuery.GetProductsAsync(dbParams, cancellationToken);

        return HttpResponses.Ok(_getProductsResponseMapper.Map(dbResponse));
    }
}
