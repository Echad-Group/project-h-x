using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models.DTOs
{
    public class UpdateProfileDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(200)]
        public string? Bio { get; set; }
        
        [MaxLength(100)]
        public string? Location { get; set; }
        
        [MaxLength(200)]
        public string? Website { get; set; }
        
        [MaxLength(50)]
        public string? Twitter { get; set; }
        
        [MaxLength(50)]
        public string? Facebook { get; set; }
        
        [MaxLength(50)]
        public string? LinkedIn { get; set; }
    }
    
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
        
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
    
    public class UpdateEmailDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
    
    public class UserProfileDto
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
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
