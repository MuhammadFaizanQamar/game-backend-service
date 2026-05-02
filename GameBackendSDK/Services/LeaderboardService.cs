using GameBackendSDK.Models;

namespace GameBackendSDK.Services;

public class LeaderboardService : BaseService
{
    private readonly string _gameId;

    public LeaderboardService(HttpClient httpClient, string baseUrl, string gameId)
        : base(httpClient, baseUrl)
    {
        _gameId = gameId;
    }

    public async Task<ScoreResponse> SubmitScoreAsync(
        long score,
        string leaderboardName = "Global",
        string scoreType = "score",
        string? metadata = null)
    {
        return await PostAsync<ScoreResponse>($"/api/leaderboards/{_gameId}/scores",
            new SubmitScoreRequest
            {
                Name = leaderboardName,
                ScoreType = scoreType,
                Score = score,
                Metadata = metadata
            });
    }

    public async Task<LeaderboardResponse> GetTopAsync(
        string leaderboardName = "Global",
        int page = 1,
        int limit = 10)
    {
        return await GetAsync<LeaderboardResponse>(
            $"/api/leaderboards/{_gameId}/top?name={leaderboardName}&page={page}&limit={limit}");
    }

    public async Task<ScoreResponse> GetMyRankAsync(string leaderboardName = "Global")
    {
        return await GetAsync<ScoreResponse>(
            $"/api/leaderboards/{_gameId}/me?name={leaderboardName}");
    }

    public async Task<LeaderboardResponse> GetAroundMeAsync(
        string leaderboardName = "Global",
        int range = 5)
    {
        return await GetAsync<LeaderboardResponse>(
            $"/api/leaderboards/{_gameId}/around-me?name={leaderboardName}&range={range}");
    }
}