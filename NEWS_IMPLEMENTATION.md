# News & Updates System Implementation

## Overview

Complete revision of the News & Updates system from hardcoded frontend data to a full-stack solution with backend API, database storage, and dynamic content management.

## Implementation Date
December 18, 2025

## What Changed

### From
- Hardcoded news articles in frontend components
- Static content with no backend
- No ability to manage articles dynamically
- Internationalization (i18n) keys for all content

### To
- Full REST API with CRUD operations
- Database-backed article storage
- Dynamic content loading with filtering, search, and pagination
- Admin interface for content management (planned)
- Real-time view tracking
- Featured articles system

## Architecture

### Backend (ASP.NET Core)

**Models** (`backend/NewKenyaAPI/Models/`)
- **Article.cs**: Main article entity
  - Id (Guid)
  - Title, Slug, Excerpt, Content
  - Category, Tags (comma-separated)
  - Author, AuthorUserId (FK to ApplicationUser)
  - PublishedDate, UpdatedDate, CreatedAt
  - FeaturedImageUrl, ImageUrls (JSON array)
  - Status (draft, published, archived)
  - Views, IsFeatured, ReadTimeMinutes

- **DTOs** (`Models/DTOs/ArticleDTO.cs`)
  - ArticleDTO: Full article data
  - ArticleListItemDTO: Summarized data for lists
  - CreateArticleDTO: Article creation payload
  - UpdateArticleDTO: Partial updates

**Controller** (`Controllers/NewsController.cs`)

Endpoints:
```
GET    /api/news                    - List articles with filters
GET    /api/news/featured           - Get featured articles
GET    /api/news/{slug}             - Get single article
POST   /api/news                    - Create article (Admin)
PUT    /api/news/{id}               - Update article (Admin)
DELETE /api/news/{id}               - Delete article (Admin)
POST   /api/news/{id}/increment-view - Increment view count
GET    /api/news/categories         - Get all categories
GET    /api/news/tags               - Get all tags
```

**Query Parameters** (GET /api/news):
- `category`: Filter by category (policy, events, press, community)
- `search`: Search in title, excerpt, content, tags
- `status`: Filter by status (default: published)
- `sortBy`: newest (default), oldest, popular
- `page`: Page number (default: 1)
- `pageSize`: Items per page (default: 12)

**Response Headers**:
- `X-Total-Count`: Total number of matching articles
- `X-Page`: Current page
- `X-Page-Size`: Items per page

**Database**

Migration: `20251218101652_AddNewsArticles`

Table: **Articles**
- Primary Key: Id (TEXT/Guid)
- Unique Index: Slug
- Indexes: Status, Category, PublishedDate, IsFeatured
- Foreign Key: AuthorUserId → AspNetUsers(Id)

**Features**:
- Automatic slug generation from title
- Slug uniqueness enforcement
- Image URLs stored as JSON array
- Tags stored as comma-separated values
- Full-text search across multiple fields
- Soft delete support via Status field

### Frontend (React)

**Service** (`src/services/newsService.js`)

Functions:
- `getArticles(params)`: Fetch articles with filtering
- `getFeaturedArticles(limit)`: Get featured articles
- `getArticleBySlug(slug)`: Get single article
- `incrementArticleViews(id)`: Track article views
- `createArticle(data)`: Create new article (Admin)
- `updateArticle(id, updates)`: Update article (Admin)
- `deleteArticle(id)`: Delete article (Admin)
- `getCategories()`: Get available categories
- `getTags()`: Get all tags

**Pages**

**News.jsx** (`src/pages/News.jsx`)
- Complete rewrite with API integration
- Real-time data fetching with loading states
- Search functionality (debounced)
- Category filtering (all, policy, events, press, community)
- Sorting options (newest, oldest, popular)
- Pagination (6 articles per page)
- Featured article display
- URL-based state management
- Responsive grid layout
- Error handling with user-friendly messages

**NewsArticle.jsx** (`src/pages/NewsArticle.jsx`)
- Dynamic article fetching by slug
- Automatic view tracking
- Multi-image carousel
- Related articles from same category
- Social share buttons
- Tag display
- Meta tags for SEO
- Loading and error states

**Sections**

