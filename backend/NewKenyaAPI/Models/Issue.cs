using System.ComponentModel.DataAnnotations;

namespace NewKenyaAPI.Models
{
    public class Issue
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;
        
        [Required]
        public string Summary { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? Icon { get; set; }
        
        [MaxLength(50)]
        public string? Color { get; set; } // For UI theming
        
        public int DisplayOrder { get; set; }
        
        public bool IsPublished { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public ICollection<IssueInitiative> Initiatives { get; set; } = new List<IssueInitiative>();
        public ICollection<IssueQuestion> Questions { get; set; } = new List<IssueQuestion>();
    }
    
    public class IssueInitiative
    {
        public int Id { get; set; }
        
        public int IssueId { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public int DisplayOrder { get; set; }
        
        // Navigation property
        public Issue Issue { get; set; } = null!;
    }
    
    public class IssueQuestion
    {
        public int Id { get; set; }
        
        public int IssueId { get; set; }
        
        [Required]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        public string Answer { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        // Navigation property
        public Issue Issue { get; set; } = null!;
    }
}
