using FluentValidation;
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

namespace InventoryService;

public static class ServiceRegistrations
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        // infra
        services.AddScoped<ISQLConnectionProvider, SQLConnectionProvider>();
        services.AddScoped<ILocationBuilder, LocationBuilder>();

        // Get Products
        services.AddScoped<IGetProductsDBQuery, GetProductsDBQuery>();
        services.AddScoped<IGetProductsResponseMapper, GetProductsResponseMapper>();
        services.AddScoped<IValidator<GetProductsRequest>, GetProductsRequestValidator>();
        services.AddScoped<IGetProductsDBParametersMapper, GetProductsDBParametersMapper>();

        // Get Product
        services.AddScoped<IGetProductDBParametersMapper, GetProductDBParametersMapper>();
        services.AddScoped<IValidator<GetProductRequest>, GetProductRequestValidator>();
        services.AddScoped<IGetProductDBQuery, GetProductDBQuery>();
        services.AddScoped<IGetProductResponseMapper, GetProductResponseMapper>();

        // Create Product
        services.AddScoped<ICreateProductDBParametersMapper, CreateProductDBParametersMapper>();
        services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddScoped<ICreateProductDBCommand, CreateProductDBCommand>();
        services.AddScoped<ICreateProductResponseMapper, CreateProductResponseMapper>();

        // Get Customers
        services.AddScoped<IGetCustomersDBQuery, GetCustomersDBQuery>();
        services.AddScoped<IGetCustomersResponseMapper, GetCustomersResponseMapper>();
        services.AddScoped<IGetCustomersDBParametersMapper, GetCustomersDBParametersMapper>();
        services.AddScoped<IValidator<GetCustomersRequest>,  GetCustomersRequestValidator>();

        // Get Customer
        services.AddScoped<IGetCustomerDBParametersMapper, GetCustomerDBParametersMapper>();
        services.AddScoped<IValidator<GetCustomerRequest>, GetCustomerRequestValidator>();
        services.AddScoped<IGetCustomerDBQuery, GetCustomerDBQuery>();
        services.AddScoped<IGetCustomerResponseMapper, GetCustomerResponseMapper>();

        // Create Customer
        services.AddScoped<ICreateCustomerDBParametersMapper, CreateCustomerDBParametersMapper>();
        services.AddScoped<IValidator<CreateCustomerRequest>, CreateCustomerRequestValidator>();
        services.AddScoped<ICreateCustomerDBCommand, CreateCustomerDBCommand>();
        services.AddScoped<ICreateCustomerResponseMapper, CreateCustomerResponseMapper>();

        // Get Purchase Orders
        services.AddScoped<IGetPurchaseOrdersResponseMapper, GetPurchaseOrdersResponseMapper>();
        services.AddScoped<IGetPurchaseOrdersDBQuery, GetPurchaseOrdersDBQuery>();
        services.AddScoped<IGetPurchaseOrdersDBParametersMapper, GetPurchaseOrdersDBParametersMapper>();
        services.AddScoped<IValidator<GetPurchaseOrdersRequest>, GetPurchaseOrdersRequestValidator>();

        // Get Purchase Order
        services.AddScoped<IGetPurchaseOrderDBParametersMapper, GetPurchaseOrderDBParametersMapper>();
        services.AddScoped<IValidator<GetPurchaseOrderRequest>, GetPurchaseOrderRequestValidator>();
        services.AddScoped<IGetPurchaseOrderDBQuery, GetPurchaseOrderDBQuery>();
        services.AddScoped<IGetPurchaseOrderResponseMapper, GetPurchaseOrderResponseMapper>();

        // Create Purchase Order
        services.AddScoped<ICreatePurchaseOrderDBParametersMapper, CreatePurchaseOrderDBParametersMapper>();
        services.AddScoped<IValidator<CreatePurchaseOrderRequest>, CreatePurchaseOrderRequestValidator>();
        services.AddScoped<ICreatePurchaseOrderDBCommand, CreatePurchaseOrderDBCommand>();
        services.AddScoped<ICreatePurchaseOrderResponseMapper,  CreatePurchaseOrderResponseMapper>();

        // Get Countries
        services.AddScoped<IGetCountriesDBQuery, GetCountriesDBQuery>();
        services.AddScoped<IGetCountriesResponseMapper, GetCountriesResponseMapper>();

        return services;
    }
}
