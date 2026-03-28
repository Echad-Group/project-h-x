using ProjectHX.Mobile.Models.Auth;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IAuthApiService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<OtpVerifyResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    Task<string> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default);
    Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<string> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
