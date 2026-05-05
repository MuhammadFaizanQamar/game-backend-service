using GameBackend.Application.Contracts.Leaderboards;
using GameBackend.Core.Entities;
using GameBackend.Core.Events;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Leaderboards;

public class SubmitScoreUseCase
{
    private readonly ILeaderboardRepository _leaderboardRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ICacheService _cacheService;
    private readonly ILeaderboardNotificationService? _notificationService;
    private readonly IEventPublisher? _eventPublisher;

    public SubmitScoreUseCase(
        ILeaderboardRepository leaderboardRepository,
        IPlayerRepository playerRepository,
        ICacheService cacheService,
        ILeaderboardNotificationService? notificationService = null,
        IEventPublisher? eventPublisher = null)
    {
        _leaderboardRepository = leaderboardRepository;
        _playerRepository = playerRepository;
        _cacheService = cacheService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
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

            existingEntry.Score = request.Score;
            existingEntry.Metadata = request.Metadata ?? "{}";
            existingEntry.SubmittedAt = DateTime.UtcNow;
            await _leaderboardRepository.UpdateEntryAsync(existingEntry);
        }
        else
        {
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

        // 3. Get updated rank
        var rank = await _leaderboardRepository.GetPlayerRankAsync(leaderboard.Id, playerId);
        var playerEntity = await _playerRepository.GetByIdAsync(playerId);

        var response = new ScoreResponse
        {
            PlayerId = playerId,
            Username = playerEntity?.Username ?? string.Empty,
            Score = request.Score,
            Rank = rank,
            Metadata = request.Metadata
        };

        // 4. Publish event to Service Bus (async — non-blocking)
        if (_eventPublisher != null)
        {
            await _eventPublisher.PublishAsync("score-submitted", new ScoreSubmittedEvent
            {
                PlayerId = playerId,
                Username = playerEntity?.Username ?? string.Empty,
                GameId = gameId,
                LeaderboardName = request.Name,
                LeaderboardId = leaderboard.Id,
                Score = request.Score,
                Rank = rank,
                SubmittedAt = DateTime.UtcNow
            });
        }
        else
        {
            // Fallback — direct cache invalidation if Service Bus not configured
            await _cacheService.DeleteAsync($"leaderboard:{leaderboard.Id}:top");
            await _cacheService.DeleteAsync($"leaderboard:{leaderboard.Id}:rank:{playerId}");
        }

        // 5. Notify SignalR clients
        if (_notificationService != null)
            await _notificationService.NotifyScoreUpdatedAsync(gameId, request.Name, response);

        return response;
    }
}