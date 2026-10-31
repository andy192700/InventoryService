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
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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
        routeBuilder.MapHealthChecks("/health", new DatabaseHealthCheckOptions())
            .RequireAuthorization(AuthenticationConstants.AuthorizationPolicy);

        var group = routeBuilder
            .MapGroup("/v1/inventory")
            .RequireAuthorization(AuthenticationConstants.AuthorizationPolicy);

        group.MapGet("/products", HttpHandler.HandleAsync<GetProductsRequest, ICollection<GetProductsResponse>>);
        group.MapGet("/product", HttpHandler.HandleAsync<GetProductRequest, GetProductResponse>);
        group.MapPost("/product", HttpHandler.HandleAsync<CreateProductRequest, CreateProductResponse>);

        group.MapGet("/customers", HttpHandler.HandleAsync<GetCustomersRequest, ICollection<GetCustomersResponse>>);
        group.MapGet("/customer", HttpHandler.HandleAsync<GetCustomerRequest, GetCustomerResponse>);
        group.MapPost("/customer", HttpHandler.HandleAsync<CreateCustomerRequest, CreateCustomerResponse>);

        group.MapGet("/purchase-orders", HttpHandler.HandleAsync<GetPurchaseOrdersRequest, ICollection<GetPurchaseOrdersResponse>>);
        group.MapGet("/purchase-order", HttpHandler.HandleAsync<GetPurchaseOrderRequest, GetPurchaseOrderResponse>);
        group.MapPost("/purchase-order", HttpHandler.HandleAsync<CreatePurchaseOrderRequest, CreatePurchaseOrderResponse>);

        group.MapGet("/countries", HttpHandler.HandleAsync<GetCountriesRequest, ICollection<GetCountriesResponse>>);

        return routeBuilder;
    }
}
