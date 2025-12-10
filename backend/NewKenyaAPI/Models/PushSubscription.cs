using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class PushSubscription
    {
        public int Id { get; set; }
        
        [Required]
        public string Endpoint { get; set; } = string.Empty;
        
        public string? P256dh { get; set; }
        
        public string? Auth { get; set; }
        
        // Link to user if authenticated
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastUsed { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
