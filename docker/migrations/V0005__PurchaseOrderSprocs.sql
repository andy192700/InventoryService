USE Inventory

GO

CREATE PROC warehouse.usp_GetPurchaseOrders
    @ProductId INT = 0,
    @CountryId INT = 0,
    @CustomerId INT = 0
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @PurchaseOrders TABLE
    (
        PurchaseOrderId INT,
        OrderDate DATETIMEOFFSET(3),
        CustomerId INT,
        CountryId INT,
        [Status] VARCHAR(25)
    )

    INSERT INTO @PurchaseOrders
    SELECT po.PurchaseOrderId,
           po.OrderDate,
           co.CustomerId,
           c.CountryId,
           pos.[Status]
    FROM warehouse.PurchaseOrder po
    JOIN warehouse.PurchaseOrderStatus pos
    ON pos.PurchaseOrderStatusId = po.PurchaseOrderStatusId
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
    JOIN warehouse.PurchaseOrderLine pol
    ON pol.PurchaseOrderId = po.PurchaseOrderId
    JOIN warehouse.ProductCountry pc
    ON pc.ProductCountryId = pol.ProductCountryId
    AND
    (
        @ProductId = 0 OR pc.ProductId = @ProductId
    )
    GROUP BY po.PurchaseOrderId,
             po.OrderDate,
             co.CustomerId,
             c.CountryId,
             pos.[Status]

    SELECT PurchaseOrderId,
           OrderDate,
           CustomerId,
           CountryId,
           [Status]
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
    SET NOCOUNT ON

    SELECT po.PurchaseOrderId,
           po.OrderDate,
           co.CustomerId,
           c.CountryId,
           pos.[Status]
    FROM warehouse.PurchaseOrder po
    JOIN warehouse.PurchaseOrderStatus pos
    ON pos.PurchaseOrderStatusId = po.PurchaseOrderStatusId
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
    SET NOCOUNT ON

    DECLARE @Now DATETIMEOFFSET(3) = GETUTCDATE()

    DECLARE @OrderPreparingStatusId INT = (SELECT PurchaseOrderStatusId FROM warehouse.PurchaseOrderStatus WHERE [Status] = 'Preparing')

    INSERT INTO warehouse.PurchaseOrder
    (OrderDate, CountryId, CustomerId, PurchaseOrderStatusId)
    SELECT @Now, 
           @CountryId,
           @CustomerId,
           @OrderPreparingStatusId

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
    SET NOCOUNT ON

    DECLARE @SuppliedLineCount INT = (SELECT COUNT(1) FROM @OrderLines)

    DECLARE @ProductCountryFoundCount INT

    SELECT @ProductCountryFoundCount = COUNT(1)
    FROM warehouse.ProductCountry pc
    JOIN @OrderLines o
    ON o.ProductId = pc.ProductId
    WHERE pc.CountryId = @CountryId

    SELECT IIF(@SuppliedLineCount = @ProductCountryFoundCount, 1, 0)
END