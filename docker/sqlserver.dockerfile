FROM mcr.microsoft.com/mssql/server:2022-latest

ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=YourStrong!Passw0rd

EXPOSE 1433