using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models.DTOs
{
    public class CreateEventRSVPDto
    {
        [Required]
        public int EventId { get; set; }
        
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
    }
}
