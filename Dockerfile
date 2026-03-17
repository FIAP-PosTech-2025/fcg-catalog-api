# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Catalog.sln ./
COPY Catalog.Api/Catalog.Api.csproj Catalog.Api/
COPY Catalog.Application/Catalog.Application.csproj Catalog.Application/
COPY Catalog.Domain/Catalog.Domain.csproj Catalog.Domain/
COPY Catalog.Infra/Catalog.Infra.csproj Catalog.Infra/
RUN dotnet restore Catalog.Api/Catalog.Api.csproj

COPY . .
RUN dotnet publish Catalog.Api/Catalog.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5001
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/publish .

EXPOSE 5001
ENTRYPOINT ["dotnet", "Catalog.Api.dll"]
