using GameBackend.Application.Contracts.Players;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Players;

public class GetPlayerProfileUseCase
{
    private readonly IPlayerRepository _playerRepository;

    public GetPlayerProfileUseCase(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<PlayerProfileResponse> ExecuteAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new Exception("Player not found");

        // Update last seen
        player.LastSeenAt = DateTime.UtcNow;
        await _playerRepository.UpdateAsync(player);

        return new PlayerProfileResponse
        {
            Id = player.Id,
            Username = player.Username,
            AvatarUrl = player.AvatarUrl,
            CreatedAt = player.CreatedAt,
            LastSeenAt = player.LastSeenAt
        };
    }
}