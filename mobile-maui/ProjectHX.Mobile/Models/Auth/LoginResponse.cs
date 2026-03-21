namespace ProjectHX.Mobile.Models.Auth;

public sealed class LoginResponse
{
    public string? Token { get; set; }
    public string? Email { get; set; }
    public bool OtpRequired { get; set; }
    public string? Message { get; set; }
}
