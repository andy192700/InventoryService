USE Inventory

GO

CREATE PROC warehouse.usp_GetProducts
    @Filter VARCHAR(100) = NULL
AS
BEGIN
    DECLARE @Products TABLE
    (
        ProductId INT,
        CreatedDate DATETIMEOFFSET(3),
        ProductName VARCHAR(100),
        ProductDesc VARCHAR(255)
    )

    INSERT INTO @Products
    SELECT ProductId,
           CreatedDate,
           ProductName,
           ProductDesc
    FROM warehouse.Product
    WHERE @Filter IS NULL
    OR (ProductName + ProductDesc) LIKE '%' + @Filter + '%'

    SELECT ProductId,
           CreatedDate,
           ProductName,
           ProductDesc
    FROM @Products
    ORDER BY ProductId 

    SELECT p.ProductId,
           c.CountryId,
           pc.UnitPrice
    FROM warehouse.Country c
    JOIN warehouse.ProductCountry pc
    ON pc.CountryId = c.CountryId
    JOIN @Products p
    ON p.ProductId = pc.ProductId
    ORDER BY ProductId,
             CountryId
END

GO

CREATE PROC warehouse.usp_GetProduct 
    @ProductId INT
AS
BEGIN
    SELECT ProductId,
           CreatedDate,
           ProductName,
           ProductDesc
    FROM warehouse.Product
    WHERE ProductId = @ProductId

    SELECT c.CountryId,
           pc.UnitPrice
    FROM warehouse.Country c
    JOIN warehouse.ProductCountry pc
    ON pc.CountryId = c.CountryId
    JOIN warehouse.Product p
    ON p.ProductId = pc.ProductId
    AND p.ProductId = @ProductId
    ORDER BY CountryId
END

GO

CREATE TYPE warehouse.tvp_ProductCountry AS TABLE
(
    CountryId INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL
)

GO

CREATE PROC warehouse.usp_CreateProduct 
    @ProductName VARCHAR(100),
    @ProductDesc VARCHAR(255),
    @ProductCountries warehouse.tvp_ProductCountry READONLY
AS
BEGIN
    DECLARE @Now DATETIMEOFFSET(3) = GETUTCDATE()

    INSERT INTO warehouse.Product
    (CreatedDate, ProductName, ProductDesc)
    VALUES (@Now, @ProductName, @ProductDesc)

    DECLARE @ProductId INT = SCOPE_IDENTITY()

    INSERT INTO warehouse.ProductCountry
    (ProductId, CountryId, UnitPrice)
    SELECT @ProductId,
           CountryId,
           UnitPrice
    FROM @ProductCountries

    SELECT @ProductId
END

GO

CREATE PROC warehouse.usp_CheckProductName @ProductName VARCHAR(100)
AS
BEGIN
    IF EXISTS 
    (
        SELECT 1
        FROM warehouse.Product
        WHERE ProductName = @ProductName
    )
    BEGIN
        SELECT 0
    END

    SELECT 1
END