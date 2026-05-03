# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "GuichetAutomatique.Api/GuichetAutomatique.Api.csproj"
RUN dotnet publish "GuichetAutomatique.Api/GuichetAutomatique.Api.csproj" -c Release -o /app/publish

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "GuichetAutomatique.Api.dll"]