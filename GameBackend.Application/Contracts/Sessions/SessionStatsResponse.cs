namespace GameBackend.Application.Contracts.Sessions;

public class SessionStatsResponse
{
    public string GameId { get; set; } = string.Empty;
    public int TotalGames { get; set; }
    public long BestScore { get; set; }
    public double AverageScore { get; set; }
    public DateTime? LastPlayedAt { get; set; }
}