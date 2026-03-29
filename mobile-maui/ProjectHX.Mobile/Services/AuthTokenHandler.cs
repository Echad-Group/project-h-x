using System.Net;
using System.Net.Http.Headers;
using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class AuthTokenHandler : DelegatingHandler
{
    private readonly ISessionService _sessionService;

    public AuthTokenHandler(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _sessionService.GetTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _sessionService.ClearAsync(SessionChangeReason.Unauthorized);
        }

        return response;
    }
}
