using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using InventoryService.IntegrationTests.Features.Products;
using InventoryService.IntegrationTests.Features.PurchaseOrders;

namespace InventoryService.IntegrationTests;

/// <summary>
/// A test method attribute for injecting constructed objects into tests,
/// supplying its own specimens to overwrite AutoFixture's default behaviour.
/// </summary>
public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute()
        : base(() =>
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization());

            fixture.Customizations.Add(new CreateProductIntRequestSpecimenBuilder()); 
            fixture.Customizations.Add(new CreatePurchaseOrderIntRequestSpecimenBuilder());

            return fixture;
        })
    {
    }
}

/// <summary>
/// A test method attribute for injecting constructed objects into tests,
/// supplying its own specimens to overwrite AutoFixture's default behaviour.
/// This attribute has the option of suppying in line parameters at a method level.
/// </summary>
public class InlineAutoMoqDataAttribute : CompositeDataAttribute
{
    public InlineAutoMoqDataAttribute(params object[] values)
        : base(
            new InlineDataAttribute(values), 
            new AutoMoqDataAttribute())
    {
    }
}

public class CreateProductIntRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(CreateProductIntRequest))
        {
            return new CreateProductIntRequest
            {
                ProductName = "ProductName",
                ProductDesc = "ProductDesc",
                ProductCountries = new List<CreateProductIntRequestProductCountries>
                {
                    {
                        new CreateProductIntRequestProductCountries
                        {
                            CountryId = "1",
                            UnitPrice = "3.44"
                        }
                    },
                    {
                        new CreateProductIntRequestProductCountries
                        {
                            CountryId = "2",
                            UnitPrice = "1.24"
                        }
                    }
                }
            };
        }

        return new NoSpecimen();
    }
}

public class CreatePurchaseOrderIntRequestSpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type && type == typeof(CreatePurchaseOrderIntRequest))
        {
            return new CreatePurchaseOrderIntRequest
            {
                CustomerId = "3",
                CountryId = "2",
                lines = new List<CreatePurchaseOrderIntRequestLine>
                {
                    {
                        new CreatePurchaseOrderIntRequestLine
                        {
                            ProductId = "2",
                            OrderQuantity = "37"
                        }
                    },
                    {
                        new CreatePurchaseOrderIntRequestLine
                        {
                            ProductId = "4",
                            OrderQuantity = "35"
                        }
                    }
                }
            };
        }

        return new NoSpecimen();
    }
}