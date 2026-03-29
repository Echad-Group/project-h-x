using ProjectHX.Mobile.Models.PublicContent;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IIssuesApiService
{
    Task<List<IssueModel>> GetIssuesAsync(CancellationToken cancellationToken = default);
    Task<IssueModel> GetIssueByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IssueModel> GetIssueBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
