using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class VolunteerAssignment
    {
        public int Id { get; set; }
        
        [Required]
        public int VolunteerId { get; set; }
        public Volunteer Volunteer { get; set; } = null!;
        
        [Required]
        public int UnitId { get; set; }
        public Unit Unit { get; set; } = null!;
        
        public int? TeamId { get; set; } // Optional - volunteer might be assigned to unit without specific team
        public Team? Team { get; set; }
        
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(500)]
        public string? Notes { get; set; } // Admin notes about assignment
        
        // Who assigned this volunteer
        public string? AssignedByUserId { get; set; }
        public ApplicationUser? AssignedByUser { get; set; }
    }
}
