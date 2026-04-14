namespace ProjectHX.Mobile.Services.Interfaces;

public interface IAppNavigator
{
    Task GoToLoginAsync();
    Task GoToMainAsync();
    Task GoToRegisterAsync();
    Task GoToForgotPasswordAsync();
    Task GoToResetPasswordAsync(string email);
    Task GoToOtpChallengeAsync(string email, string purpose);
    Task GoToNewsDetailAsync(string slug);
    Task GoToEventDetailAsync(int eventId);
    Task GoToIssueDetailAsync(int issueId);
    Task GoToAllNewsAsync();
    Task GoToAllEventsAsync();
    Task GoToAllIssuesAsync();
    Task GoToVolunteerHubAsync();
    Task GoToTasksAsync();
    Task GoToLoadingAsync();
    Task GoToWelcomeAsync();
}