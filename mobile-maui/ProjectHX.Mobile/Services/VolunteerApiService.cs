using System.Net.Http.Json;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class VolunteerApiService : IVolunteerApiService
{
    private readonly HttpClient _httpClient;

    public VolunteerApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress?.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase) == true &&
            !_httpClient.DefaultRequestHeaders.Contains("ngrok-skip-browser-warning"))
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
        }
    }

    public async Task<VolunteerStatusResponse> GetMyVolunteerStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("volunteers/check-status", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<VolunteerStatusResponse>(cancellationToken: cancellationToken);

        await ApiResponseReader.EnsureSuccessAsync(response, "Failed to load volunteer status.", cancellationToken);

        return payload ?? new VolunteerStatusResponse { IsVolunteer = false };
    }

    public async Task<string> CreateVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("volunteers", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(
            response,
            "Volunteer sign-up completed. You will now receive assignment updates for your selected areas and skills.",
            "Failed to complete volunteer sign-up.",
            cancellationToken);
    }

    public async Task<string> UpdateMyVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync("volunteers/me", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Volunteer profile updated successfully.", "Failed to update volunteer profile.", cancellationToken);
    }

    public async Task<string> LeaveVolunteerRoleAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync("volunteers/me", cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "You are no longer registered as a volunteer.", "Failed to leave volunteer role.", cancellationToken);
    }
}
