using GameBackendSDK.Models;

namespace GameBackendSDK.Services;

public class PlayerService : BaseService
{
    public PlayerService(HttpClient httpClient, string baseUrl)
        : base(httpClient, baseUrl)
    {
    }

    public async Task<PlayerProfileResponse> GetMyProfileAsync()
    {
        return await GetAsync<PlayerProfileResponse>("/api/players/me");
    }

    public async Task<PlayerProfileResponse> UpdateProfileAsync(string? username = null, string? avatarUrl = null)
    {
        return await PutAsync<PlayerProfileResponse>("/api/players/me", new UpdateProfileRequest
        {
            Username = username,
            AvatarUrl = avatarUrl
        });
    }

    public async Task<PlayerProfileResponse> GetPlayerByIdAsync(string playerId)
    {
        return await GetAsync<PlayerProfileResponse>($"/api/players/{playerId}");
    }
}