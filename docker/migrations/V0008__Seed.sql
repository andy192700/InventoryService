USE Inventory

INSERT INTO warehouse.Country
(
    CountryCode,
    CurrencyCode
)
VALUES ('US', 'USD'),
       ('GB', 'GBP'),
       ('FR', 'EUR');

DECLARE @Now DATETIMEOFFSET(3) = GETUTCDATE()

INSERT INTO warehouse.Product
(
    CreatedDate,
    ProductName,
    ProductDesc
)
VALUES (@Now, 'Hammer', '16oz claw hammer'),
       (@Now, 'Drill', '18v cordless drill'),
       (@Now, 'Saw', 'Hand saw'),
       (@Now, 'Tape Measure', '5m tape measure'),
       (@Now, 'Screwdriver Set', '6 piece screwdriver set');

DECLARE @HammerProductId INT = (SELECT ProductId FROM warehouse.Product WHERE ProductName = 'Hammer')
DECLARE @DrillProductId INT = (SELECT ProductId FROM warehouse.Product WHERE ProductName = 'Drill')
DECLARE @SawProductId INT = (SELECT ProductId FROM warehouse.Product WHERE ProductName = 'Saw')
DECLARE @TapeMeasureProductId INT = (SELECT ProductId FROM warehouse.Product WHERE ProductName = 'Tape Measure')
DECLARE @ScrewdriverSetProductId INT = (SELECT ProductId FROM warehouse.Product WHERE ProductName = 'Screwdriver Set')

DECLARE @USCountryId INT = (SELECT CountryId FROM warehouse.Country WHERE CountryCode = 'US')
DECLARE @GBCountryId INT = (SELECT CountryId FROM warehouse.Country WHERE CountryCode = 'GB')
DECLARE @FRCountryId INT = (SELECT CountryId FROM warehouse.Country WHERE CountryCode = 'FR')

INSERT INTO warehouse.ProductCountry
(
    ProductId,
    CountryId,
    UnitPrice
)
VALUES (@HammerProductId, @USCountryId, 19.99),
       (@DrillProductId, @USCountryId, 99.99),
       (@SawProductId, @USCountryId, 24.99),
       (@TapeMeasureProductId, @USCountryId, 9.99),
       (@ScrewdriverSetProductId, @USCountryId, 14.99),
       (@HammerProductId, @GBCountryId, 15.99),
       (@DrillProductId, @GBCountryId, 79.99),
       (@SawProductId, @GBCountryId, 19.99),
       (@TapeMeasureProductId, @GBCountryId, 8.99),
       (@ScrewdriverSetProductId, @GBCountryId, 11.99),
       (@HammerProductId, @FRCountryId, 17.99),
       (@DrillProductId, @FRCountryId, 89.99),
       (@SawProductId, @FRCountryId, 22.99),
       (@TapeMeasureProductId, @FRCountryId, 9.49),
       (@ScrewdriverSetProductId, @FRCountryId, 13.49);

INSERT INTO warehouse.Customer
(
    CreatedDate,
    FirstName,
    LastName
)
VALUES (@Now, 'John', 'Smith'),
       (@Now, 'Jane', 'Jones'),
       (@Now, 'Pierre', 'Dubois'),
       (@Now, 'Mary', 'Taylor'),
       (@Now, 'Tom', 'Walker');

DECLARE @JohnSmithCustomerId INT = (SELECT CustomerId FROM warehouse.Customer WHERE FirstName = 'John' AND LastName = 'Smith')
DECLARE @JaneJonesCustomerId INT = (SELECT CustomerId FROM warehouse.Customer WHERE FirstName = 'Jane' AND LastName = 'Jones')
DECLARE @PierreDuboisCustomerId INT = (SELECT CustomerId FROM warehouse.Customer WHERE FirstName = 'Pierre' AND LastName = 'Dubois')
DECLARE @MaryTaylorCustomerId INT = (SELECT CustomerId FROM warehouse.Customer WHERE FirstName = 'Mary' AND LastName = 'Taylor')
DECLARE @TomWalkerCustomerId INT = (SELECT CustomerId FROM warehouse.Customer WHERE FirstName = 'Tom' AND LastName = 'Walker')

