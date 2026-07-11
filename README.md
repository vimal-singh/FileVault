# FileVault

FileVault is a .NET 8 modular file-management solution with a clean architecture layout.

## Overview

This repository includes:
- Domain layer with the FileAggregate and supporting value objects
- Application layer with upload and retrieval use cases
- Infrastructure layer with EF Core persistence and repository implementations
- API layer exposing file endpoints

## Project Structure

- src/FileVault.API – ASP.NET Core Web API
- src/FileVault.Application – application services and use cases
- src/FileVault.Domain – domain model and business rules
- src/FileVault.Infrastructure – EF Core persistence and infrastructure services
- tests – unit, integration, and architecture tests

## Prerequisites

- .NET 8 SDK
- Docker Desktop (for SQL Server and supporting services)

## Running locally

1. Start the local dependencies:
   ```bash
   docker compose -f docker/docker-compose.yml up -d
   ```

2. Run the API:
   ```bash
   dotnet run --project src/FileVault.API/FileVault.API.csproj
   ```

3. Open the Swagger UI:
   - http://localhost:5160/swagger

## Build

```bash
dotnet build FileVault.sln
```

## Tests

```bash
dotnet test tests/FileVault.UnitTests/FileVault.UnitTests.csproj
```

## Notes

The current persistence setup is wired to a local SQL Server container defined in the Docker Compose configuration.
