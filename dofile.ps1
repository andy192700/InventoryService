using namespace System.Text;
using namespace System.Security.Cryptography;

[string] $sqlServerInstance = "localhost";
[string] $databaseName = "Inventory";
[string] $sqlLoginName = "sa";
[string] $sqlPass = "YourStrong!Passw0rd";
[string] $salt = "6E4F0F00A8F44C31A83D7EFAAA4D6156";

Target DockerUp -inherits DockerDown {
    docker compose up -d;
}

Target DockerDown {
    Push-Location docker;

    docker compose down -v --rmi all;
}

Target DotnetTest {
    Push-Location src;

    dotnet test;
}

Target CreateCredential {
    if ([string]::IsNullOrWhiteSpace($username)) {
        Write-Host "username has not been supplied" -ForegroundColor Red;

        return;
    }

    if ([string]::IsNullOrWhiteSpace($password)) {
        Write-Host "password has not been supplied" -ForegroundColor Red;

        return;
    }

    [byte[]] $bytes = [Encoding]::UTF8.GetBytes("$password$salt");
    [byte[]] $hashBytes = [SHA256]::HashData($bytes);
    [string] $passwordHash = [Convert]::ToHexString($hashBytes);

    [string] $sql = @"
INSERT INTO dbo.[UserCredential]
(
    Username,
    PasswordHash
)
VALUES
(
    '$username',
    '$passwordHash'
);
"@;

    Invoke-Sqlcmd -ServerInstance $sqlServerInstance -Database $databaseName -Username $sqlLoginName -Password $sqlPass -TrustServerCertificate -Query $sql;

    [byte[]] $bytes = [Encoding]::UTF8.GetBytes("$($username):$($passwordHash)");
    [string] $base64 = [Convert]::ToBase64String($bytes);

    Write-Host "Created credential $username";
    Write-Host "Passwordhash: $passwordHash";
    Write-Host "Basic auth header value: $base64";
}