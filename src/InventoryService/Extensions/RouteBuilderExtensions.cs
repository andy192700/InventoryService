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
using InventoryService.Features.User.WhoAmI;
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

        var group = routeBuilder.MapGroup("/v1");

        var userGroup = group.MapGroup("/user");

        userGroup.MapGet("/whoami", HttpHandler.HandleAsync<WhoAmIRequest, WhoAmIResponse>)
            .RequireAuthorization(AuthenticationConstants.NoAuthorizationPolicy);

        var inventoryGroup = group.MapGroup("/inventory");

        inventoryGroup.MapGet("/products", HttpHandler.HandleAsync<GetProductsRequest, ICollection<GetProductsResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapGet("/product", HttpHandler.HandleAsync<GetProductRequest, GetProductResponse>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapPost("/product", HttpHandler.HandleAsync<CreateProductRequest, CreateProductResponse>)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        inventoryGroup.MapGet("/customers", HttpHandler.HandleAsync<GetCustomersRequest, ICollection<GetCustomersResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapGet("/customer", HttpHandler.HandleAsync<GetCustomerRequest, GetCustomerResponse>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapPost("/customer", HttpHandler.HandleAsync<CreateCustomerRequest, CreateCustomerResponse>)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        inventoryGroup.MapGet("/purchase-orders", HttpHandler.HandleAsync<GetPurchaseOrdersRequest, ICollection<GetPurchaseOrdersResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapGet("/purchase-order", HttpHandler.HandleAsync<GetPurchaseOrderRequest, GetPurchaseOrderResponse>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapPost("/purchase-order", HttpHandler.HandleAsync<CreatePurchaseOrderRequest, CreatePurchaseOrderResponse>)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        inventoryGroup.MapGet("/countries", HttpHandler.HandleAsync<GetCountriesRequest, ICollection<GetCountriesResponse>>)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);

        return routeBuilder;
    }
}
