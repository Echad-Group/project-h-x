using System.Net.Http.Json;
using ProjectHX.Mobile.Models.Tasks;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class TasksApiService : ITasksApiService
{
    private readonly HttpClient _httpClient;

    public TasksApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        if (_httpClient.BaseAddress?.Host.Contains("ngrok", StringComparison.OrdinalIgnoreCase) == true &&
            !_httpClient.DefaultRequestHeaders.Contains("ngrok-skip-browser-warning"))
        {
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "69420");
        }
    }

    public async Task<List<TaskModel>> GetMyTasksAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("tasks/my-tasks", cancellationToken);
        await ApiResponseReader.EnsureSuccessAsync(response, "Failed to load tasks.", cancellationToken);

        var result = await response.Content.ReadFromJsonAsync<List<TaskModel>>(cancellationToken: cancellationToken);
        return result ?? [];
    }

    public async Task<string> StartTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        var request = new TaskStatusRequest { TaskId = taskId, Status = "InProgress" };
        var response = await _httpClient.PostAsJsonAsync("tasks/status", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Task started.", "Failed to start task.", cancellationToken);
    }

    public async Task<string> CompleteTaskAsync(int taskId, string? notes, CancellationToken cancellationToken = default)
    {
        var request = new TaskCompleteRequest { TaskId = taskId, CompletionNotes = notes };
        var response = await _httpClient.PostAsJsonAsync("tasks/complete", request, cancellationToken);
        return await ApiResponseReader.ReadMessageResponseAsync(response, "Task completed successfully.", "Failed to complete task.", cancellationToken);
    }
}
