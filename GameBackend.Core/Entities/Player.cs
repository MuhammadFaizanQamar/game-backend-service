namespace GameBackend.Core.Entities;

public enum PlayerRole
{
    Player,
    Admin
}

public class Player
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public PlayerRole Role { get; set; } = PlayerRole.Player;
    public bool IsBanned { get; set; } = false;
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public string Metadata { get; set; } = "{}";
}