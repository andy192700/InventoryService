USE Inventory

GO

CREATE PROC governance.usp_GetCredentials @UserName VARCHAR(50), @PasswordHash NVARCHAR(255)
AS
BEGIN   
    SELECT uc.UserName
    FROM governance.UserCredential uc
    WHERE uc.UserName = @UserName
    AND uc.PasswordHash = @PasswordHash

    SELECT up.PermissionName
    FROM governance.UserCredential uc
    JOIN governance.UserCredentialPermission ucp
    ON ucp.UserCredentialId = uc.UserCredentialId
    JOIN governance.UserPermission up
    ON up.UserPermissionId = ucp.UserPermissionId
    WHERE uc.UserName = @UserName
END