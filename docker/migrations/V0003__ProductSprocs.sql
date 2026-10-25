USE Inventory

GO

CREATE PROC dbo.usp_GetProducts
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
    FROM dbo.Product
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
    FROM dbo.Country c
    JOIN dbo.ProductCountry pc
    ON pc.CountryId = c.CountryId
    JOIN @Products p
    ON p.ProductId = pc.ProductId
    ORDER BY ProductId,
             CountryId
END

GO

CREATE PROC dbo.usp_GetProduct 
    @ProductId INT
AS
BEGIN
    SELECT ProductId,
           CreatedDate,
           ProductName,
           ProductDesc
    FROM dbo.Product
    WHERE ProductId = @ProductId

    SELECT c.CountryId,
           pc.UnitPrice
    FROM dbo.Country c
    JOIN dbo.ProductCountry pc
    ON pc.CountryId = c.CountryId
    JOIN dbo.Product p
    ON p.ProductId = pc.ProductId
    AND p.ProductId = @ProductId
    ORDER BY CountryId
END

GO

CREATE TYPE dbo.tvp_ProductCountry AS TABLE
(
    CountryId INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL
)

GO

CREATE PROC dbo.usp_CreateProduct 
    @ProductName VARCHAR(100),
    @ProductDesc VARCHAR(255),
    @ProductCountries dbo.tvp_ProductCountry READONLY
AS
BEGIN
    DECLARE @Now DATETIMEOFFSET(3) = GETUTCDATE()

    INSERT INTO dbo.Product
    (CreatedDate, ProductName, ProductDesc)
    VALUES (@Now, @ProductName, @ProductDesc)

    DECLARE @ProductId INT = SCOPE_IDENTITY()

    INSERT INTO dbo.ProductCountry
    (ProductId, CountryId, UnitPrice)
    SELECT @ProductId,
           CountryId,
           UnitPrice
    FROM @ProductCountries

    SELECT @ProductId
END