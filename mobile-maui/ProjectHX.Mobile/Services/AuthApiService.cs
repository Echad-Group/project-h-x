using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;
using System.Net.Http.Json;

namespace ProjectHX.Mobile.Services;

public sealed class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (httpClient.BaseAddress!.Host.Contains("ngrok")) _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(payload?.Message ?? "Login failed.");
        }

        return payload ?? new LoginResponse { Message = "Empty login response." };
    }

    public async Task<LoginResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/verify-otp", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(payload?.Message ?? "OTP verification failed.");
        }

        return payload ?? new LoginResponse { Message = "Empty OTP response." };
    }
}
