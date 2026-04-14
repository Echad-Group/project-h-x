using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class EventDetailViewModel : BaseViewModel
{
    private readonly IEventsApiService _eventsApiService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEvent))]
    [NotifyPropertyChangedFor(nameof(HasEventImages))]
    private CampaignEventModel? eventItem;

    [ObservableProperty]
    private string attendeeName = string.Empty;

    [ObservableProperty]
    private string attendeeEmail = string.Empty;

    [ObservableProperty]
    private string attendeePhone = string.Empty;

    [ObservableProperty]
    private string guestCount = "1";

    [ObservableProperty]
    private string specialRequirements = string.Empty;

    public bool HasEvent => EventItem != null;
    
    public bool HasEventImages => EventItem?.ImageUrls?.Count > 1;

    public EventDetailViewModel(IEventsApiService eventsApiService)
    {
        _eventsApiService = eventsApiService;
    }

    public async Task LoadByIdAsync(int id)
    {
        if (IsBusy || id <= 0)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            EventItem = await _eventsApiService.GetEventByIdAsync(id);
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

    public async Task LoadBySlugAsync(string slug)
    {
        if (IsBusy || string.IsNullOrWhiteSpace(slug))
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            EventItem = await _eventsApiService.GetEventBySlugAsync(slug);
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
    private async Task SubmitRsvpAsync()
    {
        if (EventItem == null || IsBusy)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(AttendeeName) || string.IsNullOrWhiteSpace(AttendeeEmail))
        {
            ErrorMessage = "Name and email are required for RSVP.";
            return;
        }

        if (!int.TryParse(GuestCount, out var parsedGuests) || parsedGuests < 1)
        {
            ErrorMessage = "Guest count must be at least 1.";
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            InfoMessage = await _eventsApiService.SubmitRsvpAsync(new EventRsvpRequestModel
            {
                EventId = EventItem.Id,
                Name = AttendeeName.Trim(),
                Email = AttendeeEmail.Trim(),
                Phone = string.IsNullOrWhiteSpace(AttendeePhone) ? null : AttendeePhone.Trim(),
                NumberOfGuests = parsedGuests,
                SpecialRequirements = string.IsNullOrWhiteSpace(SpecialRequirements) ? null : SpecialRequirements.Trim()
            });
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
}
