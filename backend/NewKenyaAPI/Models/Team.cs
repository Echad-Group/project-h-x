using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Team
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;
        
        [MaxLength(50)]
        public string? Icon { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public int DisplayOrder { get; set; } = 0;
        
        // Team Lead information (optional)
        public string? LeadUserId { get; set; }
        public ApplicationUser? LeadUser { get; set; }
        
        // Required skills for this team (comma-separated)
        [MaxLength(500)]
        public string? RequiredSkills { get; set; }
        
        // Preferred locations for this team (comma-separated regions)
        [MaxLength(500)]
        public string? PreferredLocations { get; set; }
        
        [MaxLength(200)]
        public string? TelegramLink { get; set; }
        
        [MaxLength(200)]
        public string? WhatsAppLink { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property
        public ICollection<VolunteerAssignment> VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
