namespace ProjectHX.Mobile.Models.PublicContent;

public sealed class NewsArticleListItemModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public int Views { get; set; }
    public bool IsFeatured { get; set; }
    public int ReadTimeMinutes { get; set; }

    public string PublishedDisplay => PublishedDate.ToLocalTime().ToString("d MMM yyyy");
    public string? PrimaryImageUrl => ImageUrls.FirstOrDefault(url => !string.IsNullOrWhiteSpace(url)) ?? FeaturedImageUrl;
    public bool HasPrimaryImage => !string.IsNullOrWhiteSpace(PrimaryImageUrl);
}
