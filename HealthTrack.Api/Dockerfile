FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY HealthTrack.Api/HealthTrack.Api.csproj HealthTrack.Api/
COPY HealthTrack.Application/HealthTrack.Application.csproj HealthTrack.Application/
COPY HealthTrack.Infrastructure/HealthTrack.Infrastructure.csproj HealthTrack.Infrastructure/
COPY HealthTrack.Domain/HealthTrack.Domain.csproj HealthTrack.Domain/
COPY HealthTrack.sln ./

RUN dotnet restore HealthTrack.Api/HealthTrack.Api.csproj

COPY . .

RUN dotnet publish HealthTrack.Api/HealthTrack.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HealthTrack.Api.dll"]