**NewsCarousel.jsx** (`src/sections/NewsCarousel.jsx`)
- Fetches 4 featured articles from API
- Loading skeleton screens
- Graceful error handling
- Responsive card layout
- Category badges
- Direct links to full articles

### UI/UX Improvements

**Loading States**
- Skeleton screens for carousel
- Spinner for article lists
- Loading indicators throughout

**Error Handling**
- User-friendly error messages
- Fallback content display
- Silent failures for non-critical features

**Performance**
- Lazy loading images
- Optimized API calls
- Pagination to reduce data transfer
- View tracking doesn't block UI

**Accessibility**
- Proper ARIA labels
- Semantic HTML
- Keyboard navigation support
- Screen reader friendly

## Data Seeding

**Script**: `backend/seed-news.ps1`

Features:
- Seeds 9 sample articles
- Covers all categories
- Mix of featured and regular articles
- Realistic content and metadata
- Progress reporting
- Error handling

Usage:
```powershell
.\backend\seed-news.ps1 -Token "your-admin-jwt-token"
```

Sample Articles:
1. Youth Empowerment Initiative (Policy, Featured)
2. Healthcare Reform Plan (Policy)
3. Nakuru Town Hall Meeting (Events)
4. Kisii Rally (Events)
5. Education Plan (Policy, Featured)
6. Rural Connectivity Initiative (Policy)
7. Mombasa Port Expansion (Press)
8. Agricultural Subsidies (Policy)
9. Women's Economic Empowerment (Community)

## Categories

- **Policy**: Government policies and reforms
- **Events**: Campaign events and rallies
- **Press**: Official press releases
- **Community**: Community programs and initiatives

## API Security

**Authentication**:
- Public endpoints: GET /api/news, GET /api/news/{slug}, GET /api/news/featured
- Protected endpoints: POST, PUT, DELETE (requires Admin role)
- JWT Bearer token authentication

**Authorization**:
- Admin role required for content management
- Author ID automatically set from authenticated user
- View tracking is public (no auth required)

## Usage Examples

### Fetching Articles

```javascript
import { getArticles } from '../services/newsService';

// Get latest articles
const { articles, totalCount } = await getArticles();

// Filter by category
const { articles } = await getArticles({ category: 'policy' });

// Search articles
const { articles } = await getArticles({ search: 'healthcare' });

// Paginated results
const { articles, totalPages } = await getArticles({
  page: 2,
  pageSize: 10
});
```

### Getting Single Article

```javascript
import { getArticleBySlug, incrementArticleViews } from '../services/newsService';

const article = await getArticleBySlug('youth-empowerment-initiative');
incrementArticleViews(article.id); // Track view (fire and forget)
```

### Creating Article (Admin)

```javascript
import { createArticle } from '../services/newsService';

const newArticle = await createArticle({
  title: "New Policy Announcement",
  excerpt: "Brief summary...",
  content: "Full article content...",
  category: "policy",
  tags: ["economy", "jobs", "development"],
  author: "Policy Team",
  publishedDate: new Date().toISOString(),
  featuredImageUrl: "https://example.com/image.jpg",
  imageUrls: ["https://example.com/image1.jpg"],
  status: "published",
  isFeatured: false,
  readTimeMinutes: 5
});
```

## Testing

### Manual Testing

1. **View Articles**:
   - Navigate to `/news`
   - Verify articles load from API
   - Test pagination
   - Test category filters
   - Test search functionality

2. **View Single Article**:
   - Click on an article
   - Verify content loads
   - Check related articles
   - Test social share buttons

3. **Admin Operations** (requires admin account):
   - Create new article
   - Update existing article
   - Delete article
   - Publish/unpublish articles

### API Testing

Using the VS Code REST Client or Postman:

```http
### Get all articles
GET http://localhost:5065/api/news

### Get featured articles
GET http://localhost:5065/api/news/featured?limit=4

### Get article by slug
GET http://localhost:5065/api/news/youth-empowerment-initiative-launches-across-kenya

### Search articles
GET http://localhost:5065/api/news?search=healthcare

### Filter by category
GET http://localhost:5065/api/news?category=policy

### Create article (requires admin token)
POST http://localhost:5065/api/news
Authorization: Bearer <your-admin-token>
Content-Type: application/json

{
  "title": "Test Article",
  "excerpt": "Test excerpt",
  "content": "Test content",
  "category": "policy",
  "tags": ["test"],
  "author": "Test Author",
  "status": "draft",
  "readTimeMinutes": 3
}
```

