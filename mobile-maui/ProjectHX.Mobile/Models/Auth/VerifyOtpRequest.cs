namespace ProjectHX.Mobile.Models.Auth;

public sealed class VerifyOtpRequest
{
    public string Email { get; set; } = string.Empty;
    public string Purpose { get; set; } = "Login";
    public string Code { get; set; } = string.Empty;
}
