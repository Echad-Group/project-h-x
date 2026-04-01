using ProjectHX.Mobile.Contexts;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Services.Interfaces;
using System.Net.Http.Json;

namespace ProjectHX.Mobile.Services;

public sealed class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient, AppStorageContext appStorageContext)
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
            throw new InvalidOperationException(await ApiResponseReader.ReadErrorMessageAsync(response, payload?.Message ?? "Login failed.", cancellationToken));
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
            throw new InvalidOperationException(await ApiResponseReader.ReadErrorMessageAsync(response, payload?.Message ?? "Registration failed.", cancellationToken));
        }

        return payload ?? new LoginResponse { Message = "Registration complete.", OtpRequired = true, Purpose = "Registration" };
    }

    public async Task<OtpVerifyResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/verify", request, cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<OtpVerifyResponse>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ApiResponseReader.ReadErrorMessageAsync(response, payload?.Message ?? "OTP verification failed.", cancellationToken));
        }

        return payload ?? new OtpVerifyResponse { Message = "OTP verified." };
    }

    public async Task<string> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/send-otp", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "OTP sent.", "Failed to send OTP.", cancellationToken);
    }

    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(
            response,
            "Password reset instructions have been sent to your email.",
            "Failed to start password reset.",
            cancellationToken);
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/reset-password", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(
            response,
            "Password has been reset successfully.",
            "Failed to reset password.",
            cancellationToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("auth/logout", content: null, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ApiResponseReader.ReadErrorMessageAsync(response, "Failed to log out.", cancellationToken));
        }
    }
}
