using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NewKenyaAPI.Data;
using NewKenyaAPI.Models;
using NewKenyaAPI.Models.DTOs;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;

namespace NewKenyaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NewsController> _logger;
        private readonly IMemoryCache _memoryCache;

        public NewsController(ApplicationDbContext context, ILogger<NewsController> logger, IMemoryCache memoryCache)
        {
            _context = context;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        // GET: api/news
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ArticleListItemDTO>>> GetArticles(
            [FromQuery] string? category = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = "published",
            [FromQuery] string? sortBy = "newest",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12)
        {
            try
            {
                var query = _context.Articles.AsQueryable();

                // Filter by status
                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(a => a.Status == status);
                }

                // Filter by category
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    query = query.Where(a => a.Category.ToLower() == category.ToLower());
                }

                // Search in title, excerpt, and content
                if (!string.IsNullOrEmpty(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(a =>
                        a.Title.ToLower().Contains(searchLower) ||
                        a.Excerpt.ToLower().Contains(searchLower) ||
                        a.Content.ToLower().Contains(searchLower) ||
                        a.Tags.ToLower().Contains(searchLower)
                    );
                }

                // Sort
                query = sortBy?.ToLower() switch
                {
                    "oldest" => query.OrderBy(a => a.PublishedDate),
                    "popular" => query.OrderByDescending(a => a.Views),
                    _ => query.OrderByDescending(a => a.PublishedDate)
                };

                // Get total count before pagination
                var totalCount = await query.CountAsync();

                // Pagination
                var articles = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                // Map to DTOs after query execution
                var articleDtos = articles.Select(a => new ArticleListItemDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Excerpt = a.Excerpt,
                    Category = a.Category,
                    Tags = string.IsNullOrEmpty(a.Tags) ? new List<string>() : a.Tags.Split(',').ToList(),
                    Author = a.Author,
                    PublishedDate = a.PublishedDate,
                    FeaturedImageUrl = a.FeaturedImageUrl,
                    ImageUrls = string.IsNullOrEmpty(a.ImageUrls) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(a.ImageUrls) ?? new List<string>(),
                    Views = a.Views,
                    IsFeatured = a.IsFeatured,
                    ReadTimeMinutes = a.ReadTimeMinutes
                }).ToList();

                Response.Headers["X-Total-Count"] = totalCount.ToString();
                Response.Headers["X-Page"] = page.ToString();
                Response.Headers["X-Page-Size"] = pageSize.ToString();

                return Ok(articleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching articles");
                return StatusCode(500, "An error occurred while fetching articles");
            }
        }

        // GET: api/news/featured
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ArticleListItemDTO>>> GetFeaturedArticles(
            [FromQuery] int limit = 4)
        {
            try
            {
                var articles = await _context.Articles
                    .Where(a => a.Status == "published" && a.IsFeatured)
                    .OrderByDescending(a => a.PublishedDate)
                    .Take(limit)
                    .ToListAsync();

                // Map to DTOs after query execution
                var articleDtos = articles.Select(a => new ArticleListItemDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Excerpt = a.Excerpt,
                    Category = a.Category,
                    Tags = string.IsNullOrEmpty(a.Tags) ? new List<string>() : a.Tags.Split(',').ToList(),
                    Author = a.Author,
                    PublishedDate = a.PublishedDate,
                    FeaturedImageUrl = a.FeaturedImageUrl,
                    ImageUrls = string.IsNullOrEmpty(a.ImageUrls) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(a.ImageUrls) ?? new List<string>(),
                    Views = a.Views,
                    IsFeatured = a.IsFeatured,
                    ReadTimeMinutes = a.ReadTimeMinutes
                }).ToList();

                return Ok(articleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching featured articles");
                return StatusCode(500, "An error occurred while fetching featured articles");
            }
        }

        // GET: api/news/{slug}
        [HttpGet("{slug}")]
        public async Task<ActionResult<ArticleDTO>> GetArticle(string slug)
        {
            try
            {
                var article = await _context.Articles
                    .FirstOrDefaultAsync(a => a.Slug == slug);

                if (article == null)
                {
                    return NotFound();
                }

                var articleDto = new ArticleDTO
                {
                    Id = article.Id,
                    Title = article.Title,
                    Slug = article.Slug,
                    Excerpt = article.Excerpt,
                    Content = article.Content,
                    Category = article.Category,
                    Tags = string.IsNullOrEmpty(article.Tags) ? new List<string>() : article.Tags.Split(',').ToList(),
                    Author = article.Author,
                    PublishedDate = article.PublishedDate,
                    UpdatedDate = article.UpdatedDate,
                    FeaturedImageUrl = article.FeaturedImageUrl,
                    ImageUrls = string.IsNullOrEmpty(article.ImageUrls) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(article.ImageUrls) ?? new List<string>(),
                    Status = article.Status,
                    Views = article.Views,
                    IsFeatured = article.IsFeatured,
                    ReadTimeMinutes = article.ReadTimeMinutes,
                    AuthorUserId = article.AuthorUserId,
                    CreatedAt = article.CreatedAt
                };

                return Ok(articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching article with slug {Slug}", slug);
                return StatusCode(500, "An error occurred while fetching the article");
            }
        }

        // POST: api/news/{id}/increment-view
        [HttpPost("{id}/increment-view")]
        public async Task<IActionResult> IncrementView(Guid id)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                {
                    return NotFound();
                }

                // Get client IP address for rate limiting
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                var cacheKey = $"article_view_{id}_{clientIp}";

                // Bot detection - basic check for common bot user agents
                var botKeywords = new[] { "bot", "crawler", "spider", "scraper" };
                if (botKeywords.Any(keyword => userAgent.ToLower().Contains(keyword)))
                {
                    _logger.LogInformation("Bot detected, skipping view increment for article {Id}", id);
                    return Ok(new { views = article.Views, incremented = false, reason = "bot_detected" });
                }

                // Check if this IP has viewed this article recently (1 hour cooldown)
                if (_memoryCache.TryGetValue(cacheKey, out _))
                {
                    _logger.LogInformation("Duplicate view blocked for article {Id} from IP {Ip}", id, clientIp);
                    return Ok(new { views = article.Views, incremented = false, reason = "rate_limited" });
                }

                // Increment view count
                article.Views++;
                await _context.SaveChangesAsync();

                // Cache this view for 1 hour to prevent rapid duplicates from same IP
                _memoryCache.Set(cacheKey, true, TimeSpan.FromHours(1));

                _logger.LogInformation("View count incremented for article {Id} from IP {Ip}", id, clientIp);
                return Ok(new { views = article.Views, incremented = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error incrementing view count for article {Id}", id);
                return StatusCode(500, "An error occurred while updating view count");
            }
        }

        // POST: api/news
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ArticleDTO>> CreateArticle([FromBody] CreateArticleDTO createDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Generate slug from title
                var slug = GenerateSlug(createDto.Title);
                
                // Ensure slug is unique
                var existingSlug = await _context.Articles.AnyAsync(a => a.Slug == slug);
                if (existingSlug)
                {
                    slug = $"{slug}-{Guid.NewGuid().ToString().Substring(0, 8)}";
                }

                var article = new Article
                {
                    Title = createDto.Title,
                    Slug = slug,
                    Excerpt = createDto.Excerpt,
                    Content = createDto.Content,
                    Category = createDto.Category,
                    Tags = string.Join(",", createDto.Tags),
                    Author = createDto.Author,
                    PublishedDate = createDto.PublishedDate ?? DateTime.UtcNow,
                    FeaturedImageUrl = createDto.FeaturedImageUrl,
                    ImageUrls = createDto.ImageUrls.Count > 0 ? JsonSerializer.Serialize(createDto.ImageUrls) : null,
                    Status = createDto.Status,
                    IsFeatured = createDto.IsFeatured,
                    ReadTimeMinutes = createDto.ReadTimeMinutes,
                    AuthorUserId = userId
                };

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                var articleDto = new ArticleDTO
                {
                    Id = article.Id,
                    Title = article.Title,
                    Slug = article.Slug,
                    Excerpt = article.Excerpt,
                    Content = article.Content,
                    Category = article.Category,
                    Tags = createDto.Tags,
                    Author = article.Author,
                    PublishedDate = article.PublishedDate,
                    UpdatedDate = article.UpdatedDate,
                    FeaturedImageUrl = article.FeaturedImageUrl,
                    ImageUrls = createDto.ImageUrls,
                    Status = article.Status,
                    Views = article.Views,
                    IsFeatured = article.IsFeatured,
                    ReadTimeMinutes = article.ReadTimeMinutes,
                    AuthorUserId = article.AuthorUserId,
                    CreatedAt = article.CreatedAt
                };

                return CreatedAtAction(nameof(GetArticle), new { slug = article.Slug }, articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating article");
                return StatusCode(500, "An error occurred while creating the article");
            }
        }

        // PUT: api/news/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] UpdateArticleDTO updateDto)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                {
                    return NotFound();
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(updateDto.Title))
                {
                    article.Title = updateDto.Title;
                    article.Slug = GenerateSlug(updateDto.Title);
                }
                if (!string.IsNullOrEmpty(updateDto.Excerpt))
                    article.Excerpt = updateDto.Excerpt;
                if (!string.IsNullOrEmpty(updateDto.Content))
                    article.Content = updateDto.Content;
                if (!string.IsNullOrEmpty(updateDto.Category))
                    article.Category = updateDto.Category;
                if (updateDto.Tags != null)
                    article.Tags = string.Join(",", updateDto.Tags);
                if (!string.IsNullOrEmpty(updateDto.Author))
                    article.Author = updateDto.Author;
                if (updateDto.PublishedDate.HasValue)
                    article.PublishedDate = updateDto.PublishedDate.Value;
                if (updateDto.FeaturedImageUrl != null)
                    article.FeaturedImageUrl = updateDto.FeaturedImageUrl;
                if (updateDto.ImageUrls != null)
                    article.ImageUrls = updateDto.ImageUrls.Count > 0 ? JsonSerializer.Serialize(updateDto.ImageUrls) : null;
                if (!string.IsNullOrEmpty(updateDto.Status))
                    article.Status = updateDto.Status;
                if (updateDto.IsFeatured.HasValue)
                    article.IsFeatured = updateDto.IsFeatured.Value;
                if (updateDto.ReadTimeMinutes.HasValue)
                    article.ReadTimeMinutes = updateDto.ReadTimeMinutes.Value;

                article.UpdatedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating article {Id}", id);
                return StatusCode(500, "An error occurred while updating the article");
            }
        }

        // DELETE: api/news/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArticle(Guid id)
        {
            try
            {
                var article = await _context.Articles.FindAsync(id);
                if (article == null)
                {
                    return NotFound();
                }

                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article {Id}", id);
                return StatusCode(500, "An error occurred while deleting the article");
            }
        }

        // GET: api/news/categories
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            try
            {
                var categories = await _context.Articles
                    .Where(a => a.Status == "published")
                    .Select(a => a.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return StatusCode(500, "An error occurred while fetching categories");
            }
        }

        // GET: api/news/tags
        [HttpGet("tags")]
        public async Task<ActionResult<IEnumerable<string>>> GetTags()
        {
            try
            {
                var allTags = await _context.Articles
                    .Where(a => a.Status == "published" && !string.IsNullOrEmpty(a.Tags))
                    .Select(a => a.Tags)
                    .ToListAsync();

                var tags = allTags
                    .SelectMany(t => t.Split(','))
                    .Select(t => t.Trim())
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();

                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tags");
                return StatusCode(500, "An error occurred while fetching tags");
            }
        }

        private string GenerateSlug(string title)
        {
            // Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Remove special characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove duplicate hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            // Ensure uniqueness by appending a number if slug already exists
            var originalSlug = slug;
            var counter = 1;
            
            while (_context.Articles.Any(a => a.Slug == slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }

            return slug;
        }
    }
}
