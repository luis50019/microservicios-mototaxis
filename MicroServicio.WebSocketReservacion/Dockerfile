FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar toda la carpeta del microservicio
COPY . .

# Restauramos el proyecto de presentación
RUN dotnet restore src/ServiceReservation.Presentation/ServiceReservation.Presentation.csproj

# Compilamos y publicamos
RUN dotnet publish src/ServiceReservation.Presentation/ServiceReservation.Presentation.csproj -c Release -o /app/publish

# Runtime final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ServiceReservation.Presentation.dll"]
