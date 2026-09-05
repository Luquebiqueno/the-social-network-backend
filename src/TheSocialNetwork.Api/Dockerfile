FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

RUN apt-get update \
    && apt-get install --no-install-recommends --yes curl \
    && rm -rf /var/lib/apt/lists/*

HEALTHCHECK --interval=30s --timeout=5s --start-period=20s --retries=3 \
    CMD curl --fail http://localhost:8080/health/ready || exit 1

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/TheSocialNetwork.Api/TheSocialNetwork.Api.csproj", "src/TheSocialNetwork.Api/"]
RUN dotnet restore "src/TheSocialNetwork.Api/TheSocialNetwork.Api.csproj"
COPY . .
WORKDIR "/src/src/TheSocialNetwork.Api"
RUN dotnet build "TheSocialNetwork.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "TheSocialNetwork.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheSocialNetwork.Api.dll"]
