using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewKenyaAPI.Models
{
    public class Donation
    {
        public int Id { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string DonorName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string DonorEmail { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
        
        [MaxLength(100)]
        public string? TransactionId { get; set; }
        
        [MaxLength(500)]
        public string? Message { get; set; }
        
        public bool IsAnonymous { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
