using GameBackend.Application.Contracts.Admin;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Admin;

public class UpdatePlayerRoleUseCase
{
    private readonly IPlayerRepository _playerRepository;

    public UpdatePlayerRoleUseCase(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task ExecuteAsync(Guid playerId, UpdatePlayerRoleRequest request)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException("Player not found");

        player.Role = request.Role;
        await _playerRepository.UpdateAsync(player);
    }
}