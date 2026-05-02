using GameBackend.Application.Contracts.Admin;
using GameBackend.Application.Contracts.Common;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Admin;

public class GetAllPlayersUseCase
{
    private readonly IPlayerRepository _playerRepository;

    public GetAllPlayersUseCase(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<PagedResponse<PlayerSummaryResponse>> ExecuteAsync(PaginationRequest pagination)
    {
        var (players, totalCount) = await _playerRepository.GetAllAsync(pagination.Skip, pagination.Limit);

        return new PagedResponse<PlayerSummaryResponse>
        {
            Page = pagination.Page,
            Limit = pagination.Limit,
            TotalCount = totalCount,
            Data = players.Select(p => new PlayerSummaryResponse
            {
                Id = p.Id,
                Username = p.Username,
                Email = p.Email,
                Role = p.Role,
                IsBanned = p.IsBanned,
                CreatedAt = p.CreatedAt,
                LastSeenAt = p.LastSeenAt
            }).ToList()
        };
    }
}