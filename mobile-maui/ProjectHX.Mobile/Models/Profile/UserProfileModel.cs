namespace ProjectHX.Mobile.Models.Profile;

public sealed class UserProfileModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
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
    public string? IdImageUrl { get; set; }
    public string? SelfieImageUrl { get; set; }
    public string? VoterCardImageUrl { get; set; }
    public string VerificationStatus { get; set; } = string.Empty;
    public string VoterCardStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
}
