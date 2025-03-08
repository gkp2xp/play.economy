FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./packages ./packages
COPY ["Play.Inventory.Service/Play.Inventory.Service.csproj", "Play.Inventory.Service/"]
COPY ["Play.Inventory.Contracts/Play.Inventory.Contracts.csproj", "Play.Inventory.Contracts/"]
RUN dotnet restore "Play.Inventory.Service/Play.Inventory.Service.csproj" -s ./packages -s https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/Play.Inventory.Service"
RUN dotnet build "Play.Inventory.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Inventory.Service.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8080
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Play.Inventory.Service.dll"]