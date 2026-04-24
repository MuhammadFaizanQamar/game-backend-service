using GameBackend.Application.Contracts.Sessions;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Sessions;

public class StartSessionUseCase
{
    private readonly ISessionRepository _sessionRepository;

    public StartSessionUseCase(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<SessionResponse> ExecuteAsync(Guid playerId, StartSessionRequest request)
    {
        // Check if player already has an active session for this game
        var existing = await _sessionRepository.GetActiveSessionAsync(playerId, request.GameId);
        if (existing != null)
        {
            // Abandon previous session automatically
            existing.Status = SessionStatus.Abandoned;
            existing.EndedAt = DateTime.UtcNow;
            await _sessionRepository.UpdateAsync(existing);
        }

        var session = await _sessionRepository.CreateAsync(new GameSession
        {
            Id = Guid.NewGuid(),
            PlayerId = playerId,
            GameId = request.GameId,
            Status = SessionStatus.Active,
            Score = 0,
            Metadata = request.Metadata ?? "{}",
            StartedAt = DateTime.UtcNow
        });

        return MapToResponse(session);
    }

    private static SessionResponse MapToResponse(GameSession session) => new()
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