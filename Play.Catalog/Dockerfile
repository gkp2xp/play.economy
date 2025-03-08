FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./packages ./packages
COPY ["Play.Catalog.Service/Play.Catalog.Service.csproj", "Play.Catalog.Service/"]
COPY ["Play.Catalog.Contracts/Play.Catalog.Contracts.csproj", "Play.Catalog.Contracts/"]
RUN dotnet restore "Play.Catalog.Service/Play.Catalog.Service.csproj" -s ./packages -s https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/Play.Catalog.Service"
RUN dotnet build "Play.Catalog.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Catalog.Service.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8080
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Play.Catalog.Service.dll"]