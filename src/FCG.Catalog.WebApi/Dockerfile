FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
<<<<<<< HEAD

COPY ["src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj", "src/FCG.Catalog.WebApi/"]
COPY ["src/FCG.Catalog.Application/FCG.Catalog.Application.csproj", "src/FCG.Catalog.Application/"]
COPY ["src/FCG.Catalog.Domain/FCG.Catalog.Domain.csproj", "src/FCG.Catalog.Domain/"]
COPY ["src/FCG.Catalog.Infrastructure.SqlServer/FCG.Catalog.Infrastructure.SqlServer.csproj", "src/FCG.Catalog.Infrastructure.SqlServer/"]
COPY ["src/FCG.Catalog.Messages/FCG.Catalog.Messages.csproj", "src/FCG.Catalog.Messages/"]

RUN dotnet restore "src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj"

COPY . .

WORKDIR "/src/src/FCG.Catalog.WebApi"

RUN dotnet build "FCG.Catalog.WebApi.csproj" -c Release -o /app/build
=======
COPY ["src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj", "src/FCG.Catalog.WebApi/"]
RUN dotnet restore "./src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj"
COPY . .
WORKDIR "/src/src/FCG.Catalog.WebApi"
RUN dotnet build "./FCG.Catalog.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
>>>>>>> main

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FCG.Catalog.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Catalog.WebApi.dll"]