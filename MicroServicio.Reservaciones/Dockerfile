# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solo el csproj para restaurar dependencias
COPY ./MicroServicio.Reservaciones.csproj ./

# Restaurar paquetes
RUN dotnet restore ./MicroServicio.Reservaciones.csproj

# Copiar todo el código fuente
COPY . .

# Publicar la app en Release
RUN dotnet publish ./MicroServicio.Reservaciones.csproj -c Release -o /app

# ---------------------------
# Etapa runtime
# ---------------------------
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

# Copiar la app publicada
COPY --from=build /app .

# Copiar appsettings.json al contenedor
COPY ./appsettings.json .

# ENTRYPOINT
ENTRYPOINT ["dotnet", "MicroServicio.Reservaciones.dll"]
