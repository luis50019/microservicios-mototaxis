# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar todo el microservicio al contenedor
COPY . .

# Restaurar dependencias de la solución completa
RUN dotnet restore MicroServicio.WebSocketReservacion.sln

# Publicar el proyecto de Presentation
RUN dotnet publish src/ServiceReservation.Presentation/ServiceReservation.Presentation.csproj -c Release -o /app/publish

# Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ServiceReservation.Presentation.dll"]
