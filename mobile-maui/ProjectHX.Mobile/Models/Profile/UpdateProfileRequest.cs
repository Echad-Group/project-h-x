namespace ProjectHX.Mobile.Models.Profile;

public sealed class UpdateProfileRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public string? Twitter { get; set; }
    public string? Facebook { get; set; }
    public string? LinkedIn { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? NationalIdNumber { get; set; }
    public string? VoterCardNumber { get; set; }
}
