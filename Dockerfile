# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src

COPY ["Interview.API/Interview.API.csproj", "Interview.API/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Interview.Application/Interview.Application.csproj", "Interview.Application/"]
COPY ["Interview.Domain/Interview.Domain.csproj", "Interview.Domain/"]
RUN dotnet restore "Interview.API/Interview.API.csproj"

COPY . .
WORKDIR /src/Interview.API
RUN dotnet publish "Interview.API.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS final
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=8080 \
    HttpsRedirection__Enabled=false \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080
USER $APP_UID

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Interview.API.dll"]
