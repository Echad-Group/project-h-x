using ProjectHX.Mobile.Models.Inbox;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IInboxApiService
{
    Task<List<InboxMessage>> GetInboxAsync(CancellationToken cancellationToken = default);
    Task<string> MarkReadAsync(int messageId, CancellationToken cancellationToken = default);
}
