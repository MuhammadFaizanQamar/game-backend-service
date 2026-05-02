namespace GameBackendSDK.Models;

[Serializable]
public class PlayerProfileResponse
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeenAt { get; set; }
}

[Serializable]
public class UpdateProfileRequest
{
    public string? Username { get; set; }
    public string? AvatarUrl { get; set; }
}