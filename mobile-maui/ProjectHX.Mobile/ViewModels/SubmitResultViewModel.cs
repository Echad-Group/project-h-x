using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Media;
using Microsoft.Maui.Networking;
using ProjectHX.Mobile.Infrastructure.Outbox;
using ProjectHX.Mobile.Models.Results;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.ViewModels;

public sealed partial class SubmitResultViewModel : BaseViewModel
{
    private readonly IResultsApiService _resultsApiService;
    private readonly ISyncOutboxService _outboxService;

    [ObservableProperty]
    private string pollingStationCode = string.Empty;

    [ObservableProperty]
    private string constituency = string.Empty;

    [ObservableProperty]
    private string county = string.Empty;

    [ObservableProperty]
    private string region = string.Empty;

    [ObservableProperty]
    private string candidateA = string.Empty;

    [ObservableProperty]
    private string candidateB = string.Empty;

    [ObservableProperty]
    private string candidateC = string.Empty;

    [ObservableProperty]
    private string rejectedVotes = string.Empty;

    [ObservableProperty]
    private string registeredVoters = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasLocation))]
    [NotifyPropertyChangedFor(nameof(LocationSummary))]
    private decimal? latitude;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasLocation))]
    [NotifyPropertyChangedFor(nameof(LocationSummary))]
    private decimal? longitude;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasEvidence))]
    [NotifyPropertyChangedFor(nameof(EvidenceSummary))]
    private string? evidenceFilePath;

    [ObservableProperty]
    private int pendingOutboxCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDeadLetters))]
    private int deadLetterCount;

    [ObservableProperty]
    private string syncSummary = "Checking sync status...";

    public bool HasLocation => Latitude.HasValue && Longitude.HasValue;
    public bool HasEvidence => !string.IsNullOrWhiteSpace(EvidenceFilePath);
    public bool HasDeadLetters => DeadLetterCount > 0;
    public string LocationSummary => HasLocation ? $"Lat {Latitude:0.000000}, Lon {Longitude:0.000000}" : "Location not captured";
    public string EvidenceSummary => HasEvidence ? Path.GetFileName(EvidenceFilePath) ?? string.Empty : "No evidence attached";

    public SubmitResultViewModel(IResultsApiService resultsApiService, ISyncOutboxService outboxService)
    {
        _resultsApiService = resultsApiService;
        _outboxService = outboxService;
        _outboxService.StatusChanged += OnOutboxStatusChanged;
    }

    public async Task LoadAsync()
    {
        await _outboxService.InitializeAsync();
        await LoadSyncStatusAsync();
    }

    [RelayCommand]
    private async Task CaptureLocationAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request) ?? await Geolocation.Default.GetLastKnownLocationAsync();
            if (location == null)
            {
                throw new InvalidOperationException("Unable to determine your location right now.");
            }

            Latitude = (decimal)location.Latitude;
            Longitude = (decimal)location.Longitude;
            OnPropertyChanged(nameof(LocationSummary));
            InfoMessage = "Location captured.";
        }
        catch (Exception ex) when (ex is FeatureNotSupportedException or PermissionException)
        {
            ErrorMessage = "Location permission is required to capture coordinates.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task CapturePhotoAsync()
    {
        await AcquireEvidenceAsync(async () => await MediaPicker.Default.CapturePhotoAsync());
    }

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        await AcquireEvidenceAsync(async () => await MediaPicker.Default.PickPhotoAsync());
    }

    [RelayCommand]
    private void ClearEvidence()
    {
        EvidenceFilePath = null;
        OnPropertyChanged(nameof(EvidenceSummary));
    }

    [RelayCommand]
    private async Task SubmitAsync()
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
            var request = BuildRequest();

            if (!HasInternet())
            {
                await QueueSubmissionAsync(request);
                return;
            }

            var message = await _resultsApiService.SubmitAsync(request);
            InfoMessage = message;
            ClearForm();
        }
        catch (HttpRequestException)
        {
            var request = BuildRequest();
            await QueueSubmissionAsync(request);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            await LoadSyncStatusAsync();
        }
    }

    [RelayCommand]
    private async Task SyncNowAsync()
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _outboxService.ProcessAsync();
            InfoMessage = "Sync finished.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            await LoadSyncStatusAsync();
        }
    }

    private ResultSubmissionRequest BuildRequest()
    {
        if (string.IsNullOrWhiteSpace(PollingStationCode))
        {
            throw new InvalidOperationException("Polling station code is required.");
        }

        return new ResultSubmissionRequest
        {
            PollingStationCode = PollingStationCode.Trim(),
            Constituency = Normalize(Constituency),
            County = Normalize(County),
            Region = Normalize(Region),
            CandidateA = ParseNumber(CandidateA, "Candidate A votes"),
            CandidateB = ParseNumber(CandidateB, "Candidate B votes"),
            CandidateC = ParseNumber(CandidateC, "Candidate C votes"),
            RejectedVotes = ParseNumber(RejectedVotes, "Rejected votes"),
            RegisteredVoters = ParseNumber(RegisteredVoters, "Registered voters"),
            Latitude = Latitude,
            Longitude = Longitude,
            DeviceFingerprint = ResolveDeviceFingerprint(),
            EvidenceFilePath = EvidenceFilePath
        };
    }

    private async Task QueueSubmissionAsync(ResultSubmissionRequest request)
    {
        await _outboxService.EnqueueAsync(new OutboxItem
        {
            Type = "result.submit",
            PayloadJson = JsonSerializer.Serialize(request),
            Status = "Queued"
        });

        InfoMessage = "No internet connection. Submission saved for sync.";
        ClearForm();
    }

    private async Task AcquireEvidenceAsync(Func<Task<FileResult?>> acquireFileAsync)
    {
        if (IsBusy)
        {
            return;
        }

        ErrorMessage = null;
        InfoMessage = null;

        try
        {
            var file = await acquireFileAsync();
            if (file == null)
            {
                return;
            }

            var evidenceFolder = Path.Combine(FileSystem.AppDataDirectory, "evidence");
            Directory.CreateDirectory(evidenceFolder);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"evidence-{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var destinationPath = Path.Combine(evidenceFolder, fileName);

            await using var sourceStream = await file.OpenReadAsync();
            await using var destinationStream = File.Create(destinationPath);
            await sourceStream.CopyToAsync(destinationStream);

            EvidenceFilePath = destinationPath;
            OnPropertyChanged(nameof(EvidenceSummary));
            InfoMessage = "Evidence attached.";
        }
        catch (Exception ex) when (ex is FeatureNotSupportedException or PermissionException)
        {
            ErrorMessage = "Photo permissions are required to attach evidence.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task LoadSyncStatusAsync()
    {
        var snapshot = await _outboxService.GetStatusAsync();
        PendingOutboxCount = snapshot.PendingCount + snapshot.ProcessingCount;
        DeadLetterCount = snapshot.DeadLetterCount;

        var lastSync = snapshot.LastProcessedAtUtc.HasValue
            ? $"Last sync {snapshot.LastProcessedAtUtc.Value.ToLocalTime():d MMM HH:mm}"
            : "No sync yet";

        SyncSummary = PendingOutboxCount == 0 && DeadLetterCount == 0
            ? $"All caught up. {lastSync}."
            : $"Pending {PendingOutboxCount}, dead-letter {DeadLetterCount}. {lastSync}.";
    }

    private void ClearForm()
    {
        PollingStationCode = string.Empty;
        Constituency = string.Empty;
        County = string.Empty;
        Region = string.Empty;
        CandidateA = string.Empty;
        CandidateB = string.Empty;
        CandidateC = string.Empty;
        RejectedVotes = string.Empty;
        RegisteredVoters = string.Empty;
        Latitude = null;
        Longitude = null;
        EvidenceFilePath = null;
        OnPropertyChanged(nameof(LocationSummary));
        OnPropertyChanged(nameof(EvidenceSummary));
    }

    private static int ParseNumber(string value, string fieldName)
    {
        if (!int.TryParse(value, out var parsed) || parsed < 0)
        {
            throw new InvalidOperationException($"{fieldName} must be a non-negative whole number.");
        }

        return parsed;
    }

    private static string? Normalize(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool HasInternet() => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

    private static string ResolveDeviceFingerprint()
        => $"{DeviceInfo.Manufacturer}:{DeviceInfo.Model}:{DeviceInfo.Name}:{DeviceInfo.VersionString}";

    private async void OnOutboxStatusChanged(object? sender, EventArgs e)
    {
        await MainThread.InvokeOnMainThreadAsync(LoadSyncStatusAsync);
    }
}
