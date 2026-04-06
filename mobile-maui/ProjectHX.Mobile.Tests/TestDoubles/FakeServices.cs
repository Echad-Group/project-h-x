using ProjectHX.Mobile.Infrastructure.Outbox;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Models.Inbox;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.PublicContent;
using ProjectHX.Mobile.Models.Results;
using ProjectHX.Mobile.Models.Tasks;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Tests.TestDoubles;

internal sealed class FakeAuthApiService : IAuthApiService
{
    public Func<LoginRequest, Task<LoginResponse>> LoginAsyncHandler { get; set; } = _ => Task.FromResult(new LoginResponse());

    public Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default) => LoginAsyncHandler(request);
    public Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default) => Task.FromResult(new LoginResponse());
    public Task<OtpVerifyResponse> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default) => Task.FromResult(new OtpVerifyResponse());
    public Task<string> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task LogoutAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

internal sealed class FakeSessionService : ISessionService
{
    public string? Token { get; private set; }

    public event EventHandler<SessionChangedEventArgs>? SessionChanged;

    public Task<string?> GetTokenAsync() => Task.FromResult(Token);
    public Task SaveTokenAsync(string token)
    {
        Token = token;
        SessionChanged?.Invoke(this, new SessionChangedEventArgs(hasActiveSession: true, SessionChangeReason.SignedIn));
        return Task.CompletedTask;
    }

    public Task ClearAsync(SessionChangeReason reason = SessionChangeReason.SignedOut)
    {
        Token = null;
        SessionChanged?.Invoke(this, new SessionChangedEventArgs(hasActiveSession: false, reason));
        return Task.CompletedTask;
    }

    public Task<bool> HasActiveSessionAsync() => Task.FromResult(!string.IsNullOrWhiteSpace(Token));
}

internal sealed class FakeAppNavigator : IAppNavigator
{
    public string? LastNavigation { get; private set; }

    public Task GoToLoginAsync()
    {
        LastNavigation = "//login";
        return Task.CompletedTask;
    }

    public Task GoToMainAsync()
    {
        LastNavigation = "//main";
        return Task.CompletedTask;
    }

    public Task GoToRegisterAsync()
    {
        LastNavigation = "RegisterPage";
        return Task.CompletedTask;
    }

    public Task GoToForgotPasswordAsync()
    {
        LastNavigation = "ForgotPasswordPage";
        return Task.CompletedTask;
    }

    public Task GoToResetPasswordAsync(string email)
    {
        LastNavigation = $"ResetPasswordPage?email={email}";
        return Task.CompletedTask;
    }

    public Task GoToOtpChallengeAsync(string email, string purpose)
    {
        LastNavigation = $"OtpChallengePage?email={email}&purpose={purpose}";
        return Task.CompletedTask;
    }

    public Task GoToNewsDetailAsync(string slug)
    {
        LastNavigation = $"NewsDetailPage?slug={slug}";
        return Task.CompletedTask;
    }

    public Task GoToEventDetailAsync(int eventId)
    {
        LastNavigation = $"EventDetailPage?id={eventId}";
        return Task.CompletedTask;
    }

    public Task GoToIssueDetailAsync(int issueId)
    {
        LastNavigation = $"IssueDetailPage?id={issueId}";
        return Task.CompletedTask;
    }

    public Task GoToVolunteerHubAsync()
    {
        LastNavigation = "//main/volunteer-tab/volunteer";
        return Task.CompletedTask;
    }

    public Task GoToTasksAsync()
    {
        LastNavigation = "//main/tasks-tab/tasks";
        return Task.CompletedTask;
    }

    public Task GoToLoadingAsync()
    {
        LastNavigation = "//loading";
        return Task.CompletedTask;
    }

    public Task GoToWelcomeAsync()
    {
        LastNavigation = "//welcome";
        return Task.CompletedTask;
    }
}

internal sealed class FakeAuthFlowStateService : IAuthFlowStateService
{
    public (string Email, string Password)? Pending { get; private set; }

    public void SetPendingLogin(string email, string password) => Pending = (email, password);
    public (string Email, string Password)? GetPendingLogin() => Pending;
    public void ClearPendingLogin() => Pending = null;
}

internal sealed class FakeTasksApiService : ITasksApiService
{
    public List<TaskModel> Tasks { get; set; } = [];
    public Func<int, string?, Task<string>> CompleteTaskAsyncHandler { get; set; } = (_, _) => Task.FromResult("Completed");

