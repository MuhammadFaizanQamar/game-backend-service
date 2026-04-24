namespace GameBackend.Application.Contracts.Sessions;

public class EndSessionRequest
{
    public long Score { get; set; }
    public string? Metadata { get; set; }
}