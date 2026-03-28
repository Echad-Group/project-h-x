using ProjectHX.Mobile.Services.Interfaces;

namespace ProjectHX.Mobile.Services;

public sealed class AuthFlowStateService : IAuthFlowStateService
{
    private (string Email, string Password)? _pendingLogin;

    public void SetPendingLogin(string email, string password)
    {
        _pendingLogin = (email, password);
    }

    public (string Email, string Password)? GetPendingLogin()
    {
        return _pendingLogin;
    }

    public void ClearPendingLogin()
    {
        _pendingLogin = null;
    }
}
