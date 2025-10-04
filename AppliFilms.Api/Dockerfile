# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copier le .csproj dans un sous-dossier
COPY AppliFilms.Api/*.csproj ./AppliFilms.Api/

# Aller dans le dossier du projet
WORKDIR /app/AppliFilms.Api

# Restaurer les dépendances
RUN dotnet restore

# Copier tout le reste du code
COPY AppliFilms.Api/. ./

# Publier l'application
RUN dotnet publish -c Release -o out

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copier le résultat du publish
COPY --from=build /app/AppliFilms.Api/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "AppliFilms.Api.dll"]