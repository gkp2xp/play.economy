FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./packages ./packages
COPY ["Play.Trading.Service/Play.Trading.Service.csproj", "Play.Trading.Service/"]
RUN dotnet restore "Play.Trading.Service/Play.Trading.Service.csproj" -s ./packages -s https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/Play.Trading.Service"
RUN dotnet build "Play.Trading.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Trading.Service.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8080
EXPOSE 443
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Play.Trading.Service.dll"]