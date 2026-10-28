USE Inventory

GO

CREATE PROC warehouse.usp_GetPurchaseOrders
    @ProductId INT = 0,
    @CountryId INT = 0,
    @CustomerId INT = 0
AS
BEGIN
    DECLARE @PurchaseOrders TABLE
    (
        PurchaseOrderId INT,
        OrderDate DATETIMEOFFSET(3),
        CustomerId INT,
        CountryId INT
    )

    INSERT INTO @PurchaseOrders
    SELECT po.PurchaseOrderId,
           po.OrderDate,
           co.CustomerId,
           c.CountryId
    FROM warehouse.PurchaseOrder po
    JOIN warehouse.Customer co
    ON co.CustomerId = po.CustomerId
    AND 
    (
        @CustomerId = 0 OR co.CustomerId = @CustomerId
    )
    JOIN warehouse.Country c
    ON c.CountryId = po.CountryId
    AND 
    (
        @CountryId = 0 OR c.CountryId = @CountryId
    )

    SELECT PurchaseOrderId,
           OrderDate,
           CustomerId,
           CountryId
    FROM @PurchaseOrders
    ORDER BY PurchaseOrderId

    SELECT pol.PurchaseOrderId,
           pol.PurchaseOrderLineId,
           pc.ProductId,
           pol.OrderQuantity,
           ROW_NUMBER() OVER(PARTITION BY pol.PurchaseOrderId ORDER BY pol.PurchaseOrderLineId) RowNo
    FROM warehouse.PurchaseOrderLine pol
    JOIN warehouse.ProductCountry pc
    ON pc.ProductCountryId = pol.ProductCountryId
    AND
    (
        @ProductId = 0 OR pc.ProductId = @ProductId
    )
    JOIN @PurchaseOrders p
    ON p.PurchaseOrderId = pol.PurchaseOrderId
    ORDER BY pol.PurchaseOrderId,
             RowNo
END

GO

CREATE PROC warehouse.usp_GetPurchaseOrder
    @PurchaseOrderId INT
AS
BEGIN
    SELECT po.PurchaseOrderId,
           po.OrderDate,
           co.CustomerId,
           c.CountryId
    FROM warehouse.PurchaseOrder po
    JOIN warehouse.Customer co
    ON co.CustomerId = po.CustomerId
    JOIN warehouse.Country c
    ON c.CountryId = po.CountryId
    WHERE po.PurchaseOrderId = @PurchaseOrderId

    SELECT pol.PurchaseOrderLineId,
           pc.ProductId,
           pol.OrderQuantity,           
           ROW_NUMBER() OVER(ORDER BY pol.PurchaseOrderLineId) RowNo
    FROM warehouse.PurchaseOrderLine pol
    JOIN warehouse.ProductCountry pc
    ON pc.ProductCountryId = pol.ProductCountryId
    WHERE pol.PurchaseOrderId = @PurchaseOrderId
    ORDER BY RowNo
END

GO

CREATE TYPE warehouse.PurchaseOrderLine AS TABLE
(
    ProductId INT NOT NULL,
    OrderQuantity INT NOT NULL
)

GO

CREATE PROC warehouse.usp_CreatePurchaseOrder
    @CustomerId INT,
    @CountryId INT,
    @OrderLines warehouse.PurchaseOrderLine READONLY
AS
BEGIN
    DECLARE @Now DATETIMEOFFSET(3) = GETUTCDATE()

    INSERT INTO warehouse.PurchaseOrder
    (OrderDate, CountryId, CustomerId)
    SELECT @Now, 
           @CountryId,
           @CustomerId

    DECLARE @PurchaseOrderId INT = SCOPE_IDENTITY()

    INSERT INTO warehouse.PurchaseOrderLine
    (PurchaseOrderId, ProductCountryId, OrderQuantity)
    SELECT @PurchaseOrderId,
           pc.ProductCountryId,
           o.OrderQuantity
    FROM @OrderLines o
    JOIN warehouse.ProductCountry pc
    ON pc.ProductId = o.ProductId
    AND pc.CountryId = @CountryId

    SELECT @PurchaseOrderId
END

GO

CREATE PROC warehouse.usp_CheckPurchaseOrderProductCountry
    @CountryId INT,
    @OrderLines warehouse.PurchaseOrderLine READONLY
AS
BEGIN
    DECLARE @SuppliedLineCount INT = (SELECT COUNT(1) FROM @OrderLines)

    DECLARE @ProductCountryFoundCount INT

    SELECT @ProductCountryFoundCount = COUNT(1)
    FROM warehouse.ProductCountry pc
    JOIN @OrderLines o
    ON o.ProductId = pc.ProductId
    WHERE pc.CountryId = @CountryId

    SELECT IIF(@SuppliedLineCount = @ProductCountryFoundCount, 1, 0)
END