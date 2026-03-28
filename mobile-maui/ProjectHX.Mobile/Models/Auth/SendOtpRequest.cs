namespace ProjectHX.Mobile.Models.Auth;

public sealed class SendOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Purpose { get; set; } = "Login";
}
