using FluentResults;
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
using InventoryService.Infrastructure.Auth;
using InventoryService.Infrastructure.Validation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace InventoryService.Extensions;

public static class ServiceRegistrations
{
    /// <summary>
    /// Register the application's services.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The application's service collection.</returns>
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddInfrastructure();
        services.AddSecurity();
        services.AddHandlerServices();
        services.AddDataIntegrityEvaluators();

        return services;
    }

    /// <summary>
    /// Add the application's cross cutting infrastructure services.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The application's service collection.</returns>
    private static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ISQLConnectionProvider, SQLConnectionProvider>();
        services.AddSingleton<ILocationBuilder, LocationBuilder>();
        services.AddSingleton<IValidationProblemWriter, ValidationProblemWriter>();
        services.AddHttpContextAccessor();

        return services;
    }

    /// <summary>
    /// Add services to support the applications Authentication/Authorization sub-system.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The application's service collection.</returns>
    private static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        services.AddOptions<HashOptions>()
            .BindConfiguration("Hash")
            .ValidateOnStart();
        services.AddSingleton<IComputeHash, ComputeHash>();
        services.AddSingleton<IValidateOptions<HashOptions>, HashOptionsValidator>();
        services.AddSingleton<IVerifyUserQuery, VerifyUserQuery>();
        services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
        services.AddAuthentication(AuthenticationConstants.AuthorizationScheme)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationConstants.AuthorizationScheme, null);
        services.AddAuthorizationBuilder()
            .AddPolicy(AuthenticationConstants.AuthorizationPolicy, policy =>
            {
                policy.RequireClaim(ClaimTypes.Name);
                policy.AddRequirements(new AuthorizationHandlerRequirement());
            });

        return services;
    }

    /// <summary>
    /// Adds feature level services to the collection.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The application's service collection.</returns>
    private static IServiceCollection AddHandlerServices(this IServiceCollection services)
    {
        // Get Products
        services.AddScoped<IGetProductsDBQuery, GetProductsDBQuery>();
        services.AddScoped<IGetProductsResponseMapper, GetProductsResponseMapper>();
        services.AddScoped<IValidator<GetProductsRequest>, GetProductsRequestValidator>();
        services.AddScoped<IGetProductsDBParametersMapper, GetProductsDBParametersMapper>();
        services.AddScoped<IRequestHandler<GetProductsRequest, Result<ICollection<GetProductsResponse>>>, GetProductsHandler>();

        // Get Product
        services.AddScoped<IGetProductDBParametersMapper, GetProductDBParametersMapper>();
        services.AddScoped<IValidator<GetProductRequest>, GetProductRequestValidator>();
        services.AddScoped<IGetProductDBQuery, GetProductDBQuery>();
        services.AddScoped<IGetProductResponseMapper, GetProductResponseMapper>();
        services.AddScoped<IRequestHandler<GetProductRequest, Result<GetProductResponse>>, GetProductHandler>();

        // Create Product
        services.AddScoped<ICreateProductDBParametersMapper, CreateProductDBParametersMapper>();
        services.AddScoped<IValidator<CreateProductRequest>, CreateProductRequestValidator>();
        services.AddScoped<ICreateProductDBCommand, CreateProductDBCommand>();
        services.AddScoped<ICreateProductResponseMapper, CreateProductResponseMapper>();
        services.AddScoped<IRequestHandler<CreateProductRequest, Result<CreateProductResponse>>, CreateProductHandler>(); 
        services.AddScoped<DataIntegrityValidator<CreateProductRequest>, CreateProductCheckProductNameValidator>();
        services.AddScoped<DataIntegrityValidator<CreateProductRequest>, CreateProductCheckCountriesExistValidator>();

        // Get Customers
        services.AddScoped<IGetCustomersDBQuery, GetCustomersDBQuery>();
        services.AddScoped<IGetCustomersResponseMapper, GetCustomersResponseMapper>();
        services.AddScoped<IGetCustomersDBParametersMapper, GetCustomersDBParametersMapper>();
        services.AddScoped<IValidator<GetCustomersRequest>, GetCustomersRequestValidator>();
        services.AddScoped<IRequestHandler<GetCustomersRequest, Result<ICollection<GetCustomersResponse>>>, GetCustomersHandler>();

        // Get Customer
        services.AddScoped<IGetCustomerDBParametersMapper, GetCustomerDBParametersMapper>();
        services.AddScoped<IValidator<GetCustomerRequest>, GetCustomerRequestValidator>();
        services.AddScoped<IGetCustomerDBQuery, GetCustomerDBQuery>();
        services.AddScoped<IGetCustomerResponseMapper, GetCustomerResponseMapper>();
        services.AddScoped<IRequestHandler<GetCustomerRequest, Result<GetCustomerResponse>>, GetCustomerHandler>();

        // Create Customer
        services.AddScoped<ICreateCustomerDBParametersMapper, CreateCustomerDBParametersMapper>();
        services.AddScoped<IValidator<CreateCustomerRequest>, CreateCustomerRequestValidator>();
        services.AddScoped<ICreateCustomerDBCommand, CreateCustomerDBCommand>();
        services.AddScoped<ICreateCustomerResponseMapper, CreateCustomerResponseMapper>();
        services.AddScoped<IRequestHandler<CreateCustomerRequest, Result<CreateCustomerResponse>>, CreateCustomerHandler>();
        services.AddScoped<DataIntegrityValidator<CreateCustomerRequest>, CreateCustomerCheckCustomerNameValidator>();

        // Get Purchase Orders
        services.AddScoped<IGetPurchaseOrdersResponseMapper, GetPurchaseOrdersResponseMapper>();
        services.AddScoped<IGetPurchaseOrdersDBQuery, GetPurchaseOrdersDBQuery>();
        services.AddScoped<IGetPurchaseOrdersDBParametersMapper, GetPurchaseOrdersDBParametersMapper>();
        services.AddScoped<IValidator<GetPurchaseOrdersRequest>, GetPurchaseOrdersRequestValidator>();
        services.AddScoped<IRequestHandler<GetPurchaseOrdersRequest, Result<ICollection<GetPurchaseOrdersResponse>>>, GetPurchaseOrdersHandler>();

        // Get Purchase Order
        services.AddScoped<IGetPurchaseOrderDBParametersMapper, GetPurchaseOrderDBParametersMapper>();
        services.AddScoped<IValidator<GetPurchaseOrderRequest>, GetPurchaseOrderRequestValidator>();
        services.AddScoped<IGetPurchaseOrderDBQuery, GetPurchaseOrderDBQuery>();
        services.AddScoped<IGetPurchaseOrderResponseMapper, GetPurchaseOrderResponseMapper>();
        services.AddScoped<IRequestHandler<GetPurchaseOrderRequest, Result<GetPurchaseOrderResponse>>, GetPurchaseOrderHandler>();

        // Create Purchase Order
        services.AddScoped<ICreatePurchaseOrderDBParametersMapper, CreatePurchaseOrderDBParametersMapper>();
        services.AddScoped<IValidator<CreatePurchaseOrderRequest>, CreatePurchaseOrderRequestValidator>();
        services.AddScoped<ICreatePurchaseOrderDBCommand, CreatePurchaseOrderDBCommand>();
        services.AddScoped<ICreatePurchaseOrderResponseMapper, CreatePurchaseOrderResponseMapper>();
        services.AddScoped<IRequestHandler<CreatePurchaseOrderRequest, Result<CreatePurchaseOrderResponse>>, CreatePurchaseOrderHandler>(); 
        services.AddScoped<DataIntegrityValidator<CreatePurchaseOrderRequest>, CreatePurchaseOrderCheckCustomerExistsValidator>();
        services.AddScoped<DataIntegrityValidator<CreatePurchaseOrderRequest>, CreatePurchaseOrderCheckProductCountryValidator>();

        // Get Countries
        services.AddScoped<IGetCountriesDBQuery, GetCountriesDBQuery>();
        services.AddScoped<IGetCountriesResponseMapper, GetCountriesResponseMapper>();
        services.AddScoped<IRequestHandler<GetCountriesRequest, Result<ICollection<GetCountriesResponse>>>, GetCountriesHandler>();

        return services;
    }

    /// <summary>
    /// Adds Data Integrity Evaluators, automatically, should a <see cref="DataIntegrityValidator{TRequest}"/> be registered,
    /// per request type.
    /// </summary>
    /// <param name="services">The application's service collection.</param>
    /// <returns>The application's service collection.</returns>
    private static IServiceCollection AddDataIntegrityEvaluators(this IServiceCollection services)
    {
        var requestTypes = services
            .Where(x =>
                x.ServiceType.IsGenericType &&
                x.ServiceType.GetGenericTypeDefinition() == typeof(DataIntegrityValidator<>))
            .Select(x => x.ServiceType.GenericTypeArguments[0])
            .Distinct()
            .ToList();

        foreach (var requestType in requestTypes)
        {
            var abstrType = typeof(IDataIntegrityEvaluator<>).MakeGenericType(requestType);
            var implType = typeof(DataIntegrityEvaluator<>).MakeGenericType(requestType);

            services.AddScoped(abstrType, implType);
        }

        return services;
    }
}
