using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewKenyaAPI.Models
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(600)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Excerpt { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty; // Comma-separated

        [Required]
        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        [Required]
        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }

        [MaxLength(500)]
        public string? FeaturedImageUrl { get; set; }

        public string? ImageUrls { get; set; } // JSON array of image URLs

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "draft"; // draft, published, archived

        public int Views { get; set; } = 0;

        public bool IsFeatured { get; set; } = false;

        public int ReadTimeMinutes { get; set; } = 5;

        [MaxLength(450)]
        public string? AuthorUserId { get; set; }

        [ForeignKey("AuthorUserId")]
        public ApplicationUser? AuthorUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
