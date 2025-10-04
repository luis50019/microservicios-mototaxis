# ---------------------------
# Etapa de build
# ---------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el csproj del microservicio
COPY ./MicroServicio.CodigoVerificacion.csproj ./

# Restaurar dependencias
RUN dotnet restore ./MicroServicio.CodigoVerificacion.csproj

# Copiar todo el código
COPY . .

# Publicar la app en Release
RUN dotnet publish ./MicroServicio.CodigoVerificacion.csproj -c Release -o /app

# ---------------------------
# Etapa runtime
# ---------------------------
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app

# Copiar la app publicada
COPY --from=build /app .

# Copiar appsettings.json
COPY ./appsettings.json .

# ENTRYPOINT
ENTRYPOINT ["dotnet", "MicroServicio.CodigoVerificacion.dll"]
