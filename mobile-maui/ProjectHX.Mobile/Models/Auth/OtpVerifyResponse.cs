namespace ProjectHX.Mobile.Models.Auth;

public sealed class OtpVerifyResponse
{
    public string? Message { get; set; }
    public string? Level { get; set; }
    public DateTime? VerifiedAt { get; set; }
}
