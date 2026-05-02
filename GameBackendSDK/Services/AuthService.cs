using GameBackendSDK.Models;

namespace GameBackendSDK.Services;

public class AuthService : BaseService
{
    private readonly Action<AuthResponse> _onAuthSuccess;

    public AuthService(HttpClient httpClient, string baseUrl, Action<AuthResponse> onAuthSuccess)
        : base(httpClient, baseUrl)
    {
        _onAuthSuccess = onAuthSuccess;
    }

    public async Task<AuthResponse> RegisterAsync(string username, string email, string password)
    {
        var response = await PostAsync<AuthResponse>("/api/auth/register", new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password
        });
        _onAuthSuccess(response);
        SetAccessToken(response.AccessToken);
        return response;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var response = await PostAsync<AuthResponse>("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = password
        });
        _onAuthSuccess(response);
        SetAccessToken(response.AccessToken);
        return response;
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken)
    {
        var response = await PostAsync<AuthResponse>("/api/auth/refresh", new RefreshTokenRequest
        {
            RefreshToken = refreshToken
        });
        _onAuthSuccess(response);
        SetAccessToken(response.AccessToken);
        return response;
    }

    public async Task LogoutAsync()
    {
        await PostAsync<object>("/api/auth/logout");
    }
}