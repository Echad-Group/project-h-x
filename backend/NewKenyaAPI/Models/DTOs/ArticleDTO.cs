namespace NewKenyaAPI.Models.DTOs
{
    public class ArticleDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public int Views { get; set; }
        public bool IsFeatured { get; set; }
        public int ReadTimeMinutes { get; set; }
        public string? AuthorUserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ArticleListItemDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string Author { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public int Views { get; set; }
        public bool IsFeatured { get; set; }
        public int ReadTimeMinutes { get; set; }
    }

    public class CreateArticleDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public string Author { get; set; } = string.Empty;
        public DateTime? PublishedDate { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public string Status { get; set; } = "draft";
        public bool IsFeatured { get; set; } = false;
        public int ReadTimeMinutes { get; set; } = 5;
    }

    public class UpdateArticleDTO
    {
        public string? Title { get; set; }
        public string? Excerpt { get; set; }
        public string? Content { get; set; }
        public string? Category { get; set; }
        public List<string>? Tags { get; set; }
        public string? Author { get; set; }
        public DateTime? PublishedDate { get; set; }
        public string? FeaturedImageUrl { get; set; }
        public List<string>? ImageUrls { get; set; }
        public string? Status { get; set; }
        public bool? IsFeatured { get; set; }
        public int? ReadTimeMinutes { get; set; }
    }
}
