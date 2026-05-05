namespace GameBackend.Core.Events;

public class ScoreSubmittedEvent
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string LeaderboardName { get; set; } = string.Empty;
    public Guid LeaderboardId { get; set; }
    public long Score { get; set; }
    public int Rank { get; set; }
    public DateTime SubmittedAt { get; set; }
}