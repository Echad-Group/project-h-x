using ProjectHX.Mobile.Models.Results;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IResultsApiService
{
    Task<string> SubmitAsync(ResultSubmissionRequest request, CancellationToken cancellationToken = default);
}
