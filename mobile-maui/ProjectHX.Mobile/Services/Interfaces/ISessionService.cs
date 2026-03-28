namespace ProjectHX.Mobile.Services.Interfaces;

public interface ISessionService
{
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    Task ClearAsync();
    Task<bool> HasActiveSessionAsync();
}
