namespace GameBackend.Application.Contracts.Leaderboards;

public class ScoreResponse
{
    public Guid PlayerId { get; set; }
    public string Username { get; set; } = string.Empty;
    public long Score { get; set; }
    public int Rank { get; set; }
    public string? Metadata { get; set; }
}