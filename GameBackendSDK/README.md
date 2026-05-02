# GameBackend Unity SDK

A C# client SDK for the GameBackend API. Drop this into any Unity project to instantly add auth, leaderboards, sessions and player profiles.

## Installation

1. Copy the `GameBackendSDK` folder into your Unity project's `Assets` folder
2. Unity will automatically compile the scripts

## Quick Start

```csharp
// Initialize the client
var client = new GameBackendClient(
    "https://game-backend-service-production.up.railway.app",
    "YourGameId");

// Register
var auth = await client.Auth.RegisterAsync("username", "email@game.com", "Password123!");

// Login
var auth = await client.Auth.LoginAsync("email@game.com", "Password123!");

// Submit a score
await client.Leaderboard.SubmitScoreAsync(1500);

// Get top 10 players
var leaderboard = await client.Leaderboard.GetTopAsync("Global", limit: 10);

// Start a game session
await client.Session.StartSessionAsync();

// End session with score
await client.Session.EndSessionAsync(score: 2500);

// Get player profile
var profile = await client.Player.GetMyProfileAsync();
```

## API Reference

### Auth
```csharp
client.Auth.RegisterAsync(username, email, password)
client.Auth.LoginAsync(email, password)
client.Auth.RefreshAsync(refreshToken)
client.Auth.LogoutAsync()
```

### Player
```csharp
client.Player.GetMyProfileAsync()
client.Player.UpdateProfileAsync(username, avatarUrl)
client.Player.GetPlayerByIdAsync(playerId)
```

### Leaderboard
```csharp
client.Leaderboard.SubmitScoreAsync(score, leaderboardName, scoreType, metadata)
client.Leaderboard.GetTopAsync(leaderboardName, page, limit)
client.Leaderboard.GetMyRankAsync(leaderboardName)
client.Leaderboard.GetAroundMeAsync(leaderboardName, range)
```

### Session
```csharp
client.Session.StartSessionAsync(metadata)
client.Session.EndSessionAsync(score, metadata)
client.Session.GetStatsAsync()
```

## Token Management

The SDK handles token storage and refresh automatically. After login, all subsequent requests include the JWT token. When a token expires, call `RefreshAsync` with the stored refresh token.

```csharp
// Store tokens after login
PlayerPrefs.SetString("AccessToken", auth.AccessToken);
PlayerPrefs.SetString("RefreshToken", auth.RefreshToken);

// On game start — restore session
var accessToken = PlayerPrefs.GetString("AccessToken");
var refreshToken = PlayerPrefs.GetString("RefreshToken");
if (!string.IsNullOrEmpty(refreshToken))
{
    await client.Auth.RefreshAsync(refreshToken);
}
```

## Error Handling

```csharp
try
{
    await client.Auth.LoginAsync(email, password);
}
catch (ApiException ex)
{
    Debug.LogError($"API Error {ex.StatusCode}: {ex.ErrorMessage}");
    if (ex.StatusCode == 401)
    {
        // Handle unauthorized
    }
}
```

## Live API
Base URL: `https://game-backend-service-production.up.railway.app`  
Swagger: `https://game-backend-service-production.up.railway.app/swagger`