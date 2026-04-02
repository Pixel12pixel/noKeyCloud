FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["noKeyCloud_api/noKeyCloud_api.csproj", "noKeyCloud_api/"]
RUN dotnet restore "noKeyCloud_api/noKeyCloud_api.csproj"
COPY . .
WORKDIR "/src/noKeyCloud_api"
RUN dotnet build "./noKeyCloud_api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./noKeyCloud_api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "noKeyCloud_api.dll"]
