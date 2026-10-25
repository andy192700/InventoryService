USE Inventory

INSERT INTO dbo.Country
(
    CountryCode,
    CurrencyCode
)
VALUES
('US', 'USD'),
('GB', 'GBP'),
('FR', 'EUR');

INSERT INTO dbo.Product
(
    CreatedDate,
    ProductName,
    ProductDesc
)
VALUES
(SYSDATETIMEOFFSET(), 'Hammer', '16oz claw hammer'),
(SYSDATETIMEOFFSET(), 'Drill', '18v cordless drill'),
(SYSDATETIMEOFFSET(), 'Saw', 'Hand saw'),
(SYSDATETIMEOFFSET(), 'Tape Measure', '5m tape measure'),
(SYSDATETIMEOFFSET(), 'Screwdriver Set', '6 piece screwdriver set');


INSERT INTO dbo.ProductCountry
(
    ProductId,
    CountryId,
    UnitPrice
)
VALUES
-- US
(1, 1, 19.99),
(2, 1, 99.99),
(3, 1, 24.99),
(4, 1, 9.99),
(5, 1, 14.99),

-- GB
(1, 2, 15.99),
(2, 2, 79.99),
(3, 2, 19.99),
(4, 2, 8.99),
(5, 2, 11.99),

-- FR
(1, 3, 17.99),
(2, 3, 89.99),
(3, 3, 22.99),
(4, 3, 9.49),
(5, 3, 13.49);

INSERT INTO dbo.Customer
(
    CreatedDate,
    FirstName,
    LastName
)
VALUES
(SYSDATETIMEOFFSET(), 'John', 'Smith'),
(SYSDATETIMEOFFSET(), 'Jane', 'Jones'),
(SYSDATETIMEOFFSET(), 'Pierre', 'Dubois'),
(SYSDATETIMEOFFSET(), 'Mary', 'Taylor'),
(SYSDATETIMEOFFSET(), 'Tom', 'Walker');

INSERT INTO dbo.PurchaseOrder
(
    OrderDate,
    CountryId,
    CustomerId
)
VALUES
(DATEADD(DAY, -10, SYSDATETIMEOFFSET()), 2, 1),
(DATEADD(DAY, -8, SYSDATETIMEOFFSET()), 2, 2),
(DATEADD(DAY, -6, SYSDATETIMEOFFSET()), 1, 4),
(DATEADD(DAY, -4, SYSDATETIMEOFFSET()), 3, 3),
(DATEADD(DAY, -2, SYSDATETIMEOFFSET()), 2, 5);

INSERT INTO dbo.PurchaseOrderLine
(
    PurchaseOrderId,
    ProductCountryId,
    OrderQuantity
)
VALUES
-- Order 1 (GB)
(1, 6, 5),   -- Hammer
(1, 7, 2),   -- Drill

-- Order 2 (GB)
(2, 6, 3),   -- Hammer
(2, 10, 4),  -- Screwdriver Set

-- Order 3 (US)
(3, 2, 10),  -- Drill
(3, 4, 5),   -- Tape Measure

-- Order 4 (FR)
(4, 11, 1),  -- Hammer
(4, 12, 3),  -- Drill

-- Order 5 (GB)
(5, 6, 7),   -- Hammer
(5, 8, 2);   -- Saw
