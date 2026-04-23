namespace GameBackend.Application.Contracts.Auth;

public class AuthResponse
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}