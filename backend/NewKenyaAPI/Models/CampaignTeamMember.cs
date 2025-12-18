using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class CampaignTeamMember
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Bio { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Responsibilities { get; set; }
        
        [MaxLength(500)]
        public string? PhotoUrl { get; set; }
        
        // Social Media Links
        [MaxLength(200)]
        public string? TwitterHandle { get; set; }
        
        [MaxLength(200)]
        public string? LinkedInUrl { get; set; }
        
        [MaxLength(200)]
        public string? FacebookUrl { get; set; }
        
        public int DisplayOrder { get; set; } = 0;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
