using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class VolunteerApiService : IVolunteerApiService
{
    private readonly HttpClient _httpClient;

    public VolunteerApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<VolunteerStatusResponse> GetMyVolunteerStatusAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("volunteers/check-status", cancellationToken);
        var payload = await response.Content.ReadFromJsonAsync<VolunteerStatusResponse>(cancellationToken: cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Failed to load volunteer status.", cancellationToken));
        }

        return payload ?? new VolunteerStatusResponse { IsVolunteer = false };
    }

    public async Task<string> UpdateMyVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync("volunteers/me", request, cancellationToken);
        return await ReadMessageResponseAsync(response, "Volunteer profile updated successfully.", "Failed to update volunteer profile.", cancellationToken);
    }

    public async Task<string> LeaveVolunteerRoleAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync("volunteers/me", cancellationToken);
        return await ReadMessageResponseAsync(response, "You are no longer registered as a volunteer.", "Failed to leave volunteer role.", cancellationToken);
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
