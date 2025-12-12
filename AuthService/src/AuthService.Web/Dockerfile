# Imagen base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Imagen para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos toda la solución
COPY . .

# Entramos directamente al proyecto correcto
WORKDIR /src/src/AuthService.Web

# Restauramos con el csproj correcto
RUN dotnet restore AuthService.Web.csproj

# Publicamos
RUN dotnet publish AuthService.Web.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthService.Web.dll"]
