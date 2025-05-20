# Base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj og nuget konfiguration
COPY nuget.config ./
COPY nuget-packages ./nuget-packages
COPY GO-CatalogService/GO-CatalogService.csproj GO-CatalogService/

# Restore using local NuGet source
RUN dotnet restore "GO-CatalogService/GO-CatalogService.csproj" --configfile ./nuget.config

# Copy the rest of the code
COPY . .

# Build og publish
WORKDIR /src/GO-CatalogService
RUN dotnet build GO-CatalogService.csproj -c Release -o /app/build
RUN dotnet publish GO-CatalogService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GO-CatalogService.dll"]
