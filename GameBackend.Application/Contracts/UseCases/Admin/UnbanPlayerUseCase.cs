using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Admin;

public class UnbanPlayerUseCase
{
    private readonly IPlayerRepository _playerRepository;

    public UnbanPlayerUseCase(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task ExecuteAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException("Player not found");

        player.IsBanned = false;
        await _playerRepository.UpdateAsync(player);
    }
}