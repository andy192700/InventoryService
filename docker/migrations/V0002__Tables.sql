USE Inventory

GO

CREATE SCHEMA governance

GO

CREATE SCHEMA warehouse

GO

CREATE TABLE warehouse.Country
(
    CountryId INT NOT NULL IDENTITY(1,1),
    CountryCode VARCHAR(2) NOT NULL,
    CurrencyCode VARCHAR(3) NOT NULL
)

ALTER TABLE warehouse.Country
ADD CONSTRAINT PK_Country
PRIMARY KEY (CountryId)

GO

CREATE TABLE warehouse.Product
(
    ProductId INT NOT NULL IDENTITY(1,1),
    CreatedDate DATETIMEOFFSET(3) NOT NULL,
    ProductName VARCHAR(100) NOT NULL,
    ProductDesc VARCHAR(255) NOT NULL
)

ALTER TABLE warehouse.Product
ADD CONSTRAINT PK_Product
PRIMARY KEY (ProductId)

CREATE UNIQUE INDEX UX_Product_ProductName
ON warehouse.Product (ProductName)

GO

CREATE TABLE warehouse.ProductCountry
(
    ProductCountryId INT NOT NULL IDENTITY(1,1),
    ProductId INT NOT NULL,
    CountryId INT NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL
)

ALTER TABLE warehouse.ProductCountry
ADD CONSTRAINT PK_ProductCountry
PRIMARY KEY (ProductCountryId)

ALTER TABLE warehouse.ProductCountry
ADD CONSTRAINT FK_ProductCountry_Product
FOREIGN KEY (ProductId) REFERENCES warehouse.Product(ProductId)

ALTER TABLE warehouse.ProductCountry
ADD CONSTRAINT FK_ProductCountry_Country
FOREIGN KEY (CountryId) REFERENCES warehouse.Country(CountryId)

CREATE UNIQUE INDEX UX_ProductCountry_ProductId_CountryId
ON warehouse.ProductCountry (ProductId, CountryId)

GO

CREATE TABLE warehouse.Customer
(
    CustomerId INT NOT NULL IDENTITY(1,1),
    CreatedDate DATETIMEOFFSET(3) NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100) NOT NULL
)

ALTER TABLE warehouse.Customer
ADD CONSTRAINT PK_Customer
PRIMARY KEY (CustomerId)

GO

CREATE TABLE warehouse.PurchaseOrder
(
    PurchaseOrderId INT NOT NULL IDENTITY(1,1),
    OrderDate DATETIMEOFFSET(3) NOT NULL,
    CountryId INT NOT NULL,
    CustomerId INT NOT NULL
)

ALTER TABLE warehouse.PurchaseOrder
ADD CONSTRAINT PK_PurchaseOrder
PRIMARY KEY (PurchaseOrderId)

ALTER TABLE warehouse.PurchaseOrder
ADD CONSTRAINT FK_PurchaseOrder_Customer
FOREIGN KEY (CustomerId) REFERENCES warehouse.Customer(CustomerId)

ALTER TABLE warehouse.PurchaseOrder
ADD CONSTRAINT FK_PurchaseOrder_Country
FOREIGN KEY (CountryId) REFERENCES warehouse.Country(CountryId)

GO

CREATE FUNCTiON warehouse.udf_PurchaseOrderCountryMatch
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
        FROM warehouse.PurchaseOrder po
        JOIN warehouse.ProductCountry pc
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

CREATE TABLE warehouse.PurchaseOrderLine
(
    PurchaseOrderLineId INT NOT NULL IDENTITY(1,1),
    PurchaseOrderId INT NOT NULL,
    ProductCountryId INT NOT NULL,
    OrderQuantity INT NOT NULL
)

ALTER TABLE warehouse.PurchaseOrderLine
ADD CONSTRAINT PK_PurchaseOrderLine
PRIMARY KEY (PurchaseOrderLineId)

ALTER TABLE warehouse.PurchaseOrderLine
ADD CONSTRAINT FK_PurchaseOrderLine_PurchaseOrderLine
FOREIGN KEY (PurchaseOrderId) REFERENCES warehouse.PurchaseOrder(PurchaseOrderId)

ALTER TABLE warehouse.PurchaseOrderLine
ADD CONSTRAINT FK_PurchaseOrder_ProductCountryId
FOREIGN KEY (ProductCountryId) REFERENCES warehouse.ProductCountry(ProductCountryId)

ALTER TABLE warehouse.PurchaseOrderLine
ADD CONSTRAINT CK_PurchaseOrderLine_CountryMatch
CHECK (warehouse.udf_PurchaseOrderCountryMatch(PurchaseOrderId, ProductCountryId) = 1)

CREATE UNIQUE INDEX UX_PurchaseOrderLine_PurchaseOrderId_ProductCountryId
ON warehouse.PurchaseOrderLine (PurchaseOrderId, ProductCountryId)

GO

CREATE TABLE governance.UserCredential
(
    UserCredentialId INT IDENTITY(1,1) not null,
    UserName VARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL
)

ALTER TABLE governance.UserCredential
ADD CONSTRAINT PK_UserCredential
PRIMARY KEY (UserCredentialId)

CREATE UNIQUE INDEX UX_UserCredential_UserCredentialId
ON governance.UserCredential (UserName)