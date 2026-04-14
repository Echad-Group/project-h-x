using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Inbox;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class InboxViewModel : BaseViewModel, IAsyncPageLoadable
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

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var list = await _inboxApiService.GetInboxAsync(cancellationToken);
            Messages = new ObservableCollection<InboxMessage>(list);
            UnreadCount = list.Count(m => m.IsUnread);
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
    private async System.Threading.Tasks.Task MarkReadAsync(InboxMessage message)
    {
        if (message == null || !message.IsUnread || IsBusy) return;

        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            await _inboxApiService.MarkReadAsync(message.Id);
            message.ReadAt = DateTime.UtcNow;
            Messages = new ObservableCollection<InboxMessage>(Messages);
            UnreadCount = Messages.Count(m => m.IsUnread);
            InfoMessage = "Message marked as read.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
