using System.Net.Http.Json;
using ProjectHX.Mobile.Models.Leaderboard;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class LeaderboardApiService : ILeaderboardApiService
{
    private readonly HttpClient _httpClient;

    public LeaderboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress?.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase) == true &&
            !_httpClient.DefaultRequestHeaders.Contains("ngrok-skip-browser-warning"))
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
        }
    }

    public async Task<MyRankModel> GetMyRankAsync(string scope = "National", CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"leaderboard/my-rank?scope={Uri.EscapeDataString(scope)}", cancellationToken);
        await ApiResponseReader.EnsureSuccessAsync(response, "Leaderboard rank not found.", cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<MyRankModel>(cancellationToken: cancellationToken);
        return result ?? new MyRankModel();
    }
}
