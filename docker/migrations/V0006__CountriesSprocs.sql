USE Inventory

GO

CREATE PROC dbo.usp_GetCountries
AS
BEGIN
    SELECT CountryId,
           CountryCode,
           CurrencyCode
    FROM dbo.Country
    ORDER BY CountryId
END