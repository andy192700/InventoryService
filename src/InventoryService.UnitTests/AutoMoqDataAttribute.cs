using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using InventoryService.Features.Customers.GetCustomer;
using InventoryService.Features.Products.CreateProduct;
using InventoryService.Features.Products.GetProduct;
using InventoryService.Features.PurchaseOrders.CreatePurchaseOrder;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrder;
using InventoryService.Features.PurchaseOrders.GetPurchaseOrders;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace InventoryService.UnitTests;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());

            fixture.Customizations.Add(new CreatePurchaseOrderRequestSpecimenBuilder()); 
            fixture.Customizations.Add(new CreateProductRequestSpecimenBuilder()); 
            fixture.Customizations.Add(new GetPurchaseOrderRequestSpecimenBuilder());
            fixture.Customizations.Add(new GetPurchaseOrdersRequestSpecimenBuilder());
            fixture.Customizations.Add(new CreateProductDBParametersSpecimenBuilder());
            fixture.Customizations.Add(new CreatePurchaseOrderDBParametersSpecimenBuilder());
            fixture.Customizations.Add(new GetProductRequestSpecimenBuilder());
            fixture.Customizations.Add(new GetCustomerRequestSpecimenBuilder());
            fixture.Customizations.Add(new QueryStringSpecimenBuilder());

            return fixture;
        })
    {
    }
}

public class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(
            new InlineDataAttribute(values), 
            new AutoMoqDataAttribute())
    {
    }
}

public class CreatePurchaseOrderRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(CreatePurchaseOrderRequest))
        {
            var lines = new List<CreatePurchaseOrderPayloadLine>()
            {
                new CreatePurchaseOrderPayloadLine
                {
                    OrderQuantity = "2",
                    ProductId = "2"
                },
                new CreatePurchaseOrderPayloadLine
                {
                    OrderQuantity = "1",
                    ProductId = "1"
                }
            };

            return new CreatePurchaseOrderRequest
            (
                new CreatePurchaseOrderPayload
                { 
                    CustomerId = "2",
                    CountryId = "1",
                    lines = lines
                }
            );
        }

        return new NoSpecimen();
    }
}


public class CreateProductRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(CreateProductRequest))
        {
            var productCountries = new List<CreateProductRequestProductCountries>()
            {
                new CreateProductRequestProductCountries
                {
                    UnitPrice = "2.22",
                    CountryId = "2"
                },
                new CreateProductRequestProductCountries
                {
                    UnitPrice = "1.1",
                    CountryId = "1"
                }
            };

            return new CreateProductRequest
            (
                new CreateProductRequestPayload
                {
                    ProductName = "product_name",
                    ProductDesc = "product_desc",
                    ProductCountries = productCountries
                }
            );
        }

        return new NoSpecimen();
    }
}


public class GetPurchaseOrderRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(GetPurchaseOrderRequest))
        {
            return new GetPurchaseOrderRequest
            (
                PurchaseOrderId: "3"
            );
        }

        return new NoSpecimen();
    }
}

public class GetPurchaseOrdersRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(GetPurchaseOrdersRequest))
        {
            return new GetPurchaseOrdersRequest
            (
                ProductId: "3",
                CustomerId: "4",
                CountryId: "2"
            );
        }

        return new NoSpecimen();
    }
}

public class CreateProductDBParametersSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(CreateProductDBParameters))
        {
            var createProductRequest = new CreateProductRequest
                (
                    new CreateProductRequestPayload
                    {
                        ProductName = "ProductName",
                        ProductDesc = "ProductDesc",
                        ProductCountries = new List<CreateProductRequestProductCountries>
                        {
                            {
                                new CreateProductRequestProductCountries
                                {
                                    CountryId = "1",
                                    UnitPrice = "3.44"
                                }
                            },
                            {
                                new CreateProductRequestProductCountries
                                {
                                    CountryId = "2",
                                    UnitPrice = "1.24"
                                }
                            }
                        }
                    }
                );

            var productCountriesdt = new DataTable();

            productCountriesdt.Columns.Add("CountryId", typeof(int));
            productCountriesdt.Columns.Add("UnitPrice", typeof(double));

            foreach (var country in createProductRequest.Payload.ProductCountries!)
            {
                var row = productCountriesdt.NewRow();

                row.SetField("CountryId", country.CountryId);
                row.SetField("UnitPrice", country.UnitPrice);

                productCountriesdt.Rows.Add(row);
            }

            return new CreateProductDBParameters
            {
                ProductName = createProductRequest.Payload.ProductName!,
                ProductDesc = createProductRequest.Payload.ProductDesc!,
                ProductCountries = productCountriesdt
            };
        }

        return new NoSpecimen();
    }
}

public class CreatePurchaseOrderDBParametersSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(CreatePurchaseOrderDBParameters))
        {
            var createPurchaseOrderRequest = new CreatePurchaseOrderRequest
            (
                new CreatePurchaseOrderPayload
                {
                    CountryId = "2",
                    CustomerId = "7",
                    lines = new List<CreatePurchaseOrderPayloadLine>
                    {
                        {
                            new CreatePurchaseOrderPayloadLine
                            {
                                OrderQuantity = "75",
                                ProductId = "4"
                            }
                        },
                        {
                            new CreatePurchaseOrderPayloadLine
                            {
                                OrderQuantity = "22",
                                ProductId = "3"
                            }
                        }
                    }
                }
            );

            var linesdt = new DataTable();

            linesdt.Columns.Add("ProductCountryId");
            linesdt.Columns.Add("OrderQuantity");

            foreach (var line in createPurchaseOrderRequest.Payload.lines!)
            {
                var row = linesdt.NewRow();

                row.SetField("ProductCountryId", int.Parse(line.ProductId!));
                row.SetField("OrderQuantity", int.Parse(line.OrderQuantity!));

                linesdt.Rows.Add(row);
            }

            return new CreatePurchaseOrderDBParameters
            {
                CountryId = int.Parse(createPurchaseOrderRequest.Payload.CountryId!),
                CustomerId = int.Parse(createPurchaseOrderRequest.Payload.CustomerId!),
                Lines = linesdt
            };
        }

        return new NoSpecimen();
    }
}

public class GetProductRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(GetProductRequest))
        {
            return new GetProductRequest
            (
               ProductId: "3"
            );
        }

        return new NoSpecimen();
    }
}

public class GetCustomerRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(GetCustomerRequest))
        {
            return new GetCustomerRequest
            (
               CustomerId: "3"
            );
        }

        return new NoSpecimen();
    }
}

public class QueryStringSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(QueryString))
        {
            return new QueryString("?this=that");
        }

        return new NoSpecimen();
    }
}