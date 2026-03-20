using Microsoft.AspNetCore.Identity;

namespace NewKenyaAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? Twitter { get; set; }
        public string? Facebook { get; set; }
        public string? LinkedIn { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public string CampaignRole { get; set; } = UserRoles.User;
        public string VerificationStatus { get; set; } = CampaignVerificationStatuses.Pending;
        public string VoterCardStatus { get; set; } = CampaignVoterCardStatuses.Missing;
        public string? NationalIdNumber { get; set; }
        public string? VoterCardNumber { get; set; }
        public string? IdImageUrl { get; set; }
        public string? SelfieImageUrl { get; set; }
        public string? VoterCardImageUrl { get; set; }
        public decimal? FaceMatchScore { get; set; }
        public bool IsOtpVerified { get; set; }
        public DateTime? OtpVerifiedAt { get; set; }
        public string? Region { get; set; }
        public string? County { get; set; }
        public string? SubCounty { get; set; }
        public string? Constituency { get; set; }
        public string? Ward { get; set; }
        public string? PollingStation { get; set; }
        public string? ParentUserId { get; set; }
        public ApplicationUser? ParentUser { get; set; }
        public ICollection<ApplicationUser> DirectDownlines { get; set; } = new List<ApplicationUser>();
        public int DownlineCount { get; set; }
        public DateTime? VerificationReviewedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}
