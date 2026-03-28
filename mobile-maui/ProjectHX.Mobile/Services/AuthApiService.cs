using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjectHX.Mobile.Services;

public sealed class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress?.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase) == true &&
            !_httpClient.DefaultRequestHeaders.Contains("ngrok-skip-browser-warning"))
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/login", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && payload?.OtpRequired == true)
        {
            return payload;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(payload?.Message ?? "Login failed.");
        }

        return payload ?? new LoginResponse { Message = "Empty login response." };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(request.FirstName), "firstName");
        content.Add(new StringContent(request.LastName), "lastName");
        content.Add(new StringContent(request.Email), "email");
        content.Add(new StringContent(request.Password), "password");

        var response = await _httpClient.PostAsync("auth/register", content, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(payload?.Message ?? "Registration failed.");
        }

        return payload ?? new LoginResponse { Message = "Registration complete.", OtpRequired = true, Purpose = "Registration" };
    }

    public async Task<OtpVerifyResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/verify", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<OtpVerifyResponse>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(payload?.Message ?? "OTP verification failed.");
        }

        return payload ?? new OtpVerifyResponse { Message = "OTP verified." };
    }

    public async Task<string> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/send-otp", request, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to send OTP.");
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            return "OTP sent.";
        }

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? "OTP sent.";
        }

        return "OTP sent.";
    }
}
