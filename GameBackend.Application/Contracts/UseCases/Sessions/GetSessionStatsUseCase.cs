using GameBackend.Application.Contracts.Sessions;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Sessions;

public class GetSessionStatsUseCase
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionStatsUseCase(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionStatsResponse> ExecuteAsync(Guid playerId, string gameId)
    {
        var stats = await _sessionRepository.GetPlayerStatsAsync(playerId, gameId);

        return new SessionStatsResponse
        {
            GameId = gameId,
            TotalGames = stats.TotalGames,
            BestScore = stats.BestScore,
            AverageScore = stats.AverageScore,
            LastPlayedAt = stats.LastPlayedAt
        };
    }
}