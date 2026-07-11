FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/FileVault.API/FileVault.API.csproj",           "src/FileVault.API/"]
COPY ["src/FileVault.Application/FileVault.Application.csproj", "src/FileVault.Application/"]
COPY ["src/FileVault.Infrastructure/FileVault.Infrastructure.csproj", "src/FileVault.Infrastructure/"]
COPY ["src/FileVault.Domain/FileVault.Domain.csproj",     "src/FileVault.Domain/"]

RUN dotnet restore "src/FileVault.API/FileVault.API.csproj"

COPY . .
WORKDIR "/src/src/FileVault.API"
RUN dotnet build "FileVault.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileVault.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileVault.API.dll"]
