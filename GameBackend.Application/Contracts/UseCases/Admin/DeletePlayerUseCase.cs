using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Admin;

public class DeletePlayerUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public DeletePlayerUseCase(
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

        // Revoke all tokens before deleting
        await _refreshTokenRepository.RevokeAllPlayerTokensAsync(playerId);
        await _playerRepository.DeleteAsync(playerId);
    }
}