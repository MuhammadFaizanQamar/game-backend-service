# Game Backend Service
![CI](https://github.com/MuhammadFaizanQamar/game-backend-service/actions/workflows/ci.yml/badge.svg)

A production-deployed, generic game backend API built with ASP.NET Core 8. Supports multiple games via a single configurable server — inspired by PlayFab architecture.

**Live API (Azure):** https://game-backend-api-gdh5fjg0a0g4eqfr.centralindia-01.azurewebsites.net/swagger
**Live API (Railway):** https://game-backend-service-production.up.railway.app/swagger
**GitHub:** https://github.com/MuhammadFaizanQamar/game-backend-service

---

## What It Does

Any game can plug into this backend by passing a `GameId`. The server handles:

- Player registration and authentication with JWT
- Leaderboards with automatic creation per game and score type
- Game session tracking with history and aggregated stats
- Player profiles with metadata support
- Redis caching on leaderboard reads
- Rate limiting to prevent abuse

---

## Architecture

```
GameBackend.API           → Controllers, Middleware, Startup, Hubs
GameBackend.Application   → Use Cases, Contracts, Validators
GameBackend.Core          → Entities, Interfaces, Events
GameBackend.Infrastructure → EF Core, Repositories, JWT, BCrypt, Redis, Messaging
GameBackend.Tests         → xUnit Unit Tests (12) + Integration Tests (6)
GameBackend SDK           → Unity C# client SDK
```

**Pattern:** Clean Architecture — no business logic in controllers, no database access in use cases.

**Data flow:**
```
Controller → UseCase → Repository → Azure PostgreSQL
↓
JWT / BCrypt / Azure Redis
↓
Azure Service Bus → Consumer → Cache Invalidation
↓
SignalR → Connected Clients
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| API Framework | ASP.NET Core 8 |
| Database | PostgreSQL (Azure Database for PostgreSQL) |
| Cache | Redis (Azure Cache for Redis / Upstash) |
| Auth | JWT + BCrypt |
| Messaging | Azure Service Bus |
| Validation | FluentValidation |
| Testing | xUnit + NSubstitute + FluentAssertions + Testcontainers |
| Deployment | Azure App Service + Docker + Azure Container Registry |
| Monitoring | Azure Application Insights |
| Real-time | SignalR |

---

## Endpoints

### Auth
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/auth/register` | Register new player | No |
| POST | `/api/auth/login` | Login, returns JWT + refresh token | No |
| POST | `/api/auth/refresh` | Refresh expired access token | No |
| POST | `/api/auth/logout` | Revoke all refresh tokens | Yes |

### Players
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/players/me` | Get own profile | Yes |
| PUT | `/api/players/me` | Update profile | Yes |
| GET | `/api/players/{id}` | Get any player's public profile | Yes |

### Leaderboards
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/leaderboards/{gameId}/scores` | Submit score (auto-creates leaderboard) | Yes |
| GET | `/api/leaderboards/{gameId}/top` | Get top players (paginated, cached) | Yes |
| GET | `/api/leaderboards/{gameId}/me` | Get own rank | Yes |
| GET | `/api/leaderboards/{gameId}/around-me` | Get players ranked near you | Yes |

### Sessions
| Method | Endpoint | Description | Auth |
|---|---|---|---|
| POST | `/api/sessions/start` | Start a game session | Yes |
| POST | `/api/sessions/{gameId}/end` | End session and submit score | Yes |
| GET | `/api/sessions/{gameId}/history` | Get session history (paginated) | Yes |
| GET | `/api/sessions/{gameId}/stats` | Get aggregated stats | Yes |

---

## Rate Limits

| Endpoint Group | Limit |
|---|---|
| Auth endpoints | 5 requests/minute |
| Leaderboard reads | 30 requests/minute |
| General endpoints | 60 requests/minute |

---

## Local Setup

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- PostgreSQL via Docker

### 1. Clone the repo
```bash
git clone https://github.com/MuhammadFaizanQamar/game-backend-service.git
cd game-backend-service
```

### 2. Start PostgreSQL
```bash
docker run --name gamebackend-db -e POSTGRES_PASSWORD=password123 -p 5432:5432 -d postgres
```

### 3. Configure appsettings.json
```json
{
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "GameBackend",
    "Audience": "GameBackendUsers",
    "AccessTokenExpiryMinutes": 15,
    "RefreshTokenExpiryDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=gamebackend;Username=postgres;Password=password123"
  },
  "Redis": {
    "Url": "your-upstash-redis-url",
    "Token": "your-upstash-token"
  }
}
```

### 4. Run migrations
```bash
dotnet ef database update --project GameBackend.Infrastructure --startup-project GameBackend.API
```

### 5. Run the API
```bash
cd GameBackend.API
dotnet run
```

### 6. Open Swagger
```
http://localhost:5152/swagger
```

---

## Running Tests
dotnet test GameBackend.Tests

18 tests — 12 unit tests covering auth, player profile and leaderboards,
6 integration tests covering full HTTP pipeline with real PostgreSQL via Testcontainers.
---

## Design Decisions

**Why Azure Service Bus?**
Score submission publishes a ScoreSubmitted event to Azure Service Bus. The API returns immediately without waiting for cache invalidation. A background consumer processes the event asynchronously — decoupling API response time from downstream processing.

**Why SignalR?**
Real-time leaderboard updates pushed to all connected clients instantly when a new high score is submitted — no polling needed.
**Why Clean Architecture?**  
Controllers stay thin. Business logic lives in use cases. Each layer is independently testable.

**Why generic leaderboards?**  
Each leaderboard belongs to a `GameId` + `Name` combination. Any game can create any number of leaderboards (Global, Weekly, Coins, XP) without schema changes.

**Why Redis for caching?**  
Top leaderboard reads are the most frequent and expensive queries. Caching for 60 seconds eliminates repeated DB hits under load.

**Why refresh tokens?**  
Short-lived access tokens (15 min) limit damage if stolen. Refresh tokens allow seamless renewal without re-login.