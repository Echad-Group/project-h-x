namespace ProjectHX.Mobile.Models.Profile;

public sealed class UpdateEmailRequest
{
    public string NewEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
