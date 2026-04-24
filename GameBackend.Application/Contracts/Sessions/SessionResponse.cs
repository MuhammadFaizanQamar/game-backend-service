using GameBackend.Core.Entities;

namespace GameBackend.Application.Contracts.Sessions;

public class SessionResponse
{
    public Guid Id { get; set; }
    public string GameId { get; set; } = string.Empty;
    public SessionStatus Status { get; set; }
    public long Score { get; set; }
    public string? Metadata { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public TimeSpan? Duration => EndedAt.HasValue ? EndedAt - StartedAt : null;
}