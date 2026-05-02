namespace GameBackend.Core.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid playerId, string username, string role = "Player");
    string GenerateRefreshToken();
}