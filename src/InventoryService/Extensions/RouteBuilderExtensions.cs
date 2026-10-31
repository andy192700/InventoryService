using InventoryService.Features.Countries.GetCountries;
using InventoryService.Features.Customers.CreateCustomer;
using InventoryService.Features.Customers.GetCustomer;
using InventoryService.Features.Customers.GetCustomers;
using InventoryService.Features.Products.CreateProduct;
using InventoryService.Features.Products.GetProduct;
using InventoryService.Features.Products.GetProducts;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;
using InventoryService.Infrastructure;
using InventoryService.Infrastructure.Auth;
using InventoryService.Infrastructure.Health;

namespace InventoryService.Extensions;

public static class RouteBuilderExtensions
{
    /// <summary>
    /// Register the application's endpoints.
    /// </summary>
    /// <param name="routeBuilder">The application builder, expressed as a <see cref="IEndpointRouteBuilder"/></param>
    /// <returns>The builder.</returns>
    public static IEndpointRouteBuilder RegisterEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapHealthChecks("/health", new DatabaseHealthCheckOptions());

        var group = routeBuilder.MapGroup("/v1/inventory");

        group.MapGet("/products", HttpHandler.HandleAsync<GetProductsRequest, ICollection<GetProductsResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        group.MapGet("/product", HttpHandler.HandleAsync<GetProductRequest, GetProductResponse>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        group.MapPost("/product", HttpHandler.HandleAsync<CreateProductRequest, CreateProductResponse>)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        group.MapGet("/customers", HttpHandler.HandleAsync<GetCustomersRequest, ICollection<GetCustomersResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        group.MapGet("/customer", HttpHandler.HandleAsync<GetCustomerRequest, GetCustomerResponse>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        group.MapPost("/customer", HttpHandler.HandleAsync<CreateCustomerRequest, CreateCustomerResponse>)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        group.MapGet("/purchase-orders", HttpHandler.HandleAsync<GetPurchaseOrdersRequest, ICollection<GetPurchaseOrdersResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        group.MapGet("/purchase-order", HttpHandler.HandleAsync<GetPurchaseOrderRequest, GetPurchaseOrderResponse>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        group.MapPost("/purchase-order", HttpHandler.HandleAsync<CreatePurchaseOrderRequest, CreatePurchaseOrderResponse>)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        group.MapGet("/countries", HttpHandler.HandleAsync<GetCountriesRequest, ICollection<GetCountriesResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);

        return routeBuilder;
    }
}
