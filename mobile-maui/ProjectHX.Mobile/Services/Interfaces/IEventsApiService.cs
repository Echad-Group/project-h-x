using ProjectHX.Mobile.Models.PublicContent;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IEventsApiService
{
    Task<List<CampaignEventModel>> GetEventsAsync(CancellationToken cancellationToken = default);
    Task<CampaignEventModel> GetEventByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CampaignEventModel> GetEventBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<string> SubmitRsvpAsync(EventRsvpRequestModel request, CancellationToken cancellationToken = default);
}
