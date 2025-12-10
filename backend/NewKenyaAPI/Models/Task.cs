using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Task
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;
        
        public int? TeamId { get; set; } // Optional - task might be for entire unit
        public Team? Team { get; set; }
        
        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }
        
        public DateTime? DueDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; } = "Open"; // Open, In Progress, Completed, Cancelled
        
        [MaxLength(50)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Urgent
        
        // Location specific task
        [MaxLength(100)]
        public string? Location { get; set; }
        
        [MaxLength(100)]
        public string? Region { get; set; }
        
        // Required skills (comma-separated)
        [MaxLength(500)]
        public string? RequiredSkills { get; set; }
        
        // Number of volunteers needed
        public int? VolunteersNeeded { get; set; }
        
        // Assignment to specific volunteer
        public int? AssignedToVolunteerId { get; set; }
        public Volunteer? AssignedToVolunteer { get; set; }
        
        // Who created this task
        public string? CreatedByUserId { get; set; }
        public ApplicationUser? CreatedByUser { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        [MaxLength(1000)]
        public string? CompletionNotes { get; set; }
    }
}