    public Task<List<TaskModel>> GetMyTasksAsync(CancellationToken cancellationToken = default) => Task.FromResult(Tasks);
    public Task<string> StartTaskAsync(int taskId, CancellationToken cancellationToken = default) => Task.FromResult("Started");
    public Task<string> CompleteTaskAsync(int taskId, string? notes, CancellationToken cancellationToken = default) => CompleteTaskAsyncHandler(taskId, notes);
}

internal sealed class FakeInboxApiService : IInboxApiService
{
    public List<InboxMessage> Messages { get; set; } = [];

    public Task<List<InboxMessage>> GetInboxAsync(CancellationToken cancellationToken = default) => Task.FromResult(Messages);
    public Task<string> MarkReadAsync(int messageId, CancellationToken cancellationToken = default) => Task.FromResult("ok");
}

internal sealed class FakeOutboxService : ISyncOutboxService
{
    public event EventHandler? StatusChanged;

    public List<OutboxItem> Enqueued { get; } = [];
    public OutboxStatusSnapshot Snapshot { get; set; } = new();

    public Task InitializeAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task EnqueueAsync(OutboxItem item, CancellationToken cancellationToken = default)
    {
        Enqueued.Add(item);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<OutboxItem>> GetPendingAsync(CancellationToken cancellationToken = default)
        => Task.FromResult((IReadOnlyList<OutboxItem>)Enqueued);

    public Task<OutboxStatusSnapshot> GetStatusAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(Snapshot);

    public Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        StatusChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }
}

internal sealed class FakeResultsApiService : IResultsApiService
{
    public Func<ResultSubmissionRequest, Task<string>> SubmitAsyncHandler { get; set; } = _ => Task.FromResult("submitted");
    public Task<string> SubmitAsync(ResultSubmissionRequest request, CancellationToken cancellationToken = default) => SubmitAsyncHandler(request);
}

internal sealed class FakeUserProfileApiService : IUserProfileApiService
{
    public UserProfileModel Profile { get; set; } = new();

    public Task<UserProfileModel> GetProfileAsync(CancellationToken cancellationToken = default) => Task.FromResult(Profile);
    public Task UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task<string> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> UpdateEmailAsync(UpdateEmailRequest request, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> DeleteAccountAsync(string password, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> UploadProfilePhotoAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> DeleteProfilePhotoAsync(CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> UploadVerificationDocumentAsync(Stream stream, string fileName, string contentType, string documentType, CancellationToken cancellationToken = default) => Task.FromResult("ok");
}

internal sealed class FakeVolunteerApiService : IVolunteerApiService
{
    public VolunteerStatusResponse Status { get; set; } = new();
    public int CreateCalls { get; private set; }
    public int UpdateCalls { get; private set; }

    public Task<VolunteerStatusResponse> GetMyVolunteerStatusAsync(CancellationToken cancellationToken = default) => Task.FromResult(Status);

    public Task<string> CreateVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        CreateCalls++;
        Status = new VolunteerStatusResponse
        {
            IsVolunteer = true,
            Volunteer = new VolunteerProfileModel
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                City = request.City,
                Region = request.Region,
                AvailabilityZones = request.AvailabilityZones,
                Skills = request.Skills,
                HoursPerWeek = request.HoursPerWeek,
                AvailableWeekends = request.AvailableWeekends,
                AvailableEvenings = request.AvailableEvenings,
                Interests = request.Interests,
                CreatedAt = DateTime.UtcNow
            }
        };

        return Task.FromResult("Volunteer sign-up completed.");
    }

    public Task<string> UpdateMyVolunteerProfileAsync(UpdateVolunteerRequest request, CancellationToken cancellationToken = default)
    {
        UpdateCalls++;
        return Task.FromResult("Volunteer profile updated successfully.");
    }

    public Task<string> LeaveVolunteerRoleAsync(CancellationToken cancellationToken = default) => Task.FromResult("ok");
}

internal sealed class FakePushApiService : IPushApiService
{
    public PushStatusModel Status { get; set; } = new();

    public Task<string> SubscribeAsync(PushSubscriptionRequestModel request, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<string> UnsubscribeAsync(string endpoint, CancellationToken cancellationToken = default) => Task.FromResult("ok");
    public Task<PushStatusModel> GetStatusAsync(CancellationToken cancellationToken = default) => Task.FromResult(Status);
}

internal sealed class FakeApiBaseUrlProvider : IApiBaseUrlProvider
{
    public Uri BaseUri { get; set; } = new("https://example.test/api/");
    public Uri GetBaseUri() => BaseUri;
}
