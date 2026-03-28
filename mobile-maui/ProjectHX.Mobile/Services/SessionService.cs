using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class SessionService : ISessionService
{
    private const string TokenKey = "auth_token";

    public Task<string?> GetTokenAsync()
    {
        return SecureStorage.GetAsync(TokenKey);
    }

    public Task SaveTokenAsync(string token)
    {
        return SecureStorage.SetAsync(TokenKey, token);
    }

    public Task ClearAsync()
    {
        SecureStorage.Remove(TokenKey);
        return Task.CompletedTask;
    }

    public async Task<bool> HasActiveSessionAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }
}
