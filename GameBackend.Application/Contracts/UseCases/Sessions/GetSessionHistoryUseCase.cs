using GameBackend.Application.Contracts.Common;
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

    public async Task<PagedResponse<SessionResponse>> ExecuteAsync(
        Guid playerId, string gameId, PaginationRequest pagination)
    {
        var (sessions, totalCount) = await _sessionRepository
            .GetPlayerHistoryAsync(playerId, gameId, pagination.Skip, pagination.Limit);

        return new PagedResponse<SessionResponse>
        {
            Page = pagination.Page,
            Limit = pagination.Limit,
            TotalCount = totalCount,
            Data = sessions.Select(session => new SessionResponse
            {
                Id = session.Id,
                GameId = session.GameId,
                Status = session.Status,
                Score = session.Score,
                Metadata = session.Metadata,
                StartedAt = session.StartedAt,
                EndedAt = session.EndedAt
            }).ToList()
        };
    }
}