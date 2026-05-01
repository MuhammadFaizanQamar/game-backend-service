using GameBackend.Application.Contracts.Common;
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

    public async Task<PagedResponse<ScoreResponse>> ExecuteAsync(
        string gameId, string name, PaginationRequest pagination)
    {
        var leaderboard = await _leaderboardRepository.GetByGameIdAndNameAsync(gameId, name);
        if (leaderboard == null)
            throw new KeyNotFoundException("Leaderboard not found");

        // Cache key includes page and limit
        var cacheKey = $"leaderboard:{leaderboard.Id}:top:{pagination.Page}:{pagination.Limit}";
        var cached = await _cacheService.GetAsync(cacheKey);
        if (cached != null)
        {
            var cachedResponse = JsonSerializer.Deserialize<PagedResponse<ScoreResponse>>(cached);
            if (cachedResponse != null) return cachedResponse;
        }

        var (entries, totalCount) = await _leaderboardRepository
            .GetTopEntriesAsync(leaderboard.Id, pagination.Skip, pagination.Limit);

        var response = new PagedResponse<ScoreResponse>
        {
            Page = pagination.Page,
            Limit = pagination.Limit,
            TotalCount = totalCount,
            Data = entries.Select((e, index) => new ScoreResponse
            {
                PlayerId = e.PlayerId,
                Username = e.Player?.Username ?? string.Empty,
                Score = e.Score,
                Rank = pagination.Skip + index + 1,
                Metadata = e.Metadata
            }).ToList()
        };

        await _cacheService.SetAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            TimeSpan.FromSeconds(60));

        return response;
    }
}