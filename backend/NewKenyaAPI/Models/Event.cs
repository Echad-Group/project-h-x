using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public DateTime Date { get; set; }
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        [MaxLength(100)]
        public string? City { get; set; }
        
        [MaxLength(100)]
        public string? Region { get; set; }
        
        public string? ImageUrl { get; set; }
        
        [MaxLength(50)]
        public string Type { get; set; } = "rally"; // rally, townhall, meetup, etc.
        
        public int Capacity { get; set; } = 0; // 0 = unlimited
        
        public bool IsPublished { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
}
