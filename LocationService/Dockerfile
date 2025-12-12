# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar la solución y la carpeta src completa
COPY ServiceLocation.sln .
COPY src/ ./src/

# Restaurar dependencias usando la solución
RUN dotnet restore ServiceLocation.sln

# Publicar la aplicación Presentation
WORKDIR /src/src/ServiceLocation.Presentation
RUN dotnet publish ServiceLocation.Presentation.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "ServiceLocation.Presentation.dll"]
