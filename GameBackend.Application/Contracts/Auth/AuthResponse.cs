namespace GameBackend.Application.Contracts.Auth;

public class AuthResponse
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } = 900;
}