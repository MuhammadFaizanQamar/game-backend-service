namespace GameBackendSDK.Models;

[Serializable]
public class SubmitScoreRequest
{
    public string Name { get; set; } = "Global";
    public string ScoreType { get; set; } = "score";
    public long Score { get; set; }
    public int ResetPeriod { get; set; } = 0;
    public string? Metadata { get; set; }
}

[Serializable]
public class ScoreResponse
{
    public string PlayerId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public long Score { get; set; }
    public int Rank { get; set; }
    public string? Metadata { get; set; }
}

[Serializable]
public class LeaderboardResponse
{
    public string LeaderboardId { get; set; } = string.Empty;
    public string GameId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ScoreType { get; set; } = string.Empty;
    public List<ScoreResponse> Data { get; set; } = new();
    public int Page { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
}