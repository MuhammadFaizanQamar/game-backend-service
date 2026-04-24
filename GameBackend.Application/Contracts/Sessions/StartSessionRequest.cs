namespace GameBackend.Application.Contracts.Sessions;

public class StartSessionRequest
{
    public string GameId { get; set; } = string.Empty;
    public string? Metadata { get; set; }
}