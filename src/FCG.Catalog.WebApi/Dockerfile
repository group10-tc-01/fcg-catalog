FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copiar os projetos usando o caminho relativo à raiz (src/NomeDoProjeto)
COPY ["src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj", "src/FCG.Catalog.WebApi/"]
COPY ["src/FCG.Catalog.Application/FCG.Catalog.Application.csproj", "src/FCG.Catalog.Application/"]
COPY ["src/FCG.Catalog.Domain/FCG.Catalog.Domain.csproj", "src/FCG.Catalog.Domain/"]
COPY ["src/FCG.Catalog.Infrastructure.SqlServer/FCG.Catalog.Infrastructure.SqlServer.csproj", "src/FCG.Catalog.Infrastructure.SqlServer/"]
COPY ["src/FCG.Catalog.Messages/FCG.Catalog.Messages.csproj", "src/FCG.Catalog.Messages/"]

# 2. Restaurar dependências apontando para o arquivo principal
RUN dotnet restore "src/FCG.Catalog.WebApi/FCG.Catalog.WebApi.csproj"

# 3. Copiar todo o código fonte
COPY . .

# 4. Ajustar o diretório de trabalho para o build
# OBS: Como copiamos a raiz (.) para dentro de /src, o caminho dobra (/src/src/...)
WORKDIR "/src/src/FCG.Catalog.WebApi"

# 5. Build e Publish
RUN dotnet build "FCG.Catalog.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG.Catalog.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG.Catalog.WebApi.dll"]