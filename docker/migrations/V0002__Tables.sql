USE Inventory

CREATE TABLE dbo.Country
(
    CountryId INT NOT NULL IDENTITY(1,1),
    CountryCode VARCHAR(2) NOT NULL,
    CurrencyCode VARCHAR(3) NOT NULL
)

ALTER TABLE dbo.Country
ADD CONSTRAINT PK_Country
PRIMARY KEY (CountryId)

GO

CREATE TABLE dbo.Product
(
    ProductId INT NOT NULL IDENTITY(1,1),
    CreatedDate DATETIMEOFFSET(3) NOT NULL,
    ProductName VARCHAR(100) NOT NULL,
    ProductDesc VARCHAR(255) NOT NULL
)

ALTER TABLE dbo.Product
ADD CONSTRAINT PK_Product
PRIMARY KEY (ProductId)

CREATE UNIQUE INDEX UX_Product_ProductName
ON dbo.Product (ProductName)

GO

CREATE TABLE dbo.ProductCountry
(
    ProductCountryId INT NOT NULL IDENTITY(1,1),
    ProductId INT NOT NULL,
    CountryId INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL
)

ALTER TABLE dbo.ProductCountry
ADD CONSTRAINT PK_ProductCountry
PRIMARY KEY (ProductCountryId)

ALTER TABLE dbo.ProductCountry
ADD CONSTRAINT FK_ProductCountry_Product
FOREIGN KEY (ProductId) REFERENCES dbo.Product(ProductId)

ALTER TABLE dbo.ProductCountry
ADD CONSTRAINT FK_ProductCountry_Country
FOREIGN KEY (CountryId) REFERENCES dbo.Country(CountryId)

CREATE UNIQUE INDEX UX_ProductCountry_ProductId_CountryId
ON dbo.ProductCountry (ProductId, CountryId)

GO

CREATE TABLE dbo.Customer
(
    CustomerId INT NOT NULL IDENTITY(1,1),
    CreatedDate DATETIMEOFFSET(3) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL
)

ALTER TABLE dbo.Customer
ADD CONSTRAINT PK_Customer
PRIMARY KEY (CustomerId)

GO

CREATE TABLE dbo.PurchaseOrder
(
    PurchaseOrderId INT NOT NULL IDENTITY(1,1),
    OrderDate DATETIMEOFFSET(3) NOT NULL,
    CountryId INT NOT NULL,
    CustomerId INT NOT NULL
)

ALTER TABLE dbo.PurchaseOrder
ADD CONSTRAINT PK_PurchaseOrder
PRIMARY KEY (PurchaseOrderId)

ALTER TABLE dbo.PurchaseOrder
ADD CONSTRAINT FK_PurchaseOrder_Customer
FOREIGN KEY (CustomerId) REFERENCES dbo.Customer(CustomerId)

ALTER TABLE dbo.PurchaseOrder
ADD CONSTRAINT FK_PurchaseOrder_Country
FOREIGN KEY (CountryId) REFERENCES dbo.Country(CountryId)

GO

CREATE FUNCTiON dbo.udf_PurchaseOrderCountryMatch
(
    @PurchaseOrderId INT,
    @ProductCountryId INT
)
RETURNS BIT
AS
BEGIN
    IF EXISTS 
    (
        SELECT 1
        FROM dbo.PurchaseOrder po
        JOIN dbo.ProductCountry pc
        ON pc.CountryId = po.CountryId
        AND pc.ProductCountryId = @ProductCountryId
        WHERE po.PurchaseOrderId = @PurchaseOrderId
    )
    BEGIN
        RETURN 1
    END

    RETURN 0    
END

GO

CREATE TABLE dbo.PurchaseOrderLine
(
    PurchaseOrderLineId INT NOT NULL IDENTITY(1,1),
    PurchaseOrderId INT NOT NULL,
    ProductCountryId INT NOT NULL,
    OrderQuantity INT NOT NULL
)

ALTER TABLE dbo.PurchaseOrderLine
ADD CONSTRAINT PK_PurchaseOrderLine
PRIMARY KEY (PurchaseOrderLineId)

ALTER TABLE dbo.PurchaseOrderLine
ADD CONSTRAINT FK_PurchaseOrderLine_PurchaseOrderLine
FOREIGN KEY (PurchaseOrderId) REFERENCES dbo.PurchaseOrder(PurchaseOrderId)

ALTER TABLE dbo.PurchaseOrderLine
ADD CONSTRAINT FK_PurchaseOrder_ProductCountryId
FOREIGN KEY (ProductCountryId) REFERENCES dbo.ProductCountry(ProductCountryId)

ALTER TABLE dbo.PurchaseOrderLine
ADD CONSTRAINT CK_PurchaseOrderLine_CountryMatch
CHECK (dbo.udf_PurchaseOrderCountryMatch(PurchaseOrderId, ProductCountryId) = 1)

CREATE UNIQUE INDEX UX_PurchaseOrderLine_PurchaseOrderId_ProductCountryId
ON dbo.PurchaseOrderLine (PurchaseOrderId, ProductCountryId)

GO

CREATE TABLE dbo.UserCredential
(
    UserCredentialId INT IDENTITY(1,1) not null,
    UserName VARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL
)

ALTER TABLE dbo.UserCredential
ADD CONSTRAINT PK_UserCredential
PRIMARY KEY (UserCredentialId)

CREATE UNIQUE INDEX UX_UserCredential_UserCredentialId
ON dbo.UserCredential (UserName)