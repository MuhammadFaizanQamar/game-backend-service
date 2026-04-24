namespace GameBackend.Application.Contracts.Players;

public class PlayerProfileResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
}