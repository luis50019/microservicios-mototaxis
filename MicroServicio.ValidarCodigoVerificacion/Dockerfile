# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos solo el .csproj primero (mejora cache)
COPY ./*.csproj ./
RUN dotnet restore

# Copiamos todo el microservicio
COPY . .

# Publicamos
RUN dotnet publish -c Release -o /app

# Runtime final
FROM base AS final
WORKDIR /app

COPY --from=build /app .

# Appsettings.json está en la misma carpeta del Dockerfile ✔
COPY ./appsettings.json .

ENTRYPOINT ["dotnet", "$(basename $(pwd)).dll"]
