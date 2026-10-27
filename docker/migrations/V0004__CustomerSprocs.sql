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