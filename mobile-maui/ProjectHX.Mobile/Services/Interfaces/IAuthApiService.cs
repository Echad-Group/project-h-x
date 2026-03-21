using ProjectHX.Mobile.Models.Auth;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IAuthApiService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
}
