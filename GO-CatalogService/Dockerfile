# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /app

# Copy project and solution files
COPY . .

COPY nuget.config ./
COPY nuget-packages ./nuget-packages

# Restore using custom nuget.config
RUN dotnet restore "GO-CatalogService/GO-CatalogService.csproj" --configfile ./nuget.config

# Publish
RUN dotnet build GO-CatalogService.csproj -c Release -o /app/build
RUN dotnet publish GO-CatalogService.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "/app/GO-CatalogService.dll"]