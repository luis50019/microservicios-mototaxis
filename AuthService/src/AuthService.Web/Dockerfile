# Usamos SDK para build y restore
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Directorio de trabajo dentro del contenedor
WORKDIR /src

# Copiamos todo el contenido del proyecto para tener todos los proyectos referenciados
COPY . .

# Restauramos todas las dependencias de la solución
RUN dotnet restore AuthService.sln

# Publicamos el proyecto web en Release
RUN dotnet publish src/AuthService.Web/AuthService.Web.csproj -c Release -o /app/publish

# Imagen runtime para ejecutar la app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copiamos lo publicado desde la etapa de build
COPY --from=build /app/publish .

# Puerto que exponemos (ajusta si es necesario)
EXPOSE 5000

# Comando para ejecutar la app
ENTRYPOINT ["dotnet", "AuthService.Web.dll"]
