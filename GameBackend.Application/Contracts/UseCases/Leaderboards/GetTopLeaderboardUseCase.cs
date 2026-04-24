using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Core.Interfaces;
using System.Text.Json;

namespace GameBackend.Application.UseCases.Leaderboards;

public class GetTopLeaderboardUseCase
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly ICacheService _cacheService;

    public GetTopLeaderboardUseCase(
        ILeaderboardRepository leaderboardRepository,
        ICacheService cacheService)
    {
        _leaderboardRepository = leaderboardRepository;
        _cacheService = cacheService;
    }

    public async Task<LeaderboardResponse> ExecuteAsync(string gameId, string name, int limit = 10)
    {
        var leaderboard = await _leaderboardRepository.GetByGameIdAndNameAsync(gameId, name);
        if (leaderboard == null)
            throw new Exception("Leaderboard not found");

        // Check cache first
        var cacheKey = $"leaderboard:{leaderboard.Id}:top:{limit}";
        var cached = await _cacheService.GetAsync(cacheKey);
        if (cached != null)
        {
            var cachedResponse = JsonSerializer.Deserialize<LeaderboardResponse>(cached);
            if (cachedResponse != null) return cachedResponse;
        }

        // Query DB
        var entries = await _leaderboardRepository.GetTopEntriesAsync(leaderboard.Id, limit);

        var response = new LeaderboardResponse
        {
            LeaderboardId = leaderboard.Id,
            GameId = gameId,
            Name = name,
            ScoreType = leaderboard.ScoreType,
            GeneratedAt = DateTime.UtcNow,
            Entries = entries.Select((e, index) => new ScoreResponse
            {
                PlayerId = e.PlayerId,
                Username = e.Player?.Username ?? string.Empty,
                Score = e.Score,
                Rank = index + 1,
                Metadata = e.Metadata
            }).ToList()
        };

        // Cache for 60 seconds
        await _cacheService.SetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromSeconds(60));

        return response;
    }
}