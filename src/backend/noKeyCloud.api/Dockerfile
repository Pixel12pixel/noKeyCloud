FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/backend/noKeyCloud.api/noKeyCloud.api.csproj", "src/backend/noKeyCloud.api/"]
COPY ["src/backend/noKeyCloud.Application/noKeyCloud.Application.csproj", "src/backend/noKeyCloud.Application/"]
COPY ["src/backend/noKeyCloud.Domain/noKeyCloud.Domain.csproj", "src/backend/noKeyCloud.Domain/"]
COPY ["src/backend/noKeyCloud.Infrastructure/noKeyCloud.Infrastructure.csproj", "src/backend/noKeyCloud.Infrastructure/"]
COPY ["src/shared/noKeyCloud.Contracts/noKeyCloud.Contracts.csproj", "src/shared/noKeyCloud.Contracts/"]

RUN dotnet restore "src/backend/noKeyCloud.api/noKeyCloud.api.csproj"

COPY . .

WORKDIR "/src/src/backend/noKeyCloud.api"
RUN dotnet build "noKeyCloud.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "noKeyCloud.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "noKeyCloud.api.dll"]
