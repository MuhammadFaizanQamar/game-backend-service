using System.Text.Json;
using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Leaderboards;

public class GetPlayerRankUseCase
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICacheService _cacheService;

    public GetPlayerRankUseCase(
        ILeaderboardRepository leaderboardRepository,
        IPlayerRepository playerRepository,
        ICacheService cacheService)
    {
        _leaderboardRepository = leaderboardRepository;
        _playerRepository = playerRepository;
        _cacheService = cacheService;
    }

    public async Task<ScoreResponse> ExecuteAsync(string gameId, string name, Guid playerId)
    {
        var leaderboard = await _leaderboardRepository.GetByGameIdAndNameAsync(gameId, name);
        if (leaderboard == null)
            throw new Exception("Leaderboard not found");

        // Check cache
        var cacheKey = $"leaderboard:{leaderboard.Id}:rank:{playerId}";
        var cached = await _cacheService.GetAsync(cacheKey);
        if (cached != null)
        {
            var cachedResponse = JsonSerializer.Deserialize<ScoreResponse>(cached);
            if (cachedResponse != null) return cachedResponse;
        }

        var entry = await _leaderboardRepository.GetEntryByPlayerAsync(leaderboard.Id, playerId);
        if (entry == null)
            throw new Exception("Player has no score on this leaderboard");

        var rank = await _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId);
        var player = await _playerRepository.GetByIdAsync(playerId);

        var response = new ScoreResponse
        {
            PlayerId = playerId,
            Username = player?.Username ?? string.Empty,
            Score = entry.Score,
            Rank = rank,
            Metadata = entry.Metadata
        };

        // Cache for 30 seconds
        await _cacheService.SetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromSeconds(30));

        return response;
    }
}