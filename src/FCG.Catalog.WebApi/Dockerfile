FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj", "src/FCG.Catalog.WebApi/"]
RUN dotnet restore "./src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj"
COPY . .
WORKDIR "/src/src/FCG.Catalog.WebApi"
RUN dotnet build "./FCG.Catalog.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FCG.Catalog.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Catalog.WebApi.dll"]