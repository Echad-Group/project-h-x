using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Inbox;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class InboxViewModel : BaseViewModel
{
    private readonly IInboxApiService _inboxApiService;

    [ObservableProperty]
    private ObservableCollection<InboxMessage> messages = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUnreadMessages))]
    private int unreadCount;

    public bool HasUnreadMessages => UnreadCount > 0;

    public InboxViewModel(IInboxApiService inboxApiService)
    {
        _inboxApiService = inboxApiService;
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var list = await _inboxApiService.GetInboxAsync();
            Messages = new ObservableCollection<InboxMessage>(list);
            UnreadCount = list.Count(m => m.IsUnread);
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
    private async System.Threading.Tasks.Task MarkReadAsync(InboxMessage message)
    {
        if (message == null || !message.IsUnread || IsBusy) return;

        ErrorMessage = null;

        try
        {
            await _inboxApiService.MarkReadAsync(message.Id);
            // Optimistic update — refresh the full list
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
