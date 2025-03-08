FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./packages ./packages
COPY ["Play.Identity.Service/Play.Identity.Service.csproj", "Play.Identity.Service/"]
COPY ["Play.Identity.Contracts/Play.Identity.Contracts.csproj", "Play.Identity.Contracts/"]
RUN dotnet restore "Play.Identity.Service/Play.Identity.Service.csproj" -s ./packages -s https://api.nuget.org/v3/index.json
COPY . .
WORKDIR "/src/Play.Identity.Service"
RUN dotnet build "Play.Identity.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Play.Identity.Service.csproj" -c Release -o /app/publish

FROM base AS final
EXPOSE 8080
EXPOSE 443
WORKDIR /app
RUN mkdir /app/keys && chmod 777 /app/keys  # Ensure the keys directory is writable
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Play.Identity.Service.dll"]