FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GameBackend.API/GameBackend.API.csproj", "GameBackend.API/"]
COPY ["GameBackend.Application/GameBackend.Application.csproj", "GameBackend.Application/"]
COPY ["GameBackend.Core/GameBackend.Core.csproj", "GameBackend.Core/"]
COPY ["GameBackend.Infrastructure/GameBackend.Infrastructure.csproj", "GameBackend.Infrastructure/"]

RUN dotnet restore "GameBackend.API/GameBackend.API.csproj"

COPY . .
WORKDIR "/src/GameBackend.API"
RUN dotnet build "GameBackend.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameBackend.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameBackend.API.dll"]