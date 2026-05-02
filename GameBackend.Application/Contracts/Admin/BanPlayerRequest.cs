namespace GameBackend.Application.Contracts.Admin;

public class BanPlayerRequest
{
    public string Reason { get; set; } = string.Empty;
}