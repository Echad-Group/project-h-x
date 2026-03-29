namespace ProjectHX.Mobile.Services.Interfaces;

public interface ISessionService
{
    event EventHandler<SessionChangedEventArgs>? SessionChanged;

    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    Task ClearAsync(SessionChangeReason reason = SessionChangeReason.SignedOut);
    Task<bool> HasActiveSessionAsync();
}
