namespace GameBackend.Application.Contracts.Leaderboards;

public class LeaderboardResponse
{
    public Guid LeaderboardId { get; set; }
    public string GameId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ScoreType { get; set; } = string.Empty;
    public List<ScoreResponse> Entries { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}