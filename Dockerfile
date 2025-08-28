# Base image com runtime do .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Expõe a porta padrão, mas Railway injeta PORT dinamicamente
EXPOSE 8080

# Imagem de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia csproj primeiro para otimizar cache
COPY ["./API.Lanchonete/API.Lanchonete.csproj", "API.Lanchonete/"]
COPY ["./API.Lanchonete.Data/API.Lanchonete.Data.csproj", "API.Lanchonete.Data/"]
COPY ["./API.Lanchonete.Core/API.Lanchonete.Core.csproj", "API.Lanchonete.Core/"]
COPY ["./API.Lanchonete.Domain/API.Lanchonete.Domain.csproj", "API.Lanchonete.Domain/"]
COPY ["./API.Lanchonete.IoC/API.Lanchonete.IoC.csproj", "API.Lanchonete.IoC/"]
COPY ["./API.Lanchonete.Business/API.Lanchonete.Business.csproj", "API.Lanchonete.Business/"]

RUN dotnet restore "API.Lanchonete/API.Lanchonete.csproj"

# Copia todo o código
COPY . .
WORKDIR "/src/API.Lanchonete"
RUN dotnet build "API.Lanchonete.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.Lanchonete.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configura o ASP.NET Core para usar a porta do Railway
ENV ASPNETCORE_URLS=http://+:${PORT}

ENTRYPOINT ["dotnet", "API.Lanchonete.dll"]
