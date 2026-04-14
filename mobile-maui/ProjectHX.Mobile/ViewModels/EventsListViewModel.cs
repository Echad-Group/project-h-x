using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class EventsListViewModel : BaseViewModel, IAsyncPageLoadable
{
    private readonly IEventsApiService _eventsApiService;
    private readonly IAppNavigator _appNavigator;

    [ObservableProperty]
    private ObservableCollection<CampaignEventModel> events = [];

    [ObservableProperty]
    private string? sortOption = "upcoming"; // upcoming, past, featured

    public EventsListViewModel(
        IEventsApiService eventsApiService,
        IAppNavigator appNavigator)
    {
        _eventsApiService = eventsApiService;
        _appNavigator = appNavigator;
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var events = await _eventsApiService.GetEventsAsync(cancellationToken);

            // Apply sorting based on sortOption
            var sorted = SortEvents(events);
            Events = new ObservableCollection<CampaignEventModel>(sorted);
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
    private Task ChangeSortAsync(string newSort)
    {
        SortOption = newSort;
        if (Events.Count > 0)
        {
            var sorted = SortEvents(Events.ToList());
            Events = new ObservableCollection<CampaignEventModel>(sorted);
        }

        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await LoadAsync();
    }

    [RelayCommand]
    private async Task OpenEventAsync(CampaignEventModel eventItem)
    {
        if (eventItem == null)
        {
            return;
        }

        await _appNavigator.GoToEventDetailAsync(eventItem.Id);
    }

    private List<CampaignEventModel> SortEvents(List<CampaignEventModel> events)
    {
        return SortOption switch
        {
            "upcoming" => events
                .Where(e => e.Date >= DateTime.UtcNow)
                .OrderBy(e => e.Date)
                .ToList(),
            "past" => events
                .Where(e => e.Date < DateTime.UtcNow)
                .OrderByDescending(e => e.Date)
                .ToList(),
            _ => events.OrderBy(e => e.Date).ToList()
        };
    }
}
