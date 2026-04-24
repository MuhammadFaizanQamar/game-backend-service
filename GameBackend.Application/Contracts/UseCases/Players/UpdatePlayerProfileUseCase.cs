using GameBackend.Application.Contracts.Players;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Players;

public class UpdatePlayerProfileUseCase
{
    private readonly IPlayerRepository _playerRepository;

    public UpdatePlayerProfileUseCase(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<PlayerProfileResponse> ExecuteAsync(Guid playerId, UpdateProfileRequest request)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new Exception("Player not found");

        // Only update fields that were provided
        if (!string.IsNullOrWhiteSpace(request.Username))
            player.Username = request.Username;

        if (!string.IsNullOrWhiteSpace(request.AvatarUrl))
            player.AvatarUrl = request.AvatarUrl;

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