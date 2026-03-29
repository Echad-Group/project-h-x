using ProjectHX.Mobile.Models.Tasks;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface ITasksApiService
{
    Task<List<TaskModel>> GetMyTasksAsync(CancellationToken cancellationToken = default);
    Task<string> StartTaskAsync(int taskId, CancellationToken cancellationToken = default);
    Task<string> CompleteTaskAsync(int taskId, string? notes, CancellationToken cancellationToken = default);
}
