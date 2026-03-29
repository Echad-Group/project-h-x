using System.Net.Http.Json;
using System.Text.Json;
using ProjectHX.Mobile.Models.Leaderboard;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class LeaderboardApiService : ILeaderboardApiService
{
    private readonly HttpClient _httpClient;

    public LeaderboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MyRankModel> GetMyRankAsync(string scope = "National", CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"leaderboard/my-rank?scope={Uri.EscapeDataString(scope)}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(await ReadErrorMessageAsync(response, "Leaderboard rank not found.", cancellationToken));
        }

        var result = await response.Content.ReadFromJsonAsync<MyRankModel>(cancellationToken: cancellationToken);
        return result ?? new MyRankModel();
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string fallback, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(content)) return fallback;

        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.TryGetProperty("message", out var msg) ? msg.GetString() ?? fallback : fallback;
    }
}
