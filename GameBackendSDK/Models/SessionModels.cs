namespace GameBackendSDK.Models;

[Serializable]
public class StartSessionRequest
{
    public string GameId { get; set; } = string.Empty;
    public string? Metadata { get; set; }
}

[Serializable]
public class EndSessionRequest
{
    public long Score { get; set; }
    public string? Metadata { get; set; }
}

[Serializable]
public class SessionResponse
{
    public string Id { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public long Score { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}

[Serializable]
public class SessionStatsResponse
{
    public string GameId { get; set; } = string.Empty;
    public int TotalGames { get; set; }
    public long BestScore { get; set; }
    public double AverageScore { get; set; }
    public DateTime? LastPlayedAt { get; set; }
}