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
            .WithName(EndpointNames.WhoAmI)
            .RequireAuthorization(AuthenticationConstants.NoAuthorizationPolicy);

        var inventoryGroup = group.MapGroup("/inventory");

        inventoryGroup.MapGet("/products", HttpHandler.HandleAsync<GetProductsRequest, ICollection<GetProductsResponse>>)
            .WithName(EndpointNames.GetProducts)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapGet("/product/{ProductId}", HttpHandler.HandleAsync<GetProductRequest, GetProductResponse>)
            .WithName(EndpointNames.GetProduct)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapPost("/product", HttpHandler.HandleAsync<CreateProductRequest, CreateProductResponse>)
            .WithName(EndpointNames.CreateProduct)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        inventoryGroup.MapGet("/customers", HttpHandler.HandleAsync<GetCustomersRequest, ICollection<GetCustomersResponse>>)
            .WithName(EndpointNames.GetCustomers)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapGet("/customer/{customerId}", HttpHandler.HandleAsync<GetCustomerRequest, GetCustomerResponse>)
            .WithName(EndpointNames.GetCustomer)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapPost("/customer", HttpHandler.HandleAsync<CreateCustomerRequest, CreateCustomerResponse>)
            .WithName(EndpointNames.CreateCustomer)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        inventoryGroup.MapGet("/purchase-orders", HttpHandler.HandleAsync<GetPurchaseOrdersRequest, ICollection<GetPurchaseOrdersResponse>>)
            .WithName(EndpointNames.GetPurchaseOrders)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapGet("/purchase-order/{purchaseOrderId}", HttpHandler.HandleAsync<GetPurchaseOrderRequest, GetPurchaseOrderResponse>)
            .WithName(EndpointNames.GetPurchaseOrder)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);
        inventoryGroup.MapPost("/purchase-order", HttpHandler.HandleAsync<CreatePurchaseOrderRequest, CreatePurchaseOrderResponse>)
            .WithName(EndpointNames.CreatePurchaseOrder)
            .RequireAuthorization(AuthenticationConstants.WritingAuthorizationPolicy);

        inventoryGroup.MapGet("/countries", HttpHandler.HandleAsync<GetCountriesRequest, ICollection<GetCountriesResponse>>)
            .WithName(EndpointNames.GetCountries)
            .RequireAuthorization(AuthenticationConstants.ReadingAuthorizationPolicy);

        return routeBuilder;
    }
}
