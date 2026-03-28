using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class UserProfileApiService : IUserProfileApiService
{
    private readonly HttpClient _httpClient;

    public UserProfileApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserProfileModel> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("userprofile", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<UserProfileModel>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("Failed to load profile.");
        }

        return payload ?? throw new InvalidOperationException("Empty profile response.");
    }

    public async Task UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync("userprofile", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Failed to update profile.", cancellationToken));
        }
    }

    public async Task<string> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync("userprofile/password", request, cancellationToken);
        return await ReadMessageResponseAsync(response, "Password changed successfully.", "Failed to change password.", cancellationToken);
    }

    public async Task<string> UpdateEmailAsync(UpdateEmailRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync("userprofile/email", request, cancellationToken);
        return await ReadMessageResponseAsync(response, "Email updated successfully.", "Failed to update email.", cancellationToken);
    }

    public async Task<string> DeleteAccountAsync(string password, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "userprofile")
        {
            Content = JsonContent.Create(password)
        }, cancellationToken);

        return await ReadMessageResponseAsync(response, "Account deleted successfully.", "Failed to delete account.", cancellationToken);
    }

    public async Task<string> UploadProfilePhotoAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        using var content = CreateFileContent(stream, fileName, contentType);
        var response = await _httpClient.PostAsync("userprofile/upload-photo", content, cancellationToken);
        return await ReadMessageResponseAsync(response, "Profile photo uploaded successfully.", "Failed to upload profile photo.", cancellationToken);
    }

    public async Task<string> DeleteProfilePhotoAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync("userprofile/photo", cancellationToken);
        return await ReadMessageResponseAsync(response, "Profile photo deleted successfully.", "Failed to delete profile photo.", cancellationToken);
    }

    public async Task<string> UploadVerificationDocumentAsync(Stream stream, string fileName, string contentType, string documentType, CancellationToken cancellationToken = default)
    {
        using var content = CreateFileContent(stream, fileName, contentType);
        content.Add(new StringContent(documentType), "documentType");

        var response = await _httpClient.PostAsync("userprofile/upload-verification-document", content, cancellationToken);
        return await ReadMessageResponseAsync(response, "Verification document uploaded successfully.", "Failed to upload verification document.", cancellationToken);
    }

    private static MultipartFormDataContent CreateFileContent(Stream stream, string fileName, string contentType)
    {
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType);
        content.Add(streamContent, "file", fileName);
        return content;
    }

    private static async Task<string> ReadMessageResponseAsync(HttpResponseMessage response, string successFallback, string failureFallback, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, failureFallback, cancellationToken));
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return successFallback;
        }

        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? successFallback;
        }

        return successFallback;
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallback;
        }

        using var doc = JsonDocument.Parse(content);
        if (doc.RootElement.TryGetProperty("message", out var message))
        {
            return message.GetString() ?? fallback;
        }

        if (doc.RootElement.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Array)
        {
            var values = errors.EnumerateArray().Select(item => item.GetString()).Where(item => !string.IsNullOrWhiteSpace(item));
            var joined = string.Join(Environment.NewLine, values!);
            return string.IsNullOrWhiteSpace(joined) ? fallback : joined;
        }

        return fallback;
    }
}
