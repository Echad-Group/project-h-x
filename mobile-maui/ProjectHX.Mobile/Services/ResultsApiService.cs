using System.Net.Http.Json;
using ProjectHX.Mobile.Models.Results;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class ResultsApiService : IResultsApiService
{
    private readonly HttpClient _httpClient;

    public ResultsApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress?.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase) == true &&
            !_httpClient.DefaultRequestHeaders.Contains("ngrok-skip-browser-warning"))
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
        }
    }

    public async Task<string> SubmitAsync(ResultSubmissionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("results/submit", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Result submitted.", "Failed to submit result.", cancellationToken);
    }
}
