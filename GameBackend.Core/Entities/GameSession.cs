namespace GameBackend.Core.Entities;

public enum SessionStatus
{
    Active,
    Completed,
    Abandoned
}

public class GameSession
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public string GameId { get; set; } = string.Empty;
    public SessionStatus Status { get; set; }
    public long Score { get; set; }
    public string Metadata { get; set; } = "{}";
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public Player Player { get; set; } = null!;
}