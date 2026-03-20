namespace NewKenyaAPI.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CampaignRole { get; set; }
        public string? NationalIdNumber { get; set; }
        public string? VoterCardNumber { get; set; }
        public string? Region { get; set; }
        public string? County { get; set; }
        public string? SubCounty { get; set; }
        public string? Constituency { get; set; }
        public string? Ward { get; set; }
        public string? PollingStation { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string CampaignRole { get; set; } = UserRoles.User;
        public string VerificationStatus { get; set; } = CampaignVerificationStatuses.Pending;
        public string VoterCardStatus { get; set; } = CampaignVoterCardStatuses.Missing;
        public string? Region { get; set; }
        public string? County { get; set; }
        public bool OtpVerified { get; set; }
        public List<string> Roles { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }
}
