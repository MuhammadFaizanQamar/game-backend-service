FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src

COPY . .

RUN dotnet restore "GameBackend.API/GameBackend.API.csproj"
RUN dotnet publish "GameBackend.API/GameBackend.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-stage
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
COPY --from=build-stage /app/publish .
ENTRYPOINT ["dotnet", "GameBackend.API.dll"]