using GameBackend.Core.Entities;

namespace GameBackend.Application.Contracts.Leaderboards;

public class SubmitScoreRequest
{
    public string Name { get; set; } = string.Empty;
    public string ScoreType { get; set; } = string.Empty;
    public long Score { get; set; }
    public ResetPeriod ResetPeriod { get; set; }
    public string? Metadata { get; set; }
}