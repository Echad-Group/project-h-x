using ProjectHX.Mobile.Models.Profile;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IUserProfileApiService
{
    Task<UserProfileModel> GetProfileAsync(CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<string> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<string> UpdateEmailAsync(UpdateEmailRequest request, CancellationToken cancellationToken = default);
    Task<string> DeleteAccountAsync(string password, CancellationToken cancellationToken = default);
    Task<string> UploadProfilePhotoAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<string> DeleteProfilePhotoAsync(CancellationToken cancellationToken = default);
    Task<string> UploadVerificationDocumentAsync(Stream stream, string fileName, string contentType, string documentType, CancellationToken cancellationToken = default);
}
