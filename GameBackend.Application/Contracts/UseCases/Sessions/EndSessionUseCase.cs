using GameBackend.Application.Contracts.Sessions;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Sessions;

public class EndSessionUseCase
{
    private readonly ISessionRepository _sessionRepository;

    public EndSessionUseCase(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionResponse> ExecuteAsync(Guid playerId, string gameId, EndSessionRequest request)
    {
        var session = await _sessionRepository.GetActiveSessionAsync(playerId, gameId);
        if (session == null)
            throw new Exception("No active session found for this game");

        session.Status = SessionStatus.Completed;
        session.Score = request.Score;
        session.Metadata = request.Metadata ?? session.Metadata;
        session.EndedAt = DateTime.UtcNow;

        await _sessionRepository.UpdateAsync(session);

        return new SessionResponse
        {
            Id = session.Id,
            GameId = session.GameId,
            Status = session.Status,
            Score = session.Score,
            Metadata = session.Metadata,
            StartedAt = session.StartedAt,
            EndedAt = session.EndedAt
        };
    }
}