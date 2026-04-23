using GameBackend.Application.Contracts.Auth;
using GameBackend.Core.Entities;
using GameBackend.Core.Interfaces;

namespace GameBackend.Application.UseCases.Auth;

public class RegisterPlayerUseCase
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterPlayerUseCase(
        IPlayerRepository playerRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _playerRepository = playerRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponse> ExecuteAsync(RegisterRequest request)
    {
        // 1. Check if user exists
        var existingUser = await _playerRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new Exception("User already exists");

        // 2. Hash password
        var hashedPassword = _passwordHasher.Hash(request.Password);

        // 3. Create player
        var player = new Player
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            LastSeenAt = DateTime.UtcNow
        };

        // 4. Save
        await _playerRepository.AddAsync(player);

        // 5. Generate token
        var token = _jwtTokenGenerator.GenerateToken(player.Id, player.Username);

        // 6. Return response
        return new AuthResponse
        {
            PlayerId = player.Id,
            Username = player.Username,
            Token = token
        };
    }
}