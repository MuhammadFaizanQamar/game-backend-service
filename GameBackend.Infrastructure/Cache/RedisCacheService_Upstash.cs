using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameBackend.Infrastructure.Cache;

public class RedisCacheService_Upstash : RedisCacheServiceBase
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public RedisCacheService_Upstash(RedisSettings settings)
    {
        _baseUrl = settings.Url.TrimEnd('/');
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", settings.Token);
    }

    public override async Task<string?> GetAsync(string key)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/get/{Uri.EscapeDataString(key)}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<UpstashResponse>(json);
        return result?.Result;
    }

    public override async Task SetAsync(string key, string value, TimeSpan expiry)
    {
        var seconds = (int)expiry.TotalSeconds;
        var url = $"{_baseUrl}/set/{Uri.EscapeDataString(key)}/{Uri.EscapeDataString(value)}?EX={seconds}";
        await _httpClient.GetAsync(url);
    }

    public override async Task DeleteAsync(string key)
    {
        await _httpClient.GetAsync($"{_baseUrl}/del/{Uri.EscapeDataString(key)}");
    }

    private class UpstashResponse
    {
        [JsonPropertyName("result")]
        public string? Result { get; set; }
    }
}