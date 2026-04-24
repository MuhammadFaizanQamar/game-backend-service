namespace GameBackend.Core.Entities;

public class LeaderboardEntry
{
    public Guid Id { get; set; }
    public Guid LeaderboardId { get; set; }
    public Guid PlayerId { get; set; }
    public long Score { get; set; }
    public string Metadata { get; set; } = "{}";
    public DateTime SubmittedAt { get; set; }
    public Leaderboard Leaderboard { get; set; } = null!;
    public Player Player { get; set; } = null!;
}