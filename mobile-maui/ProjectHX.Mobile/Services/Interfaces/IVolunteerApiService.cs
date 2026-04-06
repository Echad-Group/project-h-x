using ProjectHX.Mobile.Models.Volunteer;

namespace ProjectHX.Mobile.Services.Interfaces;

public interface IVolunteerApiService
{
    Task<VolunteerStatusResponse> GetMyVolunteerStatusAsync(CancellationToken cancellationToken = default);
    Task<string> CreateVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default);
    Task<string> UpdateMyVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default);
    Task<string> LeaveVolunteerRoleAsync(CancellationToken cancellationToken = default);
}
