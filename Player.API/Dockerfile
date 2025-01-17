FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Player.API/Player.API.csproj", "Player.API/"]
COPY ["Player.Application/Player.Application.csproj", "Player.Application/"]
COPY ["Player.Domain/Player.Domain.csproj", "Player.Domain/"]
COPY ["Player.Infrastructure.Persistence/Player.Infrastructure.Persistence.csproj", "Player.Infrastructure.Persistence/"]
RUN dotnet restore -a $TARGETARCH "Player.API/Player.API.csproj"

COPY . .
WORKDIR "/src/Player.API"
RUN dotnet build "Player.API.csproj" --no-restore -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Player.API.csproj" -c $BUILD_CONFIGURATION -a $TARGETARCH -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Player.API.dll"]
