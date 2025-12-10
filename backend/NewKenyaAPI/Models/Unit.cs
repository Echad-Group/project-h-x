using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Unit
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Icon { get; set; } // Emoji or icon identifier
        
        [MaxLength(50)]
        public string? Color { get; set; } // Theme color for UI
        
        public bool IsActive { get; set; } = true;
        
        public int DisplayOrder { get; set; } = 0;
        
        // Manager/Lead information (optional)
        public string? LeadUserId { get; set; }
        public ApplicationUser? LeadUser { get; set; }
        
        [MaxLength(200)]
        public string? TelegramLink { get; set; }
        
        [MaxLength(200)]
        public string? WhatsAppLink { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation property
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<VolunteerAssignment> VolunteerAssignments { get; set; } = new List<VolunteerAssignment>();
    }
}
