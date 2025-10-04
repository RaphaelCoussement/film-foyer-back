# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copier le fichier .csproj et restaurer les dépendances
COPY AppliFilms.Api/*.csproj ./AppliFilms.Api/
WORKDIR /app/AppliFilms.Api
RUN dotnet restore

# Copier tout le code du projet
COPY AppliFilms.Api/. ./

# Publier l'application en Release
RUN dotnet publish -c Release -o out

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/AppliFilms.Api/out .

# Exposer le port pour Render
EXPOSE 80

# Définir l'entrée
ENTRYPOINT ["dotnet", "AppliFilms.Api.dll"]