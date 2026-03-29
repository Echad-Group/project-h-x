using System.Net.Http;
using ProjectHX.Mobile.Models.Auth;
using ProjectHX.Mobile.Models.Inbox;
using ProjectHX.Mobile.Models.Profile;
using ProjectHX.Mobile.Models.Tasks;
using ProjectHX.Mobile.Models.Volunteer;
using ProjectHX.Mobile.Tests.TestDoubles;
using ProjectHX.Mobile.ViewModels;
using Xunit;

namespace ProjectHX.Mobile.Tests.ViewModels;

public class ViewModelBaselineTests
{
    [Fact]
    public async Task LoginViewModel_SetsErrorMessage_OnAuthFailure()
    {
        var auth = new FakeAuthApiService
        {
            LoginAsyncHandler = _ => throw new InvalidOperationException("invalid credentials")
        };
        var navigator = new FakeAppNavigator();

        var vm = new LoginViewModel(auth, navigator, new FakeSessionService(), new FakeAuthFlowStateService())
        {
            Email = "member@example.com",
            Password = "bad-password"
        };

        await vm.SignInCommand.ExecuteAsync(null);

        Assert.False(vm.IsBusy);
        Assert.Equal("invalid credentials", vm.ErrorMessage);
        Assert.Null(vm.InfoMessage);
    }

    [Fact]
    public async Task ProfileViewModel_LoadAsync_MapsProfileAndVolunteerAndPush()
    {
        var profileService = new FakeUserProfileApiService
        {
            Profile = new UserProfileModel
            {
                Email = "member@example.com",
                FirstName = "Ada",
                LastName = "Lovelace",
                PhoneNumber = "255700111222",
                Location = "Dar es Salaam",
                NationalIdNumber = "NIDA-123",
                VoterCardNumber = "VOTER-777",
                Roles = ["Volunteer"]
            }
        };

        var volunteerService = new FakeVolunteerApiService
        {
            Status = new VolunteerStatusResponse
            {
                IsVolunteer = true,
                Volunteer = new VolunteerProfileModel
                {
                    Name = "Ada Lovelace",
                    Email = "member@example.com",
                    CreatedAt = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc)
                }
            }
        };

        var pushService = new FakePushApiService
        {
            Status = new() { IsSubscribed = true, SubscriptionDate = new DateTime(2026, 3, 20, 8, 0, 0, DateTimeKind.Utc) }
        };
        var navigator = new FakeAppNavigator();

        var vm = new ProfileViewModel(
            profileService,
            volunteerService,
            new FakeAuthApiService(),
            navigator,
            new FakeSessionService(),
            new FakeApiBaseUrlProvider(),
            pushService);

        await vm.LoadAsync();

        Assert.Equal("member@example.com", vm.Details.Email);
        Assert.Equal("Ada", vm.Details.FirstName);
        Assert.Equal("Volunteer", vm.Identity.RolesSummary);
        Assert.True(vm.Volunteer.IsVolunteer);
        Assert.True(vm.Account.IsPushSubscribed);
        Assert.Contains("Subscribed since", vm.Account.PushStatusSummary);
        Assert.False(vm.IsBusy);
    }

    [Fact]
    public async Task TasksViewModel_QueuesCompletion_WhenOfflineErrorOccurs()
    {
        var tasksApi = new FakeTasksApiService
        {
            Tasks =
            [
                new TaskModel { Id = 42, Title = "Collect forms", Status = "InProgress" }
            ],
            CompleteTaskAsyncHandler = (_, _) => throw new HttpRequestException("network unavailable")
        };

        var outbox = new FakeOutboxService();
        var vm = new TasksViewModel(tasksApi, outbox);

        await vm.LoadAsync();
        vm.SelectTaskCommand.Execute(vm.Tasks.Single());
        vm.CompletionNotes = "Completed at station";

        await vm.CompleteTaskCommand.ExecuteAsync(null);

        Assert.Single(outbox.Enqueued);
        Assert.Equal("task.complete", outbox.Enqueued[0].Type);
        Assert.Contains("queued for sync", vm.InfoMessage ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task InboxViewModel_LoadAsync_ComputesUnreadCount()
    {
        var inboxApi = new FakeInboxApiService
        {
            Messages =
            [
                new InboxMessage { Id = 1, Channel = "email", Body = "Welcome", ReadAt = null },
                new InboxMessage { Id = 2, Channel = "push", Body = "Reminder", ReadAt = DateTime.UtcNow }
            ]
        };

        var vm = new InboxViewModel(inboxApi);
        await vm.LoadAsync();

        Assert.Equal(2, vm.Messages.Count);
        Assert.Equal(1, vm.UnreadCount);
        Assert.True(vm.HasUnreadMessages);
    }

    [Fact]
    public async Task SubmitResultViewModel_LoadAsync_ReflectsOutboxSnapshot()
    {
        var outbox = new FakeOutboxService
        {
            Snapshot = new()
            {
                PendingCount = 2,
                ProcessingCount = 1,
                DeadLetterCount = 1,
                LastProcessedAtUtc = new DateTime(2026, 3, 28, 12, 0, 0, DateTimeKind.Utc)
            }
        };

        var vm = new SubmitResultViewModel(new FakeResultsApiService(), outbox);
        await vm.LoadAsync();

        Assert.Equal(3, vm.PendingOutboxCount);
        Assert.Equal(1, vm.DeadLetterCount);
        Assert.Contains("Pending 3, dead-letter 1", vm.SyncSummary);
    }

    [Fact]
    public async Task SubmitResultViewModel_SubmitAsync_RejectsImpossibleVoteTotals()
    {
        var vm = new SubmitResultViewModel(new FakeResultsApiService(), new FakeOutboxService())
        {
            PollingStationCode = "PS-77",
            CandidateA = "70",
            CandidateB = "40",
            CandidateC = "10",
            RejectedVotes = "5",
            RegisteredVoters = "100"
        };

        await vm.SubmitCommand.ExecuteAsync(null);

        Assert.Equal("Total ballots counted cannot exceed registered voters.", vm.ErrorMessage);
    }
}
