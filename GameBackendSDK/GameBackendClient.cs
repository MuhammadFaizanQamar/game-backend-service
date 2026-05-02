using GameBackendSDK.Models;
using GameBackendSDK.Services;

namespace GameBackendSDK;

public class GameBackendClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly string _gameId;

    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;

    public AuthService Auth { get; }
    public PlayerService Player { get; }
    public LeaderboardService Leaderboard { get; }
    public SessionService Session { get; }

    public GameBackendClient(string baseUrl, string gameId)
    {
        _baseUrl = baseUrl;
        _gameId = gameId;
        _httpClient = new HttpClient();

        Auth = new AuthService(_httpClient, _baseUrl, OnAuthSuccess);
        Player = new PlayerService(_httpClient, _baseUrl);
        Leaderboard = new LeaderboardService(_httpClient, _baseUrl, _gameId);
        Session = new SessionService(_httpClient, _baseUrl, _gameId);
    }

    private void OnAuthSuccess(AuthResponse response)
    {
        _accessToken = response.AccessToken;
        _refreshToken = response.RefreshToken;

        // Update token on all services
        Auth.SetAccessToken(_accessToken);
        Player.SetAccessToken(_accessToken);
        Leaderboard.SetAccessToken(_accessToken);
        Session.SetAccessToken(_accessToken);
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken);
    public string CurrentPlayerId { get; private set; } = string.Empty;
    public string CurrentUsername { get; private set; } = string.Empty;
}