## Future Enhancements

### Planned Features

1. **Admin Management Interface**:
   - Rich text editor for content
   - Image upload functionality
   - Draft/publish workflow
   - Bulk operations
   - Analytics dashboard

2. **Enhanced Content**:
   - Video embeds
   - Image galleries
   - Polls and surveys
   - Comments section
   - Social media integration

3. **SEO Optimization**:
   - Dynamic meta tags
   - Structured data (JSON-LD)
   - Sitemap generation
   - RSS feed

4. **Analytics**:
   - Detailed view tracking
   - User engagement metrics
   - Popular articles dashboard
   - Traffic sources

5. **Content Features**:
   - Scheduled publishing
   - Auto-save drafts
   - Version history
   - Multi-author support
   - Content approval workflow

6. **User Features**:
   - Bookmarking articles
   - Reading lists
   - Email notifications
   - Personalized recommendations

## Migration from Old System

**Before**: Hardcoded articles in components with i18n keys

```javascript
const ALL_ARTICLES = [
  { 
    id: 'youth-agenda', 
    titleKey: 'news.articles.youthAgenda.title',
    excerptKey: 'news.articles.youthAgenda.excerpt',
    // ... more i18n keys
  }
];
```

**After**: API-driven with database storage

```javascript
useEffect(() => {
  const fetchArticles = async () => {
    const { articles } = await getArticles();
    setArticles(articles);
  };
  fetchArticles();
}, []);
```

**Benefits**:
- No code deployment needed for content updates
- Scalable to thousands of articles
- Real-time content management
- Better performance with pagination
- Analytics and tracking capabilities
- SEO-friendly with dynamic meta tags

## Performance Considerations

**Optimizations**:
- Database indexes on frequently queried fields
- Pagination to limit data transfer
- Lazy loading images
- View tracking doesn't block rendering
- Caching headers on GET endpoints
- Image CDN support

**Recommended**:
- Implement Redis caching for frequently accessed articles
- Use CDN for featured images
- Add database query optimization
- Implement full-text search indexing
- Consider read replicas for scaling

## Troubleshooting

### Articles Not Loading

**Check**:
1. API is running: `http://localhost:5065/api/news`
2. Database has articles: Run seed script
3. CORS configuration allows frontend origin
4. Network tab in browser DevTools for errors

### Slug Not Found (404)

**Causes**:
- Article doesn't exist in database
- Slug mismatch (check exact slug)
- Article status is 'draft' or 'archived'

### Images Not Displaying

**Verify**:
- Image URLs are valid and accessible
- CORS allows image loading
- ImageUrls JSON is properly formatted
- FeaturedImageUrl is set

### Admin Operations Fail (401/403)

**Ensure**:
- Valid admin JWT token
- Token not expired
- User has Admin role
- Authorization header format: `Bearer <token>`

## Maintenance

### Regular Tasks

- Monitor API error logs
- Review slow queries
- Archive old articles
- Backup database regularly
- Update featured articles rotation
- Clean up unused images

### Updates

- Keep ASP.NET Core updated
- Update React dependencies
- Review and update security policies
- Monitor API usage patterns
- Optimize based on analytics

## Related Documentation

- [PRODUCTION_CHECKLIST.md](./PRODUCTION_CHECKLIST.md) - Deployment checklist
- [API_TESTING.md](./backend/API_TESTING.md) - API testing guide
- [README.md](./README.md) - Project overview

## Conclusion

The News & Updates system has been completely revised from a static, hardcoded solution to a dynamic, database-driven system with full CRUD capabilities, search, filtering, and pagination. The implementation provides a solid foundation for future enhancements while maintaining excellent performance and user experience.

**Key Achievements**:
✅ Full REST API with comprehensive endpoints
✅ Database schema with proper indexing
✅ Dynamic frontend with real-time data
✅ Search and filtering capabilities
✅ View tracking and analytics foundation
✅ Featured articles system
✅ Related articles recommendation
✅ Responsive and accessible UI
✅ Loading states and error handling
✅ Seed data for testing
✅ Ready for admin interface integration
