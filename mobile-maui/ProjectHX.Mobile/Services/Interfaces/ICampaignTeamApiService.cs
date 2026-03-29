using ProjectHX.Mobile.Models.PublicContent;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface ICampaignTeamApiService
{
    Task<List<CampaignTeamMemberModel>> GetTeamAsync(CancellationToken cancellationToken = default);
}
