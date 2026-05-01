using GameBackend.Application.Contracts.Auth;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Auth;

public class RegisterPlayerUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RegisterPlayerUseCase(
        IPlayerRepository playerRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterRequest request)
    {
        var existingUser = await _playerRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists");

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
        };

        await _playerRepository.AddAsync(player);

        var accessToken = _jwtTokenGenerator.GenerateToken(player.Id, player.Username);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            PlayerId = player.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        return new AuthResponse
        {
            PlayerId = player.Id,
            Username = player.Username,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}