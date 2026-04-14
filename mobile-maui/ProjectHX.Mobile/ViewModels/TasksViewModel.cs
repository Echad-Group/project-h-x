using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Infrastructure.Outbox;
using ProjectHX.Mobile.Models.Tasks;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class TasksViewModel : BaseViewModel, IAsyncPageLoadable
{
    private readonly ITasksApiService _tasksApiService;
    private readonly ISyncOutboxService _outboxService;

    private List<TaskModel> _allTasks = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasTasks))]
    [NotifyPropertyChangedFor(nameof(HasNoTasks))]
    private ObservableCollection<TaskModel> tasks = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedTask))]
    private TaskModel? selectedTask;

    [ObservableProperty]
    private string activeFilter = "All";

    [ObservableProperty]
    private string completionNotes = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCompletionFormNotVisible))]
    private bool isCompletionFormVisible;

    public bool HasSelectedTask => SelectedTask != null;
    public bool IsCompletionFormNotVisible => !IsCompletionFormVisible;
    public bool HasTasks => Tasks.Count > 0;
    public bool HasNoTasks => !HasTasks;

    public TasksViewModel(ITasksApiService tasksApiService, ISyncOutboxService outboxService)
    {
        _tasksApiService = tasksApiService;
        _outboxService = outboxService;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            await LoadTasksCoreAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void SetFilter(string filter)
    {
        ActiveFilter = filter;
        SelectedTask = null;
        IsCompletionFormVisible = false;
        ApplyFilter(filter);
    }

    [RelayCommand]
    private void SelectTask(TaskModel task)
    {
        if (SelectedTask?.Id == task.Id)
        {
            SelectedTask = null;
            IsCompletionFormVisible = false;
        }
        else
        {
            SelectedTask = task;
            CompletionNotes = string.Empty;
            IsCompletionFormVisible = false;
        }
    }

    [RelayCommand]
    private void ShowCompletionForm()
    {
        IsCompletionFormVisible = true;
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task StartTaskAsync()
    {
        if (SelectedTask == null || IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var msg = await _tasksApiService.StartTaskAsync(SelectedTask.Id);
            InfoMessage = msg;
            await LoadTasksCoreAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task CompleteTaskAsync()
    {
        if (SelectedTask == null || IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        var taskId = SelectedTask.Id;
        var notes = CompletionNotes.Trim();

        try
        {
            var msg = await _tasksApiService.CompleteTaskAsync(taskId, notes);
            InfoMessage = msg;
            IsCompletionFormVisible = false;
            await LoadTasksCoreAsync();
        }
        catch (Exception ex) when (IsNetworkError(ex))
        {
            // Queue for offline sync
            var payload = JsonSerializer.Serialize(new { taskId, notes });
            await _outboxService.EnqueueAsync(new OutboxItem
            {
                Type = "task.complete",
                PayloadJson = payload
            });
            InfoMessage = "No connection — queued for sync when back online.";
            IsCompletionFormVisible = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadTasksCoreAsync(CancellationToken cancellationToken = default)
    {
        SelectedTask = null;
        IsCompletionFormVisible = false;
        _allTasks = await _tasksApiService.GetMyTasksAsync(cancellationToken);
        ApplyFilter(ActiveFilter);
    }

    private void ApplyFilter(string filter)
    {
        var filtered = filter switch
        {
            "Pending" => _allTasks.Where(t => t.Status == "Pending"),
            "InProgress" => _allTasks.Where(t => t.Status == "InProgress"),
            "Completed" => _allTasks.Where(t => t.Status == "Completed"),
            _ => (IEnumerable<TaskModel>)_allTasks
        };

        Tasks = new ObservableCollection<TaskModel>(filtered);
        OnPropertyChanged(nameof(HasTasks));
        OnPropertyChanged(nameof(HasNoTasks));
    }

    private static bool IsNetworkError(Exception ex)
        => ex is HttpRequestException || ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase);
}
