# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Imagen de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos TODA la solución
COPY . .

# Restauramos desde el .sln
RUN dotnet restore AuthService.sln

# Publicamos el proyecto principal
WORKDIR /src/src/AuthService.Web
RUN dotnet publish AuthService.Web.csproj -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Web.dll"]
