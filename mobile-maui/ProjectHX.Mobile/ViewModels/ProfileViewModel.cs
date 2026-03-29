using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly IVolunteerApiService _volunteerApiService;
    private readonly IAuthApiService _authApiService;
    private readonly ISessionService _sessionService;
    private readonly IApiBaseUrlProvider _apiBaseUrlProvider;
    private readonly IPushApiService _pushApiService;

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

    [ObservableProperty]
    private string verificationStatus = string.Empty;

    [ObservableProperty]
    private string voterCardStatus = string.Empty;

    [ObservableProperty]
    private string rolesSummary = string.Empty;

    [ObservableProperty]
    private string completionSummary = string.Empty;

    [ObservableProperty]
    private double completionProgress;

    [ObservableProperty]
    private string? profilePhotoUrl;

    [ObservableProperty]
    private string nidaDocumentStatus = "Not uploaded";

    [ObservableProperty]
    private string voterCardDocumentStatus = "Not uploaded";

    [ObservableProperty]
    private string selfieDocumentStatus = "Not uploaded";

    [ObservableProperty]
    private string newEmail = string.Empty;

    [ObservableProperty]
    private string emailPassword = string.Empty;

    [ObservableProperty]
    private string currentPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string deletePassword = string.Empty;

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

    [ObservableProperty]
    private bool isPushSubscribed;

    [ObservableProperty]
    private string pushStatusSummary = "Push status not loaded.";

    public bool HasProfilePhoto => !string.IsNullOrWhiteSpace(ProfilePhotoUrl);

    public ProfileViewModel(
        IUserProfileApiService userProfileApiService,
        IVolunteerApiService volunteerApiService,
        IAuthApiService authApiService,
        ISessionService sessionService,
        IApiBaseUrlProvider apiBaseUrlProvider,
        IPushApiService pushApiService)
    {
        _userProfileApiService = userProfileApiService;
        _volunteerApiService = volunteerApiService;
        _authApiService = authApiService;
        _sessionService = sessionService;
        _apiBaseUrlProvider = apiBaseUrlProvider;
        _pushApiService = pushApiService;
    }

    partial void OnIsVolunteerChanged(bool value)
    {
        VolunteerStatusSummary = value ? "Active volunteer profile" : "Not registered as a volunteer.";
    }

    partial void OnProfilePhotoUrlChanged(string? value)
    {
        OnPropertyChanged(nameof(HasProfilePhoto));
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await LoadProfileDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveProfileAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = "First name and last name are required.";
            return;
        }

        IsBusy = true;

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

            await LoadProfileDataAsync();
            InfoMessage = "Profile updated successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadProfilePhotoAsync()
    {
        await UploadFileAsync(
            pickOptions: new PickOptions
            {
                PickerTitle = "Select a profile photo",
                FileTypes = FilePickerFileType.Images
            },
            uploadAsync: (stream, fileName, contentType, cancellationToken) => _userProfileApiService.UploadProfilePhotoAsync(stream, fileName, contentType, cancellationToken),
            successMessage: "Profile photo uploaded successfully.");
    }

    [RelayCommand]
    private async Task DeleteProfilePhotoAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;
        IsBusy = true;

        try
        {
            InfoMessage = await _userProfileApiService.DeleteProfilePhotoAsync();
            await LoadProfileDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UploadNidaDocumentAsync()
    {
        await UploadVerificationDocumentInternalAsync("nida", "Select NIDA document");
    }

    [RelayCommand]
    private async Task UploadVoterCardDocumentAsync()
    {
        await UploadVerificationDocumentInternalAsync("voter-card", "Select voter card document");
    }

    [RelayCommand]
    private async Task UploadSelfieDocumentAsync()
    {
        await UploadFileAsync(
            pickOptions: new PickOptions
            {
                PickerTitle = "Select verification selfie",
                FileTypes = FilePickerFileType.Images
            },
            uploadAsync: (stream, fileName, contentType, cancellationToken) => _userProfileApiService.UploadVerificationDocumentAsync(stream, fileName, contentType, "selfie", cancellationToken),
            successMessage: "Selfie uploaded successfully.");
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "All password fields are required.";
            return;
        }

        if (!string.Equals(NewPassword, ConfirmPassword, StringComparison.Ordinal))
        {
            ErrorMessage = "New password and confirmation do not match.";
            return;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _userProfileApiService.ChangePasswordAsync(new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            });

            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UpdateEmailAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(NewEmail) || string.IsNullOrWhiteSpace(EmailPassword))
        {
            ErrorMessage = "New email and current password are required.";
            return;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _userProfileApiService.UpdateEmailAsync(new UpdateEmailRequest
            {
                NewEmail = NewEmail.Trim(),
                Password = EmailPassword
            });

            EmailPassword = string.Empty;
            await LoadProfileDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;
        IsBusy = true;

        try
        {
            await _authApiService.LogoutAsync();
        }
        catch
        {
        }
        finally
        {
            await _sessionService.ClearAsync();
            IsBusy = false;
            await Shell.Current.GoToAsync("//login");
        }
    }

    [RelayCommand]
    private async Task EnablePushAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var endpoint = ResolvePushEndpoint();
            InfoMessage = await _pushApiService.SubscribeAsync(new PushSubscriptionRequestModel
            {
                Endpoint = endpoint,
                Keys = new PushKeysModel
                {
                    P256dh = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                    Auth = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                }
            });

            await LoadPushStatusAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DisablePushAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            InfoMessage = await _pushApiService.UnsubscribeAsync(ResolvePushEndpoint());
            await LoadPushStatusAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshPushStatusAsync()
    {
        await LoadPushStatusAsync();
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (string.IsNullOrWhiteSpace(DeletePassword))
        {
            ErrorMessage = "Password is required to delete the account.";
            return;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _userProfileApiService.DeleteAccountAsync(DeletePassword);
            DeletePassword = string.Empty;
            await _sessionService.ClearAsync();
            await Shell.Current.GoToAsync("//login");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveVolunteerProfileAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (!IsVolunteer)
        {
            ErrorMessage = "Volunteer profile was not found for your account.";
            return;
        }

        if (string.IsNullOrWhiteSpace(VolunteerName) || string.IsNullOrWhiteSpace(VolunteerEmail))
        {
            ErrorMessage = "Volunteer name and email are required.";
            return;
        }

        int? hoursPerWeek = null;
        if (!string.IsNullOrWhiteSpace(VolunteerHoursPerWeek))
        {
            if (!int.TryParse(VolunteerHoursPerWeek.Trim(), out var parsedHours) || parsedHours < 0)
            {
                ErrorMessage = "Hours per week must be a valid non-negative number.";
                return;
            }

            hoursPerWeek = parsedHours;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _volunteerApiService.UpdateMyVolunteerProfileAsync(new UpdateVolunteerRequest
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

            await LoadVolunteerDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LeaveVolunteerRoleAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        if (!IsVolunteer)
        {
            InfoMessage = "You are currently not registered as a volunteer.";
            return;
        }

        IsBusy = true;

        try
        {
            InfoMessage = await _volunteerApiService.LeaveVolunteerRoleAsync();
            IsVolunteer = false;
            ClearVolunteerFields();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyProfile(UserProfileModel profile)
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
        VerificationStatus = profile.VerificationStatus;
        VoterCardStatus = profile.VoterCardStatus;
        RolesSummary = profile.Roles.Count == 0 ? "Member" : string.Join(", ", profile.Roles);
        NewEmail = profile.Email;
        ProfilePhotoUrl = BuildAbsoluteUrl(profile.ProfilePhotoUrl);
        NidaDocumentStatus = BuildDocumentStatus("National ID", profile.IdImageUrl);
        VoterCardDocumentStatus = BuildDocumentStatus("Voter card", profile.VoterCardImageUrl);
        SelfieDocumentStatus = BuildDocumentStatus("Selfie", profile.SelfieImageUrl);
        UpdateCompleteness();
    }

    private async Task LoadProfileDataAsync()
    {
        var profile = await _userProfileApiService.GetProfileAsync();
        ApplyProfile(profile);
        await LoadVolunteerDataAsync();
        await LoadPushStatusAsync();
    }

    private async Task LoadPushStatusAsync()
    {
        try
        {
            var status = await _pushApiService.GetStatusAsync();
            IsPushSubscribed = status.IsSubscribed;
            PushStatusSummary = status.IsSubscribed
                ? $"Subscribed since {status.SubscriptionDate?.ToLocalTime():d MMM yyyy HH:mm}"
                : "Push notifications are disabled on this device.";
        }
        catch
        {
            IsPushSubscribed = false;
            PushStatusSummary = "Unable to read push status right now.";
        }
    }

    private async Task LoadVolunteerDataAsync()
    {
        var volunteerStatus = await _volunteerApiService.GetMyVolunteerStatusAsync();
        IsVolunteer = volunteerStatus.IsVolunteer;

        if (volunteerStatus.IsVolunteer && volunteerStatus.Volunteer is not null)
        {
            ApplyVolunteer(volunteerStatus.Volunteer);
            return;
        }

        ClearVolunteerFields();
    }

    private async Task UploadVerificationDocumentInternalAsync(string documentType, string pickerTitle)
    {
        await UploadFileAsync(
            pickOptions: new PickOptions
            {
                PickerTitle = pickerTitle,
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" } },
                    { DevicePlatform.Android, new[] { "image/*", "application/pdf" } },
                    { DevicePlatform.iOS, new[] { "public.image", "com.adobe.pdf" } },
                    { DevicePlatform.MacCatalyst, new[] { "public.image", "com.adobe.pdf" } }
                })
            },
            uploadAsync: (stream, fileName, contentType, cancellationToken) => _userProfileApiService.UploadVerificationDocumentAsync(stream, fileName, contentType, documentType, cancellationToken),
            successMessage: "Verification document uploaded successfully.");
    }

    private async Task UploadFileAsync(
        PickOptions pickOptions,
        Func<Stream, string, string, CancellationToken, Task<string>> uploadAsync,
        string successMessage)
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        var file = await FilePicker.Default.PickAsync(pickOptions);
        if (file is null)
        {
            return;
        }

        IsBusy = true;

        try
        {
            await using var stream = await file.OpenReadAsync();
            InfoMessage = await uploadAsync(stream, file.FileName, ResolveContentType(file), CancellationToken.None);
            await LoadProfileDataAsync();

            if (string.IsNullOrWhiteSpace(InfoMessage))
            {
                InfoMessage = successMessage;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string? BuildAbsoluteUrl(string? relativeOrAbsoluteUrl)
    {
        if (string.IsNullOrWhiteSpace(relativeOrAbsoluteUrl))
        {
            return null;
        }

        if (Uri.TryCreate(relativeOrAbsoluteUrl, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        var apiBaseUri = _apiBaseUrlProvider.GetBaseUri();
        var siteRoot = new Uri(apiBaseUri.GetLeftPart(UriPartial.Authority));
        return new Uri(siteRoot, relativeOrAbsoluteUrl).ToString();
    }

    private string BuildDocumentStatus(string label, string? url)
    {
        return string.IsNullOrWhiteSpace(url) ? $"{label}: not uploaded" : $"{label}: uploaded";
    }

    private void UpdateCompleteness()
    {
        var checks = new[]
        {
            !string.IsNullOrWhiteSpace(FirstName),
            !string.IsNullOrWhiteSpace(LastName),
            !string.IsNullOrWhiteSpace(PhoneNumber),
            !string.IsNullOrWhiteSpace(Location),
            !string.IsNullOrWhiteSpace(NationalIdNumber),
            !string.IsNullOrWhiteSpace(VoterCardNumber)
        };

        var complete = checks.Count(item => item);
        CompletionProgress = checks.Length == 0 ? 0 : (double)complete / checks.Length;
        CompletionSummary = $"{complete}/{checks.Length} core profile items complete";
    }

    private static string ResolveContentType(FileResult file)
    {
        if (!string.IsNullOrWhiteSpace(file.ContentType))
        {
            return file.ContentType;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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
    }

    private static string ResolvePushEndpoint()
        => $"maui://device/{DeviceInfo.Platform}/{DeviceInfo.Model}/{DeviceInfo.Name}";
}
