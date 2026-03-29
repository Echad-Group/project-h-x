using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class ProfileDetailsViewModel : ProfileSectionViewModelBase
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly Func<Task> _refreshProfileAsync;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string phoneNumber = string.Empty;

    [ObservableProperty]
    private string bio = string.Empty;

    [ObservableProperty]
    private string location = string.Empty;

    [ObservableProperty]
    private string website = string.Empty;

    [ObservableProperty]
    private string twitter = string.Empty;

    [ObservableProperty]
    private string facebook = string.Empty;

    [ObservableProperty]
    private string linkedIn = string.Empty;

    [ObservableProperty]
    private string nationalIdNumber = string.Empty;

    [ObservableProperty]
    private string voterCardNumber = string.Empty;

    public ProfileDetailsViewModel(BaseViewModel pageState, IUserProfileApiService userProfileApiService, Func<Task> refreshProfileAsync)
        : base(pageState)
    {
        _userProfileApiService = userProfileApiService;
        _refreshProfileAsync = refreshProfileAsync;
    }

    public void ApplyProfile(UserProfileModel profile)
    {
        Email = profile.Email;
        FirstName = profile.FirstName ?? string.Empty;
        LastName = profile.LastName ?? string.Empty;
        PhoneNumber = profile.PhoneNumber ?? string.Empty;
        Bio = profile.Bio ?? string.Empty;
        Location = profile.Location ?? string.Empty;
        Website = profile.Website ?? string.Empty;
        Twitter = profile.Twitter ?? string.Empty;
        Facebook = profile.Facebook ?? string.Empty;
        LinkedIn = profile.LinkedIn ?? string.Empty;
        NationalIdNumber = profile.NationalIdNumber ?? string.Empty;
        VoterCardNumber = profile.VoterCardNumber ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            SetError("First name and last name are required.");
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            await _userProfileApiService.UpdateProfileAsync(new UpdateProfileRequest
            {
                FirstName = FirstName.Trim(),
                LastName = LastName.Trim(),
                PhoneNumber = NormalizeOptional(PhoneNumber),
                Bio = NormalizeOptional(Bio),
                Location = NormalizeOptional(Location),
                Website = NormalizeOptional(Website),
                Twitter = NormalizeOptional(Twitter),
                Facebook = NormalizeOptional(Facebook),
                LinkedIn = NormalizeOptional(LinkedIn),
                NationalIdNumber = NormalizeOptional(NationalIdNumber),
                VoterCardNumber = NormalizeOptional(VoterCardNumber)
            });

            await _refreshProfileAsync();
            SetInfo("Profile updated successfully.");
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

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}