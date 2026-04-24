# Getting Started

<cite>
**Referenced Files in This Document**
- [global.json](file://global.json)
- [GameBackend.API.csproj](file://GameBackend.API/GameBackend.API.csproj)
- [Program.cs](file://GameBackend.API/Program.cs)
- [Startup.cs](file://GameBackend.API/Startup.cs)
- [appsettings.json](file://GameBackend.API/appsettings.json)
- [appsettings.Development.json](file://GameBackend.API/appsettings.Development.json)
- [launchSettings.json](file://GameBackend.API/Properties/launchSettings.json)
- [GameDbContext.cs](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs)
- [JwtSettings.cs](file://GameBackend.Infrastructure/Security/JwtSettings.cs)
- [PlayerRepository.cs](file://GameBackend.Infrastructure/Repositories/PlayerRepository.cs)
- [Player.cs](file://GameBackend.Core/Entities/Player.cs)
</cite>

## Update Summary
**Changes Made**
- Updated architecture overview to reflect the new Startup.cs pattern
- Modified Program.cs delegation explanation to show structured startup approach
- Updated service configuration and middleware pipeline documentation
- Revised diagrams to show Program.cs delegating to Startup.cs
- Enhanced authentication and middleware setup documentation

## Table of Contents
1. [Introduction](#introduction)
2. [Project Structure](#project-structure)
3. [Core Components](#core-components)
4. [Architecture Overview](#architecture-overview)
5. [Detailed Component Analysis](#detailed-component-analysis)
6. [Dependency Analysis](#dependency-analysis)
7. [Performance Considerations](#performance-considerations)
8. [Troubleshooting Guide](#troubleshooting-guide)
9. [Conclusion](#conclusion)
10. [Appendices](#appendices)

## Introduction
This guide helps you install, configure, and run the GameBackend project locally. It covers prerequisites, environment setup, database configuration with PostgreSQL, JWT settings, and initial compilation and testing. By the end, you will have a working development server with Swagger UI, a configured database connection, and secure JWT authentication ready for use.

**Updated** The project now uses a structured Startup.cs pattern for application configuration, improving separation of concerns and maintainability.

## Project Structure
The solution consists of layered projects with a structured startup pattern:
- GameBackend.API: ASP.NET Core web host with structured startup and HTTP entrypoint
- GameBackend.Application: Application use cases and contracts
- GameBackend.Core: Domain entities and interfaces
- GameBackend.Infrastructure: Persistence, repositories, and security implementations
- GameBackend.Tests: Test project (no extra setup required beyond building)

```mermaid
graph TB
subgraph "Web Host with Structured Startup"
API["GameBackend.API<br/>Program.cs delegates to Startup.cs"]
STARTUP["Startup.cs<br/>Configures Services & Middleware"]
end
subgraph "Application Layer"
APP["GameBackend.Application"]
ENDPOINT["AuthController<br/>PlayerController"]
end
subgraph "Domain Layer"
CORE["GameBackend.Core"]
PLAYER["Player Entity"]
end
subgraph "Infrastructure Layer"
INFRA["GameBackend.Infrastructure"]
DBCTX["GameDbContext"]
REPO["PlayerRepository"]
SEC["JwtSettings<br/>JwtTokenGenerator<br/>PasswordHasher"]
ENDPOINT --> STARTUP
STARTUP --> APP
STARTUP --> INFRA
APP --> CORE
INFRA --> CORE
INFRA --> DBCTX
INFRA --> REPO
INFRA --> SEC
```

**Diagram sources**
- [Program.cs:1-16](file://GameBackend.API/Program.cs#L1-L16)
- [Startup.cs:15-117](file://GameBackend.API/Startup.cs#L15-L117)
- [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)
- [GameDbContext.cs:1-28](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs#L1-L28)
- [PlayerRepository.cs:1-46](file://GameBackend.Infrastructure/Repositories/PlayerRepository.cs#L1-L46)
- [JwtSettings.cs:1-8](file://GameBackend.Infrastructure/Security/JwtSettings.cs#L1-L8)
- [Player.cs:1-13](file://GameBackend.Core/Entities/Player.cs#L1-L13)

**Section sources**
- [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)
- [Program.cs:1-16](file://GameBackend.API/Program.cs#L1-L16)
- [Startup.cs:15-117](file://GameBackend.API/Startup.cs#L15-L117)

## Core Components
- .NET 8.0 SDK: Required for building and running the project.
- PostgreSQL: Used via Entity Framework Core provider for persistence.
- JWT Authentication: Configured with issuer, audience, and symmetric key.
- Swagger/OpenAPI: Enabled in development for API exploration.
- Structured Startup Pattern: Program.cs delegates to Startup.cs for configuration.

Key configuration locations:
- SDK version and roll-forward policy in [global.json:1-7](file://global.json#L1-L7)
- Target framework and package references in [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)
- JWT settings and connection string in [appsettings.json:2-9](file://GameBackend.API/appsettings.json#L2-L9)
- Development logging overrides in [appsettings.Development.json:1-9](file://GameBackend.API/appsettings.Development.json#L1-L9)
- Program.cs delegation to Startup.cs in [Program.cs:10-15](file://GameBackend.API/Program.cs#L10-L15)
- Comprehensive service configuration in [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)

**Section sources**
- [global.json:1-7](file://global.json#L1-L7)
- [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)
- [appsettings.json:1-17](file://GameBackend.API/appsettings.json#L1-L17)
- [appsettings.Development.json:1-9](file://GameBackend.API/appsettings.Development.json#L1-L9)
- [Program.cs:10-15](file://GameBackend.API/Program.cs#L10-L15)
- [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)

## Architecture Overview
The runtime startup follows a structured pattern where Program.cs delegates to Startup.cs for comprehensive configuration. Startup.cs handles dependency injection registration, database context configuration, authentication setup, CORS policies, and middleware pipeline construction.

```mermaid
sequenceDiagram
participant Dev as "Developer"
participant Host as "Program.cs"
participant Start as "Startup.cs"
participant Cfg as "appsettings.json"
participant Db as "GameDbContext"
participant Repo as "PlayerRepository"
participant Sec as "JwtSettings/JwtTokenGenerator"
Dev->>Host : dotnet run (launchSettings.json)
Host->>Start : CreateHostBuilder(args).UseStartup<Startup>()
Start->>Cfg : Load "Jwt" and "ConnectionStrings"
Start->>Db : AddDbContext(...) with Npgsql
Start->>Repo : Register IPlayerRepository
Start->>Sec : Configure JwtSettings and AddJwtBearer
Start->>Start : Configure CORS, Controllers, Swagger
Start->>Host : UseAuthentication/UseAuthorization/UseEndpoints
Host-->>Dev : HTTP endpoints available (Swagger)
```

**Diagram sources**
- [Program.cs:10-15](file://GameBackend.API/Program.cs#L10-L15)
- [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)
- [appsettings.json:2-9](file://GameBackend.API/appsettings.json#L2-L9)
- [GameDbContext.cs:6-11](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs#L6-L11)
- [PlayerRepository.cs:8-15](file://GameBackend.Infrastructure/Repositories/PlayerRepository.cs#L8-L15)
- [JwtSettings.cs:3-8](file://GameBackend.Infrastructure/Security/JwtSettings.cs#L3-L8)
- [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31)

## Detailed Component Analysis

### Prerequisites and Installation
- Install .NET 8.0 SDK. The repository enforces this version and minor roll-forward behavior.
- Verify your installation matches the SDK requirement defined in [global.json:1-7](file://global.json#L1-L7).
- Build the solution to restore packages and compile all projects.

What to expect after build:
- Projects resolve NuGet dependencies including ASP.NET Core, JWT bearer, OpenAPI/Swagger, and Npgsql EF provider.
- The API project targets net8.0 as declared in [GameBackend.API.csproj](file://GameBackend.API/GameBackend.API.csproj#L4).

Verification steps:
- Run the API project using the configured launch profiles in [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31).
- Confirm Swagger UI loads at the configured URLs.

**Section sources**
- [global.json:1-7](file://global.json#L1-L7)
- [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)
- [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31)

### Environment Setup
- Environment variables: The launch settings set ASPNETCORE_ENVIRONMENT to Development for all profiles.
- Logging: Development overrides reduce noise from ASP.NET Core categories in [appsettings.Development.json:2-6](file://GameBackend.API/appsettings.Development.json#L2-L6).

Recommended approach:
- Keep Development profile active during local setup.
- Optionally override secrets via environment variables or user secrets in Development.

**Section sources**
- [launchSettings.json:18-30](file://GameBackend.API/Properties/launchSettings.json#L18-L30)
- [appsettings.Development.json:1-9](file://GameBackend.API/appsettings.Development.json#L1-L9)

### Database Configuration with PostgreSQL
- Provider: Npgsql EF provider is referenced in the API project.
- Connection string: Defined under "ConnectionStrings:DefaultConnection" in [appsettings.json:7-9](file://GameBackend.API/appsettings.json#L7-L9).
- Model: The Player entity is mapped with unique indexes for Email and Username in [GameDbContext.cs:19-26](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs#L19-L26).
- Repository: Accesses Players DbSet via [PlayerRepository.cs:8-33](file://GameBackend.Infrastructure/Repositories/PlayerRepository.cs#L8-L33).

Setup checklist:
- Ensure PostgreSQL is installed and running.
- Create a database named gamebackend (or update the connection string accordingly).
- Confirm the connection string resolves to a reachable host/port with valid credentials.
- Seed minimal data if needed for testing.

```mermaid
flowchart TD
Start(["Startup"]) --> ReadCfg["Read 'DefaultConnection' from appsettings.json"]
ReadCfg --> UseEF["Configure DbContext with Npgsql"]
UseEF --> EnsureDb["Ensure database exists and migrations applied"]
EnsureDb --> Ready(["API Ready"])
```

**Diagram sources**
- [appsettings.json:7-9](file://GameBackend.API/appsettings.json#L7-L9)
- [Startup.cs:27-28](file://GameBackend.API/Startup.cs#L27-L28)
- [GameDbContext.cs:6-11](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs#L6-L11)

**Section sources**
- [GameBackend.API.csproj:10-13](file://GameBackend.API/GameBackend.API.csproj#L10-L13)
- [appsettings.json:7-9](file://GameBackend.API/appsettings.json#L7-L9)
- [Startup.cs:27-28](file://GameBackend.API/Startup.cs#L27-L28)
- [GameDbContext.cs:19-26](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs#L19-L26)
- [PlayerRepository.cs:17-33](file://GameBackend.Infrastructure/Repositories/PlayerRepository.cs#L17-L33)

### JWT Settings Configuration
- Settings model: JwtSettings holds Key, Issuer, and Audience in [JwtSettings.cs:3-8](file://GameBackend.Infrastructure/Security/JwtSettings.cs#L3-L8).
- Configuration binding: The API binds "Jwt" section to JwtSettings in [Startup.cs:36](file://GameBackend.API/Startup.cs#L36).
- Validation parameters: Issuer, audience, and signing key are configured in [Startup.cs:52-66](file://GameBackend.API/Startup.cs#L52-L66).
- Current defaults: Key, Issuer, and Audience are present in [appsettings.json:2-6](file://GameBackend.API/appsettings.json#L2-L6).

Recommendations:
- Replace the default Key with a strong, random secret in production.
- Align Issuer and Audience with your identity provider and client audiences.
- Store sensitive keys using environment variables or secret stores in Development.

```mermaid
classDiagram
class JwtSettings {
+string Key
+string Issuer
+string Audience
}
class Startup_cs {
+ConfigureServices() registers JwtSettings
+ConfigureJwtBearer() sets validation params
}
Startup_cs --> JwtSettings : "binds 'Jwt' section"
```

**Diagram sources**
- [JwtSettings.cs:3-8](file://GameBackend.Infrastructure/Security/JwtSettings.cs#L3-L8)
- [Startup.cs:36](file://GameBackend.API/Startup.cs#L36)
- [Startup.cs:52-66](file://GameBackend.API/Startup.cs#L52-L66)

**Section sources**
- [JwtSettings.cs:1-8](file://GameBackend.Infrastructure/Security/JwtSettings.cs#L1-L8)
- [Startup.cs:36](file://GameBackend.API/Startup.cs#L36)
- [Startup.cs:52-66](file://GameBackend.API/Startup.cs#L52-L66)
- [appsettings.json:2-6](file://GameBackend.API/appsettings.json#L2-L6)

### Structured Startup Pattern Implementation
The application now uses a structured startup pattern where Program.cs delegates configuration responsibilities to Startup.cs:

**Program.cs Responsibilities:**
- Host builder creation and web host configuration
- Delegation to Startup.cs for service and middleware configuration

**Startup.cs Responsibilities:**
- Service registration in ConfigureServices()
- Middleware pipeline configuration in Configure()
- Authentication, authorization, and CORS setup
- Controller and JSON configuration

Key implementation details:
- Program.cs delegates to Startup.cs using [Program.cs:14](file://GameBackend.API/Program.cs#L14)
- Startup.cs ConfigureServices() registers all dependencies in [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)
- Startup.cs Configure() sets up middleware pipeline in [Startup.cs:95-116](file://GameBackend.API/Startup.cs#L95-L116)

**Section sources**
- [Program.cs:10-15](file://GameBackend.API/Program.cs#L10-L15)
- [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)
- [Startup.cs:95-116](file://GameBackend.API/Startup.cs#L95-L116)

### Initial Compilation and First-Time Setup
Steps:
1. Restore and build the solution to fetch dependencies.
2. Ensure PostgreSQL is running and reachable.
3. Confirm the connection string in [appsettings.json:7-9](file://GameBackend.API/appsettings.json#L7-L9) matches your environment.
4. Run the API project using the Development profile from [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31).
5. Navigate to the Swagger UI endpoint exposed by the Development profile.

Verification:
- Swagger UI loads and displays controllers.
- No authentication errors appear at startup.
- Database context initializes without connection failures.

**Section sources**
- [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31)
- [Startup.cs:95-116](file://GameBackend.API/Startup.cs#L95-L116)
- [appsettings.json:1-17](file://GameBackend.API/appsettings.json#L1-L17)

### First-Time Testing Procedures
- Use Swagger UI to test authentication endpoints after the API starts.
- Register a new player and log in to receive a JWT token.
- Call protected endpoints with the Authorization header using the Bearer token.
- Confirm that token validation succeeds with the configured issuer, audience, and key.

**Section sources**
- [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)
- [Startup.cs:95-116](file://GameBackend.API/Startup.cs#L95-L116)

## Dependency Analysis
High-level dependencies remain the same with improved separation of concerns:
- GameBackend.API depends on Application and Infrastructure projects.
- Infrastructure depends on Core for entities and interfaces.
- Program.cs delegates configuration to Startup.cs for better organization.

```mermaid
graph LR
API["GameBackend.API.csproj"] --> APP["GameBackend.Application.csproj"]
API --> INF["GameBackend.Infrastructure.csproj"]
INF --> CORE["GameBackend.Core.csproj"]
Program_cs["Program.cs"] --> Startup_cs["Startup.cs"]
Startup_cs --> API
```

**Diagram sources**
- [GameBackend.API.csproj:20-23](file://GameBackend.API/GameBackend.API.csproj#L20-L23)
- [Program.cs:14](file://GameBackend.API/Program.cs#L14)
- [Startup.cs:15-117](file://GameBackend.API/Startup.cs#L15-L117)

**Section sources**
- [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)

## Performance Considerations
- Keep Development logging reasonable; the defaults reduce ASP.NET Core noise in [appsettings.Development.json:2-6](file://GameBackend.API/appsettings.Development.json#L2-L6).
- Use HTTPS in development for realistic auth behavior as configured in [launchSettings.json:22-31](file://GameBackend.API/Properties/launchSettings.json#L22-L31).
- The structured startup pattern improves maintainability and reduces startup complexity.

## Troubleshooting Guide
Common setup issues and resolutions:

- .NET SDK mismatch
  - Symptom: Build fails with incompatible SDK.
  - Fix: Install .NET 8.0 SDK as required by [global.json:1-7](file://global.json#L1-L7).

- PostgreSQL connection failure
  - Symptom: Startup throws a database connection error.
  - Fix: Verify the connection string in [appsettings.json:7-9](file://GameBackend.API/appsettings.json#L7-L9) and ensure PostgreSQL is running and reachable.

- JWT validation errors
  - Symptom: Unauthorized responses due to issuer/audience/key mismatches.
  - Fix: Confirm "Jwt" settings in [appsettings.json:2-6](file://GameBackend.API/appsettings.json#L2-L6) match the validation parameters in [Startup.cs:52-66](file://GameBackend.API/Startup.cs#L52-L66).

- Swagger not loading
  - Symptom: Swagger UI not available in Development.
  - Fix: Ensure Development profile is active and Swagger is enabled in [Startup.cs:95-116](file://GameBackend.API/Startup.cs#L95-L116) and [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31).

- Missing Npgsql provider
  - Symptom: EF provider not found errors.
  - Fix: The API project references the provider in [GameBackend.API.csproj:10-13](file://GameBackend.API/GameBackend.API.csproj#L10-L13); rebuild to restore packages.

- Startup.cs not found
  - Symptom: Build fails with Startup.cs not found.
  - Fix: Ensure Startup.cs exists in the GameBackend.API project and Program.cs references it correctly in [Program.cs:14](file://GameBackend.API/Program.cs#L14).

**Section sources**
- [global.json:1-7](file://global.json#L1-L7)
- [appsettings.json:2-9](file://GameBackend.API/appsettings.json#L2-L9)
- [Startup.cs:52-66](file://GameBackend.API/Startup.cs#L52-L66)
- [Startup.cs:95-116](file://GameBackend.API/Startup.cs#L95-L116)
- [launchSettings.json:11-31](file://GameBackend.API/Properties/launchSettings.json#L11-L31)
- [GameBackend.API.csproj:10-13](file://GameBackend.API/GameBackend.API.csproj#L10-L13)
- [Program.cs:14](file://GameBackend.API/Program.cs#L14)

## Conclusion
You now have the prerequisites, configuration references, and step-by-step instructions to install, configure, and run GameBackend locally. With PostgreSQL available, a valid connection string, and properly bound JWT settings, the API will start with Swagger UI and JWT authentication configured through the structured Startup.cs pattern. The refactored architecture improves maintainability while preserving all existing functionality. Proceed to register and authenticate users, then test protected endpoints using the generated tokens.

## Appendices

### Appendix A: Configuration Reference Summary
- SDK version and roll-forward: [global.json:1-7](file://global.json#L1-L7)
- Target framework and packages: [GameBackend.API.csproj:1-26](file://GameBackend.API/GameBackend.API.csproj#L1-L26)
- JWT settings: [appsettings.json:2-6](file://GameBackend.API/appsettings.json#L2-L6), [JwtSettings.cs:3-8](file://GameBackend.Infrastructure/Security/JwtSettings.cs#L3-L8)
- Connection string: [appsettings.json:7-9](file://GameBackend.API/appsettings.json#L7-L9)
- Development logging: [appsettings.Development.json:1-9](file://GameBackend.API/appsettings.Development.json#L1-L9)
- Program.cs delegation: [Program.cs:10-15](file://GameBackend.API/Program.cs#L10-L15)
- Startup.cs services: [Startup.cs:24-93](file://GameBackend.API/Startup.cs#L24-L93)
- Entity model and indexes: [GameDbContext.cs:19-26](file://GameBackend.Infrastructure/Persistence/GameDbContext.cs#L19-L26), [Player.cs:3-12](file://GameBackend.Core/Entities/Player.cs#L3-L12)
- Repository access: [PlayerRepository.cs:17-33](file://GameBackend.Infrastructure/Repositories/PlayerRepository.cs#L17-L33)