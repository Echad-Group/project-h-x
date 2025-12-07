using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Contact
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? Subject { get; set; }
        
        [Required]
        [MaxLength(2000)]
        public string Message { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
