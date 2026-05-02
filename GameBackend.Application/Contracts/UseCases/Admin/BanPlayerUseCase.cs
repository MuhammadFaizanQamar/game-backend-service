using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Admin;

public class BanPlayerUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public BanPlayerUseCase(
        IPlayerRepository playerRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _playerRepository = playerRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task ExecuteAsync(Guid playerId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new KeyNotFoundException("Player not found");

        player.IsBanned = true;
        await _playerRepository.UpdateAsync(player);

        // Revoke all tokens immediately
        await _refreshTokenRepository.RevokeAllPlayerTokensAsync(playerId);
    }
}