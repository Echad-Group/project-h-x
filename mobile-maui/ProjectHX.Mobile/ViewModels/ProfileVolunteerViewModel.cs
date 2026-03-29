using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class ProfileVolunteerViewModel : ProfileSectionViewModelBase
{
    private readonly IVolunteerApiService _volunteerApiService;
    private readonly Func<Task> _refreshVolunteerAsync;

    [ObservableProperty]
    private bool isVolunteer;

    [ObservableProperty]
    private string volunteerStatusSummary = "Not registered as a volunteer.";

    [ObservableProperty]
    private string volunteerName = string.Empty;

    [ObservableProperty]
    private string volunteerEmail = string.Empty;

    [ObservableProperty]
    private string volunteerPhone = string.Empty;

    [ObservableProperty]
    private string volunteerCity = string.Empty;

    [ObservableProperty]
    private string volunteerRegion = string.Empty;

    [ObservableProperty]
    private string volunteerAvailabilityZones = string.Empty;

    [ObservableProperty]
    private string volunteerSkills = string.Empty;

    [ObservableProperty]
    private string volunteerInterests = string.Empty;

    [ObservableProperty]
    private string volunteerHoursPerWeek = string.Empty;

    [ObservableProperty]
    private bool volunteerAvailableWeekends;

    [ObservableProperty]
    private bool volunteerAvailableEvenings;

    public ProfileVolunteerViewModel(BaseViewModel pageState, IVolunteerApiService volunteerApiService, Func<Task> refreshVolunteerAsync)
        : base(pageState)
    {
        _volunteerApiService = volunteerApiService;
        _refreshVolunteerAsync = refreshVolunteerAsync;
    }

    partial void OnIsVolunteerChanged(bool value)
    {
        VolunteerStatusSummary = value ? "Active volunteer profile" : "Not registered as a volunteer.";
    }

    public void ApplyStatus(VolunteerStatusResponse volunteerStatus)
    {
        IsVolunteer = volunteerStatus.IsVolunteer;

        if (volunteerStatus.IsVolunteer && volunteerStatus.Volunteer is not null)
        {
            ApplyVolunteer(volunteerStatus.Volunteer);
            return;
        }

        ClearVolunteerFields();
    }

    [RelayCommand]
    private async Task SaveVolunteerProfileAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (!IsVolunteer)
        {
            SetError("Volunteer profile was not found for your account.");
            return;
        }

        if (string.IsNullOrWhiteSpace(VolunteerName) || string.IsNullOrWhiteSpace(VolunteerEmail))
        {
            SetError("Volunteer name and email are required.");
            return;
        }

        int? hoursPerWeek = null;
        if (!string.IsNullOrWhiteSpace(VolunteerHoursPerWeek))
        {
            if (!int.TryParse(VolunteerHoursPerWeek.Trim(), out var parsedHours) || parsedHours < 0)
            {
                SetError("Hours per week must be a valid non-negative number.");
                return;
            }

            hoursPerWeek = parsedHours;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _volunteerApiService.UpdateMyVolunteerProfileAsync(new UpdateVolunteerRequest
            {
                Name = VolunteerName.Trim(),
                Email = VolunteerEmail.Trim(),
                Phone = NormalizeOptional(VolunteerPhone),
                City = NormalizeOptional(VolunteerCity),
                Region = NormalizeOptional(VolunteerRegion),
                AvailabilityZones = NormalizeOptional(VolunteerAvailabilityZones),
                Skills = NormalizeOptional(VolunteerSkills),
                HoursPerWeek = hoursPerWeek,
                AvailableWeekends = VolunteerAvailableWeekends,
                AvailableEvenings = VolunteerAvailableEvenings,
                Interests = NormalizeOptional(VolunteerInterests)
            });

            await _refreshVolunteerAsync();
            SetInfo(message);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    [RelayCommand]
    private async Task LeaveVolunteerRoleAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (!IsVolunteer)
        {
            SetInfo("You are currently not registered as a volunteer.");
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var message = await _volunteerApiService.LeaveVolunteerRoleAsync();
            await _refreshVolunteerAsync();
            SetInfo(message);
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
        }
        finally
        {
            FinishOperation();
        }
    }

    private void ApplyVolunteer(VolunteerProfileModel volunteer)
    {
        VolunteerName = volunteer.Name;
        VolunteerEmail = volunteer.Email;
        VolunteerPhone = volunteer.Phone ?? string.Empty;
        VolunteerCity = volunteer.City ?? string.Empty;
        VolunteerRegion = volunteer.Region ?? string.Empty;
        VolunteerAvailabilityZones = volunteer.AvailabilityZones ?? string.Empty;
        VolunteerSkills = volunteer.Skills ?? string.Empty;
        VolunteerInterests = volunteer.Interests ?? string.Empty;
        VolunteerHoursPerWeek = volunteer.HoursPerWeek?.ToString() ?? string.Empty;
        VolunteerAvailableWeekends = volunteer.AvailableWeekends;
        VolunteerAvailableEvenings = volunteer.AvailableEvenings;
        VolunteerStatusSummary = $"Active volunteer since {volunteer.CreatedAt:yyyy-MM-dd}";
    }

    private void ClearVolunteerFields()
    {
        VolunteerName = string.Empty;
        VolunteerEmail = string.Empty;
        VolunteerPhone = string.Empty;
        VolunteerCity = string.Empty;
        VolunteerRegion = string.Empty;
        VolunteerAvailabilityZones = string.Empty;
        VolunteerSkills = string.Empty;
        VolunteerInterests = string.Empty;
        VolunteerHoursPerWeek = string.Empty;
        VolunteerAvailableWeekends = false;
        VolunteerAvailableEvenings = false;
        VolunteerStatusSummary = "Not registered as a volunteer.";
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}