using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class EventRSVP
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string EventId { get; set; } = string.Empty;
        
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
        
        [Range(1, 10)]
        public int NumberOfGuests { get; set; } = 1;
        
        [MaxLength(500)]
        public string? SpecialRequirements { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
