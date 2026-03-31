using Microsoft.Extensions.Configuration;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class ApiBaseUrlProvider : IApiBaseUrlProvider
{
    private const string ApiBaseUrlEnvVar = "PROJECTHX_API_BASE_URL";
    private readonly IConfiguration _configuration;

    public ApiBaseUrlProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Uri GetBaseUri()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(ApiBaseUrlEnvVar);
        if (Uri.TryCreate(fromEnvironment, UriKind.Absolute, out var envUri))
        {
            return EnsureApiBase(envUri);
        }

        var configuredBaseUrl = GetConfiguredBaseUrl();
        if (Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var configuredUri))
        {
            return EnsureApiBase(configuredUri);
        }

        throw new InvalidOperationException("No valid API base URL was found in appsettings.json or PROJECTHX_API_BASE_URL.");
    }

    private string? GetConfiguredBaseUrl()
    {
#if DEBUG
#if ANDROID
        return GetFirstConfiguredValue(
            "ApiSettings:DebugAndroidBaseUrl",
            "ApiSettings:DebugDefaultBaseUrl");
#elif IOS
        return GetFirstConfiguredValue(
            "ApiSettings:DebugIosBaseUrl",
            "ApiSettings:DebugDefaultBaseUrl");
#else
        return _configuration["ApiSettings:DebugDefaultBaseUrl"];
#endif
#else
        return _configuration["ApiSettings:ProductionBaseUrl"];
#endif
    }

    private string? GetFirstConfiguredValue(params string[] keys)
    {
        foreach (var key in keys)
        {
            var value = _configuration[key];
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static Uri EnsureApiBase(Uri uri)
    {
        var baseValue = uri.ToString().TrimEnd('/');
        if (!baseValue.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            baseValue += "/api";
        }

        return new Uri(baseValue + "/");
    }
}
