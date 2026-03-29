using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class SessionService : ISessionService
{
    private const string TokenKey = "auth_token";

    private readonly ILogger<SessionService> _logger;

    public event EventHandler<SessionChangedEventArgs>? SessionChanged;

    public SessionService(ILogger<SessionService> logger)
    {
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync()
    {
        var token = await ReadStoredTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        if (!TryReadExpiry(token, out var expiresAtUtc))
        {
            _logger.LogWarning("Stored token could not be parsed and will be cleared.");
            await ClearInternalAsync(SessionChangeReason.InvalidToken, raiseIfAlreadyCleared: true);
            return null;
        }

        if (expiresAtUtc <= DateTimeOffset.UtcNow)
        {
            _logger.LogInformation("Stored session token expired at {ExpiryUtc}.", expiresAtUtc);
            await ClearInternalAsync(SessionChangeReason.Expired, raiseIfAlreadyCleared: true);
            return null;
        }

        return token;
    }

    public async Task SaveTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required.", nameof(token));
        }

        await SecureStorage.SetAsync(TokenKey, token);
        OnSessionChanged(new SessionChangedEventArgs(hasActiveSession: true, SessionChangeReason.SignedIn));
    }

    public Task ClearAsync(SessionChangeReason reason = SessionChangeReason.SignedOut)
    {
        return ClearInternalAsync(reason, raiseIfAlreadyCleared: false);
    }

    public async Task<bool> HasActiveSessionAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }

    private async Task<string?> ReadStoredTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read session token from secure storage.");
            return null;
        }
    }

    private Task ClearInternalAsync(SessionChangeReason reason, bool raiseIfAlreadyCleared)
    {
        var hadToken = SecureStorage.Remove(TokenKey);
        if (hadToken || raiseIfAlreadyCleared)
        {
            OnSessionChanged(new SessionChangedEventArgs(hasActiveSession: false, reason));
        }

        return Task.CompletedTask;
    }

    private void OnSessionChanged(SessionChangedEventArgs args)
    {
        SessionChanged?.Invoke(this, args);
    }

    private static bool TryReadExpiry(string token, out DateTimeOffset expiresAtUtc)
    {
        expiresAtUtc = default;

        var segments = token.Split('.');
        if (segments.Length < 2)
        {
            return false;
        }

        try
        {
            var payload = segments[1]
                .Replace('-', '+')
                .Replace('_', '/');

            payload = payload.PadRight(payload.Length + ((4 - payload.Length % 4) % 4), '=');
            var json = Convert.FromBase64String(payload);

            using var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("exp", out var expElement))
            {
                return false;
            }

            if (!expElement.TryGetInt64(out var expSeconds))
            {
                return false;
            }

            expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