INSERT INTO warehouse.PurchaseOrder
(
    OrderDate,
    CountryId,
    CustomerId
)
VALUES (DATEADD(DAY, -10, @Now), @GBCountryId, @JohnSmithCustomerId),
       (DATEADD(DAY, -8, @Now), @GBCountryId, @JaneJonesCustomerId),
       (DATEADD(DAY, -6, @Now), @USCountryId, @MaryTaylorCustomerId),
       (DATEADD(DAY, -4, @Now), @FRCountryId, @PierreDuboisCustomerId),
       (DATEADD(DAY, -2, @Now), @GBCountryId, @TomWalkerCustomerId);

DECLARE @PurchaseOrderIdOne INT = (SELECT PurchaseOrderId FROM warehouse.PurchaseOrder WHERE CustomerId = @JohnSmithCustomerId)
DECLARE @PurchaseOrderIdTwo INT = (SELECT PurchaseOrderId FROM warehouse.PurchaseOrder WHERE CustomerId = @JaneJonesCustomerId)
DECLARE @PurchaseOrderIdThree INT = (SELECT PurchaseOrderId FROM warehouse.PurchaseOrder WHERE CustomerId = @MaryTaylorCustomerId)
DECLARE @PurchaseOrderIdFour INT = (SELECT PurchaseOrderId FROM warehouse.PurchaseOrder WHERE CustomerId = @PierreDuboisCustomerId)
DECLARE @PurchaseOrderIdFive INT = (SELECT PurchaseOrderId FROM warehouse.PurchaseOrder WHERE CustomerId = @TomWalkerCustomerId)

DECLARE @HammerGBProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @GBCountryId AND ProductId = @HammerProductId)
DECLARE @DrillGBProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @GBCountryId AND ProductId = @DrillProductId)
DECLARE @ScrewdriverSetGBProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @GBCountryId AND ProductId = @ScrewdriverSetProductId)
DECLARE @SawGBProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @GBCountryId AND ProductId = @SawProductId)

DECLARE @DrillUSProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @USCountryId AND ProductId = @DrillProductId)
DECLARE @TapeMeasureUSProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @USCountryId AND ProductId = @TapeMeasureProductId)

DECLARE @HammerFRProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @FRCountryId AND ProductId = @HammerProductId)
DECLARE @DrillFRProductCountryId INT = (SELECT ProductCountryId FROM warehouse.ProductCountry WHERE CountryId = @FRCountryId AND ProductId = @DrillProductId)

INSERT INTO warehouse.PurchaseOrderLine
(
    PurchaseOrderId,
    ProductCountryId,
    OrderQuantity
)
VALUES (@PurchaseOrderIdOne, @HammerGBProductCountryId, 5),
       (@PurchaseOrderIdOne, @DrillGBProductCountryId, 2),
       (@PurchaseOrderIdTwo, @HammerGBProductCountryId, 3),
       (@PurchaseOrderIdTwo, @ScrewdriverSetGBProductCountryId, 4),
       (@PurchaseOrderIdThree, @DrillUSProductCountryId, 10),
       (@PurchaseOrderIdThree, @TapeMeasureUSProductCountryId, 5),
       (@PurchaseOrderIdFour, @HammerFRProductCountryId, 1),
       (@PurchaseOrderIdFour, @DrillFRProductCountryId, 3),
       (@PurchaseOrderIdFive, @HammerGBProductCountryId, 7),
       (@PurchaseOrderIdFive, @SawGBProductCountryId, 2);

INSERT INTO governance.UserCredential
(UserName, PasswordHash)
VALUES ('admin', '7520E7AF019B59F4A4E79A85C5C42A53238B8A40DD4280EC0357B1CD61ED792B')