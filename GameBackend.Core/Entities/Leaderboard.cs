namespace GameBackend.Core.Entities;

public enum ResetPeriod
{
    None,
    Daily,
    Weekly,
    Monthly
}

public class Leaderboard
{
    public Guid Id { get; set; }
    public string GameId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ScoreType { get; set; } = string.Empty;
    public ResetPeriod ResetPeriod { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<LeaderboardEntry> Entries { get; set; } = new List<LeaderboardEntry>();
}