using GameBackend.Core.Entities;

namespace GameBackend.Application.Contracts.Admin;

public class PlayerSummaryResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public PlayerRole Role { get; set; }
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
}