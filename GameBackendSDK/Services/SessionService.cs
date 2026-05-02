using GameBackendSDK.Models;

namespace GameBackendSDK.Services;

public class SessionService : BaseService
{
    private readonly string _gameId;

    public SessionService(HttpClient httpClient, string baseUrl, string gameId)
        : base(httpClient, baseUrl)
    {
        _gameId = gameId;
    }

    public async Task<SessionResponse> StartSessionAsync(string? metadata = null)
    {
        return await PostAsync<SessionResponse>("/api/sessions/start", new StartSessionRequest
        {
            GameId = _gameId,
            Metadata = metadata
        });
    }

    public async Task<SessionResponse> EndSessionAsync(long score, string? metadata = null)
    {
        return await PostAsync<SessionResponse>($"/api/sessions/{_gameId}/end", new EndSessionRequest
        {
            Score = score,
            Metadata = metadata
        });
    }

    public async Task<SessionStatsResponse> GetStatsAsync()
    {
        return await GetAsync<SessionStatsResponse>($"/api/sessions/{_gameId}/stats");
    }
}