using GameBackend.Application.Contracts.Sessions;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Sessions;

public class GetSessionHistoryUseCase
{
    private readonly ISessionRepository _sessionRepository;

    public GetSessionHistoryUseCase(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<List<SessionResponse>> ExecuteAsync(Guid playerId, string gameId, int limit = 20)
    {
        var sessions = await _sessionRepository.GetPlayerHistoryAsync(playerId, gameId, limit);

        return sessions.Select(session => new SessionResponse
        {
            Id = session.Id,
            GameId = session.GameId,
            Status = session.Status,
            Score = session.Score,
            Metadata = session.Metadata,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt
        }).ToList();
    }
}