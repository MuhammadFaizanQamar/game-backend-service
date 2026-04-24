using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Leaderboards;

public class SubmitScoreUseCase
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICacheService _cacheService;

    public SubmitScoreUseCase(
        ILeaderboardRepository leaderboardRepository,
        IPlayerRepository playerRepository,
        ICacheService cacheService)
    {
        _leaderboardRepository = leaderboardRepository;
        _playerRepository = playerRepository;
        _cacheService = cacheService;
    }

    public async Task<ScoreResponse> ExecuteAsync(string gameId, Guid playerId, SubmitScoreRequest request)
    {
        // 1. Get or auto-create leaderboard
        var leaderboard = await _leaderboardRepository.GetByGameIdAndNameAsync(gameId, request.Name);
        if (leaderboard == null)
        {
            leaderboard = await _leaderboardRepository.CreateAsync(new Leaderboard
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                Name = request.Name,
                ScoreType = request.ScoreType,
                ResetPeriod = request.ResetPeriod,
                CreatedAt = DateTime.UtcNow
            });
        }

        // 2. Check existing entry
        var existingEntry = await _leaderboardRepository.GetEntryByPlayerAsync(leaderboard.Id, playerId);

        if (existingEntry != null)
        {
            // Option A — only update if new score is higher
            if (request.Score <= existingEntry.Score)
            {
                var currentRank = await _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId);
                var player = await _playerRepository.GetByIdAsync(playerId);
                return new ScoreResponse
                {
                    PlayerId = playerId,
                    Username = player?.Username ?? string.Empty,
                    Score = existingEntry.Score,
                    Rank = currentRank,
                    Metadata = existingEntry.Metadata
                };
            }

            // Update with new high score
            existingEntry.Score = request.Score;
            existingEntry.Metadata = request.Metadata ?? "{}";
            existingEntry.SubmittedAt = DateTime.UtcNow;
            await _leaderboardRepository.UpdateEntryAsync(existingEntry);
        }
        else
        {
            // Create new entry
            await _leaderboardRepository.AddEntryAsync(new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                LeaderboardId = leaderboard.Id,
                PlayerId = playerId,
                Score = request.Score,
                Metadata = request.Metadata ?? "{}",
                SubmittedAt = DateTime.UtcNow
            });
        }

        // 3. Invalidate cache for this leaderboard
        await _cacheService.DeleteAsync($"leaderboard:{leaderboard.Id}:top");
        await _cacheService.DeleteAsync($"leaderboard:{leaderboard.Id}:rank:{playerId}");

        // 4. Get updated rank
        var rank = await _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId);
        var playerEntity = await _playerRepository.GetByIdAsync(playerId);

        return new ScoreResponse
        {
            PlayerId = playerId,
            Username = playerEntity?.Username ?? string.Empty,
            Score = request.Score,
            Rank = rank,
            Metadata = request.Metadata
        };
    }
}