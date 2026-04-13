using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class ProfileIdentityViewModel : ProfileSectionViewModelBase
{
    private readonly IUserProfileApiService _userProfileApiService;
    private readonly IApiBaseUrlProvider _apiBaseUrlProvider;
    private readonly Func<Task> _refreshProfileAsync;

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
    [NotifyPropertyChangedFor(nameof(HasNidaDocument))]
    private string? nidaDocumentUrl;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasVoterCardDocument))]
    private string? voterCardDocumentUrl;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelfieDocument))]
    [NotifyPropertyChangedFor(nameof(HasSelfiePreview))]
    private string? selfieDocumentUrl;

    [ObservableProperty]
    private string nidaDocumentStatus = "Not uploaded";

    [ObservableProperty]
    private string voterCardDocumentStatus = "Not uploaded";

    [ObservableProperty]
    private string selfieDocumentStatus = "Not uploaded";

    public bool HasProfilePhoto => !string.IsNullOrWhiteSpace(ProfilePhotoUrl);
    public bool HasNidaDocument => !string.IsNullOrWhiteSpace(NidaDocumentUrl);
    public bool HasVoterCardDocument => !string.IsNullOrWhiteSpace(VoterCardDocumentUrl);
    public bool HasSelfieDocument => !string.IsNullOrWhiteSpace(SelfieDocumentUrl);
    public bool HasSelfiePreview => IsImageUrl(SelfieDocumentUrl);

    public ProfileIdentityViewModel(
        BaseViewModel pageState,
        IUserProfileApiService userProfileApiService,
        IApiBaseUrlProvider apiBaseUrlProvider,
        Func<Task> refreshProfileAsync)
        : base(pageState)
    {
        _userProfileApiService = userProfileApiService;
        _apiBaseUrlProvider = apiBaseUrlProvider;
        _refreshProfileAsync = refreshProfileAsync;
    }

    partial void OnProfilePhotoUrlChanged(string? value)
    {
        OnPropertyChanged(nameof(HasProfilePhoto));
    }

    public void ApplyProfile(UserProfileModel profile)
    {
        VerificationStatus = profile.VerificationStatus;
        VoterCardStatus = profile.VoterCardStatus;
        RolesSummary = profile.Roles.Count == 0 ? "Member" : string.Join(", ", profile.Roles);
        ProfilePhotoUrl = BuildAbsoluteUrl(profile.ProfilePhotoUrl);
        NidaDocumentUrl = BuildAbsoluteUrl(profile.IdImageUrl);
        VoterCardDocumentUrl = BuildAbsoluteUrl(profile.VoterCardImageUrl);
        SelfieDocumentUrl = BuildAbsoluteUrl(profile.SelfieImageUrl);
        NidaDocumentStatus = BuildDocumentStatus("National ID", NidaDocumentUrl);
        VoterCardDocumentStatus = BuildDocumentStatus("Voter card", VoterCardDocumentUrl);
        SelfieDocumentStatus = BuildDocumentStatus("Selfie", SelfieDocumentUrl);
        UpdateCompleteness(profile);
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
        if (!TryStartOperation())
        {
            return;
        }

        ClearMessages();

        try
        {
            var message = await _userProfileApiService.DeleteProfilePhotoAsync();
            await _refreshProfileAsync();
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
    private async Task OpenNidaDocumentAsync()
    {
        await OpenUrlAsync(NidaDocumentUrl);
    }

    [RelayCommand]
    private async Task OpenVoterCardDocumentAsync()
    {
        await OpenUrlAsync(VoterCardDocumentUrl);
    }

    [RelayCommand]
    private async Task OpenSelfieDocumentAsync()
    {
        await OpenUrlAsync(SelfieDocumentUrl);
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
        if (IsPageBusy)
        {
            return;
        }

        ClearMessages();

        var file = await FilePicker.Default.PickAsync(pickOptions);
        if (file is null)
        {
            return;
        }

        if (!TryStartOperation())
        {
            return;
        }

        try
        {
            await using var stream = await file.OpenReadAsync();
            var message = await uploadAsync(stream, file.FileName, ResolveContentType(file), CancellationToken.None);
            await _refreshProfileAsync();
            SetInfo(string.IsNullOrWhiteSpace(message) ? successMessage : message);
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

    private static string BuildDocumentStatus(string label, string? url)
    {
        return string.IsNullOrWhiteSpace(url) ? $"{label}: not uploaded" : $"{label}: uploaded";
    }

    private static bool IsImageUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        var path = Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri) ? absoluteUri.AbsolutePath : url;
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" or ".svg";
    }

    private static async Task OpenUrlAsync(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        await Browser.Default.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
    }

    private void UpdateCompleteness(UserProfileModel profile)
    {
        var checks = new[]
        {
            !string.IsNullOrWhiteSpace(profile.FirstName),
            !string.IsNullOrWhiteSpace(profile.LastName),
            !string.IsNullOrWhiteSpace(profile.PhoneNumber),
            !string.IsNullOrWhiteSpace(profile.Location),
            !string.IsNullOrWhiteSpace(profile.NationalIdNumber),
            !string.IsNullOrWhiteSpace(profile.VoterCardNumber)
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
}