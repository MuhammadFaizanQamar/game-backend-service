using System.Net;
using System.Net.Http.Json;
using GameBackend.Application.Contracts.Auth;
using Xunit;
using FluentAssertions;

namespace GameBackend.Tests.Integration;

public class AuthIntegrationTests : IClassFixture<GameBackendWebFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(GameBackendWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        // Arrange
        var request = new
        {
            username = "integrationuser",
            email = "integration@test.com",
            password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.Username.Should().Be("integrationuser");
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns400()
    {
        // Arrange
        var request = new
        {
            username = "duplicateuser",
            email = "duplicate@test.com",
            password = "Password123!"
        };

        // Act
        await _client.PostAsJsonAsync("/api/auth/register", request);
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200()
    {
        // Arrange — register first
        var registerRequest = new
        {
            username = "loginuser",
            email = "login@test.com",
            password = "Password123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "login@test.com",
            password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        // Arrange
        var registerRequest = new
        {
            username = "wrongpassuser",
            email = "wrongpass@test.com",
            password = "Password123!"
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            email = "wrongpass@test.com",
            password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithoutToken_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/players/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetProfile_WithValidToken_Returns200()
    {
        // Arrange — register and get token
        var request = new
        {
            username = "profileuser",
            email = "profile@test.com",
            password = "Password123!"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", request);
        var auth = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth!.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/players/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}