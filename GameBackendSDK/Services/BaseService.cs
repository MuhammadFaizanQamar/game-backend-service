using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using GameBackendSDK.Exceptions;

namespace GameBackendSDK.Services;

public abstract class BaseService
{
    protected readonly HttpClient HttpClient;
    protected readonly string BaseUrl;
    private string _accessToken = string.Empty;

    protected BaseService(HttpClient httpClient, string baseUrl)
    {
        HttpClient = httpClient;
        BaseUrl = baseUrl.TrimEnd('/');
    }

    internal void SetAccessToken(string token)
    {
        _accessToken = token;
        HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await HttpClient.GetAsync($"{BaseUrl}{endpoint}");
        return await HandleResponseAsync<T>(response);
    }

    protected async Task<T> PostAsync<T>(string endpoint, object? body = null)
    {
        var content = body != null
            ? new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            : null;
        var response = await HttpClient.PostAsync($"{BaseUrl}{endpoint}", content);
        return await HandleResponseAsync<T>(response);
    }

    protected async Task<T> PutAsync<T>(string endpoint, object? body = null)
    {
        var content = body != null
            ? new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            : null;
        var response = await HttpClient.PutAsync($"{BaseUrl}{endpoint}", content);
        return await HandleResponseAsync<T>(response);
    }

    protected async Task DeleteAsync(string endpoint)
    {
        var response = await HttpClient.DeleteAsync($"{BaseUrl}{endpoint}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new ApiException((int)response.StatusCode, error);
        }
    }

    private static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(content, options);
                throw new ApiException((int)response.StatusCode, error?.Error ?? content);
            }
            catch (JsonException)
            {
                throw new ApiException((int)response.StatusCode, content);
            }
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<T>(content, jsonOptions)
               ?? throw new ApiException(500, "Failed to deserialize response");
    }

    private class ErrorResponse
    {
        public string? Error { get; set; }
    }
}