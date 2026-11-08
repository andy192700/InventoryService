# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ../src/ ./ 

RUN dotnet restore InventoryService/InventoryService.csproj

WORKDIR /src/InventoryService
RUN dotnet publish -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:6000

EXPOSE 6000

ENTRYPOINT ["dotnet", "InventoryService.dll"]