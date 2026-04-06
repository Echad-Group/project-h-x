using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class ProfileVolunteerViewModel : ProfileSectionViewModelBase
{
    private readonly IVolunteerApiService _volunteerApiService;
    private readonly Func<Task> _refreshVolunteerAsync;
    private readonly IAppNavigator? _appNavigator;

    private bool _isSyncingSelections;
    private string _memberNameHint = string.Empty;
    private string _memberEmailHint = string.Empty;
    private string _memberRegionHint = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VolunteerActionText))]
    [NotifyPropertyChangedFor(nameof(CanLeaveVolunteer))]
    [NotifyPropertyChangedFor(nameof(ShowVolunteerDashboard))]
    [NotifyPropertyChangedFor(nameof(ShowVolunteerForm))]
    [NotifyPropertyChangedFor(nameof(CanCancelEdit))]
    private bool isVolunteer;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowVolunteerDashboard))]
    [NotifyPropertyChangedFor(nameof(ShowVolunteerForm))]
    [NotifyPropertyChangedFor(nameof(ShowSuccessState))]
    [NotifyPropertyChangedFor(nameof(CanCancelEdit))]
    private bool isEditMode;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowVolunteerDashboard))]
    [NotifyPropertyChangedFor(nameof(ShowVolunteerForm))]
    [NotifyPropertyChangedFor(nameof(ShowSuccessState))]
    private bool hasCompletedSignup;

    [ObservableProperty]
    private string volunteerStatusSummary = "Not registered as a volunteer yet.";

    [ObservableProperty]
    private string volunteerName = string.Empty;

    [ObservableProperty]
    private string volunteerEmail = string.Empty;

    [ObservableProperty]
    private string volunteerPhone = string.Empty;

    [ObservableProperty]
    private string volunteerCity = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AssignmentPreferenceSummary))]
    private string volunteerRegion = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AssignmentPreferenceSummary))]
    private string volunteerAvailabilityZones = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AssignmentPreferenceSummary))]
    private string volunteerSkills = string.Empty;

    [ObservableProperty]
    private string volunteerInterests = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HoursPerWeekSummary))]
    private int volunteerHoursPerWeek = 5;

    [ObservableProperty]
    private bool volunteerAvailableWeekends;

    [ObservableProperty]
    private bool volunteerAvailableEvenings;

    public ProfileVolunteerViewModel(
        BaseViewModel pageState,
        IVolunteerApiService volunteerApiService,
        Func<Task> refreshVolunteerAsync,
        IAppNavigator? appNavigator = null)
        : base(pageState)
    {
        _volunteerApiService = volunteerApiService;
        _refreshVolunteerAsync = refreshVolunteerAsync;
        _appNavigator = appNavigator;

        InitializeSelectionOptions();
    }

    public ObservableCollection<SelectableOptionItem> AvailabilityZoneOptions { get; } = [];

    public ObservableCollection<SelectableOptionItem> SkillOptions { get; } = [];

    public IReadOnlyList<string> RegionOptions { get; } =
    [
        "Nairobi",
        "Coast",
        "Western",
        "Eastern",
        "Central",
        "Rift Valley",
        "Nyanza",
        "North Eastern"
    ];

    public string VolunteerActionText => IsVolunteer ? "Save Volunteer Profile" : "Join Volunteer Team";

    public bool CanLeaveVolunteer => IsVolunteer;

    public bool ShowVolunteerDashboard => IsVolunteer && !IsEditMode && !HasCompletedSignup;

    public bool ShowVolunteerForm => !HasCompletedSignup && (!IsVolunteer || IsEditMode);

    public bool ShowSuccessState => HasCompletedSignup;

    public bool CanCancelEdit => IsVolunteer && IsEditMode;

    public string HoursPerWeekSummary => VolunteerHoursPerWeek <= 0
        ? "Availability: On request"
        : $"Availability: about {VolunteerHoursPerWeek} hour{(VolunteerHoursPerWeek == 1 ? string.Empty : "s")} per week";

    public string AssignmentPreferenceSummary
    {
        get
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(VolunteerRegion))
            {
                parts.Add($"Region: {VolunteerRegion.Trim()}");
            }

            if (!string.IsNullOrWhiteSpace(VolunteerAvailabilityZones))
            {
                parts.Add($"Areas: {VolunteerAvailabilityZones.Trim()}");
            }

            if (!string.IsNullOrWhiteSpace(VolunteerSkills))
            {
                parts.Add($"Skills: {VolunteerSkills.Trim()}");
            }

            return parts.Count == 0
                ? "Choose your region, coverage areas, and skills so team leads can route the right project assignments to you."
                : $"Assignment matching uses {string.Join(" • ", parts)}.";
        }
    }

    partial void OnIsVolunteerChanged(bool value)
    {
        VolunteerStatusSummary = value
            ? "Active volunteer profile"
            : "Not registered as a volunteer yet. Complete the form below to join a team and receive assignment updates.";
    }

    partial void OnVolunteerAvailabilityZonesChanged(string value)
    {
        SyncOptionSelections(AvailabilityZoneOptions, value);
        OnPropertyChanged(nameof(AssignmentPreferenceSummary));
    }

    partial void OnVolunteerSkillsChanged(string value)
    {
        SyncOptionSelections(SkillOptions, value);
        OnPropertyChanged(nameof(AssignmentPreferenceSummary));
    }

    public void ApplyMemberProfile(UserProfileModel profile)
    {
        _memberNameHint = string.Join(" ", new[] { profile.FirstName, profile.LastName }.Where(item => !string.IsNullOrWhiteSpace(item))).Trim();
        _memberEmailHint = profile.Email ?? string.Empty;
        _memberRegionHint = profile.Location ?? string.Empty;

        if (!IsVolunteer)
        {
            VolunteerName = string.IsNullOrWhiteSpace(VolunteerName) ? _memberNameHint : VolunteerName;
            VolunteerEmail = string.IsNullOrWhiteSpace(VolunteerEmail) ? _memberEmailHint : VolunteerEmail;
            VolunteerRegion = string.IsNullOrWhiteSpace(VolunteerRegion) ? _memberRegionHint : VolunteerRegion;
        }
    }

    public void ApplyStatus(VolunteerStatusResponse volunteerStatus)
    {
        IsVolunteer = volunteerStatus.IsVolunteer;

        if (volunteerStatus.IsVolunteer && volunteerStatus.Volunteer is not null)
        {
            ApplyVolunteer(volunteerStatus.Volunteer);
            return;
        }

        IsEditMode = false;
        HasCompletedSignup = false;
        ClearVolunteerFields();
    }

    [RelayCommand]
    private void BeginEditing()
    {
        if (!IsVolunteer)
        {
            return;
        }

        ClearMessages();
        HasCompletedSignup = false;
        IsEditMode = true;
    }

    [RelayCommand]
    private async Task CancelEditingAsync()
    {
        ClearMessages();
        HasCompletedSignup = false;
        IsEditMode = false;

        if (IsVolunteer)
        {
            await _refreshVolunteerAsync();
        }
    }

    [RelayCommand]
    private void DismissSuccess()
    {
        ClearMessages();
        HasCompletedSignup = false;
        IsEditMode = false;
    }

    [RelayCommand]
    private async Task ViewAssignmentsAsync()
    {
        DismissSuccess();

        if (_appNavigator is not null)
        {
            await _appNavigator.GoToTasksAsync();
        }
    }

    [RelayCommand]
    private async Task SaveVolunteerProfileAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (string.IsNullOrWhiteSpace(VolunteerName) || string.IsNullOrWhiteSpace(VolunteerEmail))
        {
            SetError("Volunteer name and email are required.");
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            var availabilityZones = ResolveJoinedSelections(AvailabilityZoneOptions, VolunteerAvailabilityZones);
            var skills = ResolveJoinedSelections(SkillOptions, VolunteerSkills);

            var request = new UpdateVolunteerRequest
            {
                Name = VolunteerName.Trim(),
                Email = VolunteerEmail.Trim(),
                Phone = NormalizeOptional(VolunteerPhone),
                City = NormalizeOptional(VolunteerCity),
                Region = NormalizeOptional(VolunteerRegion),
                AvailabilityZones = NormalizeOptional(availabilityZones),
                Skills = NormalizeOptional(skills),
                HoursPerWeek = VolunteerHoursPerWeek,
                AvailableWeekends = VolunteerAvailableWeekends,
                AvailableEvenings = VolunteerAvailableEvenings,
                Interests = NormalizeOptional(VolunteerInterests)
            };

            var wasVolunteer = IsVolunteer;
            var message = wasVolunteer
                ? await _volunteerApiService.UpdateMyVolunteerProfileAsync(request)
                : await _volunteerApiService.CreateVolunteerProfileAsync(request);

            await _refreshVolunteerAsync();
            IsEditMode = false;
            HasCompletedSignup = !wasVolunteer;

            SetInfo(string.IsNullOrWhiteSpace(message)
                ? wasVolunteer
                    ? "Volunteer profile updated successfully."
                    : "Volunteer sign-up completed. You will now receive project updates for your selected areas and skills."
                : message);
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
            HasCompletedSignup = false;
            IsEditMode = false;
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

    private void InitializeSelectionOptions()
    {
        foreach (var zone in new[] { "Central", "East", "West", "North", "South", "Suburbs", "Rural", "Urban" })
        {
            AvailabilityZoneOptions.Add(CreateOption(zone));
        }

        foreach (var skill in new[]
        {
            ("Design", "🎨"),
            ("Social Media", "📱"),
            ("Logistics", "📦"),
            ("Public Speaking", "🎤"),
            ("Fundraising", "💰"),
            ("Tech Support", "💻"),
            ("Event Planning", "📅"),
            ("Community Outreach", "🤝")
        })
        {
            SkillOptions.Add(CreateOption(skill.Item1, skill.Item2));
        }
    }

    private SelectableOptionItem CreateOption(string label, string? icon = null)
    {
        var option = new SelectableOptionItem(label, icon);
        option.PropertyChanged += OnSelectionOptionChanged;
        return option;
    }

    private void OnSelectionOptionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectableOptionItem.IsSelected))
        {
            SyncJoinedSelectionsFromOptions();
        }
    }

    private void SyncJoinedSelectionsFromOptions()
    {
        if (_isSyncingSelections)
        {
            return;
        }

        try
        {
            _isSyncingSelections = true;
            VolunteerAvailabilityZones = string.Join(", ", AvailabilityZoneOptions.Where(item => item.IsSelected).Select(item => item.Value));
            VolunteerSkills = string.Join(", ", SkillOptions.Where(item => item.IsSelected).Select(item => item.Value));
        }
        finally
        {
            _isSyncingSelections = false;
            OnPropertyChanged(nameof(AssignmentPreferenceSummary));
        }
    }

    private void SyncOptionSelections(IEnumerable<SelectableOptionItem> options, string rawValue)
    {
        if (_isSyncingSelections)
        {
            return;
        }

        try
        {
            _isSyncingSelections = true;
            var selectedValues = ParseCsvValues(rawValue).ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var option in options)
            {
                option.IsSelected = selectedValues.Contains(option.Value);
            }
        }
        finally
        {
            _isSyncingSelections = false;
            OnPropertyChanged(nameof(AssignmentPreferenceSummary));
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
        VolunteerHoursPerWeek = volunteer.HoursPerWeek ?? 5;
        VolunteerAvailableWeekends = volunteer.AvailableWeekends;
        VolunteerAvailableEvenings = volunteer.AvailableEvenings;
        VolunteerStatusSummary = $"Active volunteer since {volunteer.CreatedAt:yyyy-MM-dd}";

        SyncOptionSelections(AvailabilityZoneOptions, VolunteerAvailabilityZones);
        SyncOptionSelections(SkillOptions, VolunteerSkills);
    }

    private void ClearVolunteerFields()
    {
        VolunteerName = _memberNameHint;
        VolunteerEmail = _memberEmailHint;
        VolunteerPhone = string.Empty;
        VolunteerCity = string.Empty;
        VolunteerRegion = _memberRegionHint;
        VolunteerAvailabilityZones = string.Empty;
        VolunteerSkills = string.Empty;
        VolunteerInterests = string.Empty;
        VolunteerHoursPerWeek = 5;
        VolunteerAvailableWeekends = false;
        VolunteerAvailableEvenings = false;
        VolunteerStatusSummary = "Not registered as a volunteer yet. Complete the form below to join a team and receive assignment updates.";

        SyncOptionSelections(AvailabilityZoneOptions, VolunteerAvailabilityZones);
        SyncOptionSelections(SkillOptions, VolunteerSkills);
    }

    private static string ResolveJoinedSelections(IEnumerable<SelectableOptionItem> options, string rawValue)
    {
        var selected = options.Where(item => item.IsSelected).Select(item => item.Value).ToList();
        if (selected.Count > 0)
        {
            return string.Join(", ", selected);
        }

        return string.Join(", ", ParseCsvValues(rawValue));
    }

    private static IEnumerable<string> ParseCsvValues(string? rawValue)
    {
        return (rawValue ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(item => !string.IsNullOrWhiteSpace(item));
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}