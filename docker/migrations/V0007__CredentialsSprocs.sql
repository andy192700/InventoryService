USE Inventory

GO

CREATE PROC dbo.usp_VerifyCredentials @UserName VARCHAR(50), @PasswordHash NVARCHAR(255)
AS
BEGIN
    IF EXISTS 
    (
        SELECT 1
        FROM dbo.UserCredential
        WHERE UserName = @UserName
        AND PasswordHash = @PasswordHash
    )
    BEGIN
        SELECT 1
    END

    SELECT 0
END