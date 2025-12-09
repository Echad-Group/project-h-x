using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Volunteer
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        // Location Information
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(100)]
        public string? Region { get; set; }
        
        [MaxLength(500)]
        public string? AvailabilityZones { get; set; } // Comma-separated zones
        
        // Skills Offered (comma-separated)
        [MaxLength(500)]
        public string? Skills { get; set; }
        
        // Availability
        public int? HoursPerWeek { get; set; }
        public bool AvailableWeekends { get; set; } = false;
        public bool AvailableEvenings { get; set; } = false;
        
        [MaxLength(500)]
        public string? Interests { get; set; }
        
        // Link to the authenticated user (optional - for logged-in users)
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
