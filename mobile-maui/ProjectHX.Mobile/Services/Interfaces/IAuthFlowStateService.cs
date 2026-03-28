namespace ProjectHX.Mobile.Services.Interfaces;

public interface IAuthFlowStateService
{
    void SetPendingLogin(string email, string password);
    (string Email, string Password)? GetPendingLogin();
    void ClearPendingLogin();
}
