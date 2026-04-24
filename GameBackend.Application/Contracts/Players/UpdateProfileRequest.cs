namespace GameBackend.Application.Contracts.Players;

public class UpdateProfileRequest
{
    public string? Username { get; set; }
    public string? AvatarUrl { get; set; }
}