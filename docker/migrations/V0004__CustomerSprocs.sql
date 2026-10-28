USE Inventory

GO

CREATE PROC warehouse.usp_GetCustomers
    @Filter VARCHAR(100) = NULL
AS
BEGIN
    SELECT CustomerId,
           CreatedDate,
           FirstName,
           LastName
    FROM warehouse.Customer
    WHERE @Filter IS NULL
    OR (FirstName + LastName) LIKE '%' + @Filter + '%'
    ORDER BY CustomerId
END

GO

CREATE PROC warehouse.usp_GetCustomer
    @CustomerId INT
AS
BEGIN
    SELECT CustomerId,
           CreatedDate,
           FirstName,
           LastName
    FROM warehouse.Customer    
    WHERE CustomerId = @CustomerId
END

GO

CREATE PROC warehouse.usp_CreateCustomer
    @FirstName VARCHAR(100),
    @LastName VARCHAR(100)
AS
BEGIN
    DECLARE @Now DATETIMEOFFSET(3) = GETUTCDATE()

    INSERT INTO warehouse.Customer
    (CreatedDate, FirstName, LastName)
    VALUES (@Now, @FirstName, @LastName)

    SELECT SCOPE_IDENTITY()
END

GO

CREATE PROC warehouse.usp_CheckCustomerName
    @FirstName VARCHAR(100),
    @LastName VARCHAR(100)
AS
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM warehouse.Customer
        WHERE FirstName = @FirstName
        AND LastName = @LastName
    )
    BEGIN
        SELECT 0
    END

    SELECT 1
END

GO

CREATE PROC warehouse.usp_CheckCustomerExists @CustomerId INT
AS
BEGIN
    IF EXISTS
    (
        SELECT 1
        FROM warehouse.Customer
        WHERE CustomerId = @CustomerId
    )
    BEGIN
        SELECT 1
    END

    SELECT 0
END