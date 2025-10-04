# Étape 1 : Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier csproj et restaurer
COPY *.sln .
COPY AppliFilms.Api/*.csproj ./AppliFilms.Api/
RUN dotnet restore

# Copier le reste du code et builder
COPY . .
WORKDIR /src/AppliFilms.Api
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : Runtime (image plus légère)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Définir le port (Render écoutera sur 10000 par défaut)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "AppliFilms.Api.dll"]
