using ProjectHX.Mobile.Pages;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class AppNavigator : IAppNavigator
{
    public Task GoToLoginAsync() => NavigateAsync("//login");

    public Task GoToMainAsync() => NavigateAsync("//main");

    public Task GoToRegisterAsync() => NavigateAsync(nameof(RegisterPage));

    public Task GoToForgotPasswordAsync() => NavigateAsync(nameof(ForgotPasswordPage));

    public Task GoToResetPasswordAsync(string email)
        => NavigateAsync($"{nameof(ResetPasswordPage)}?email={Uri.EscapeDataString(email ?? string.Empty)}");

    public Task GoToOtpChallengeAsync(string email, string purpose)
        => NavigateAsync($"{nameof(OtpChallengePage)}?email={Uri.EscapeDataString(email ?? string.Empty)}&purpose={Uri.EscapeDataString(purpose ?? string.Empty)}");

    public Task GoToNewsDetailAsync(string slug)
        => NavigateAsync($"{nameof(NewsDetailPage)}?slug={Uri.EscapeDataString(slug)}");

    public Task GoToEventDetailAsync(int eventId)
        => NavigateAsync($"{nameof(EventDetailPage)}?id={eventId}");

    public Task GoToIssueDetailAsync(int issueId)
        => NavigateAsync($"{nameof(IssueDetailPage)}?id={issueId}");

    public Task GoToAllNewsAsync()
        => NavigateAsync(nameof(NewsListPage));

    public Task GoToAllEventsAsync()
        => NavigateAsync(nameof(EventsListPage));

    public Task GoToAllIssuesAsync()
        => NavigateAsync(nameof(IssuesListPage));

    public Task GoToVolunteerHubAsync()
        => NavigateAsync("//main/volunteer-tab/volunteer");

    public Task GoToTasksAsync()
        => NavigateAsync("//main/tasks-tab/tasks");

    private static Task NavigateAsync(string route)
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            if (Shell.Current is null)
            {
                throw new InvalidOperationException("Shell navigation is not available.");
            }

            await Shell.Current.GoToAsync(route);
        });
    }

    public Task GoToLoadingAsync() => NavigateAsync("//loading");

    public Task GoToWelcomeAsync() => NavigateAsync("//welcome");
}