USE Inventory

GO

CREATE PROC warehouse.usp_GetCountries
AS
BEGIN
    SET NOCOUNT ON

    SELECT CountryId,
           CountryCode,
           CurrencyCode
    FROM warehouse.Country
    ORDER BY CountryId
END