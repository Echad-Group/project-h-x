using ProjectHX.Mobile.Models.PublicContent;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IPushApiService
{
    Task<string> SubscribeAsync(PushSubscriptionRequestModel request, CancellationToken cancellationToken = default);
    Task<string> UnsubscribeAsync(string endpoint, CancellationToken cancellationToken = default);
    Task<PushStatusModel> GetStatusAsync(CancellationToken cancellationToken = default);
}
