import React, { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { getArticles, getFeaturedArticles } from '../services/newsService'

const ITEMS_PER_PAGE = 6;

// Image Carousel Component
function ImageCarousel({ images }) {
  const [currentIndex, setCurrentIndex] = useState(0);

  if (!images || images.length === 0) {
    return (
      <div className="w-full h-48 bg-gradient-to-br from-green-100 to-red-100 flex items-center justify-center">
        <svg className="w-16 h-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      </div>
    );
  }

  const goToNext = (e) => {
    e.preventDefault();
    setCurrentIndex((prev) => (prev + 1) % images.length);
  };

  const goToPrevious = (e) => {
    e.preventDefault();
    setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
  };

  const goToSlide = (index, e) => {
    e.preventDefault();
    setCurrentIndex(index);
  };

  return (
    <div className="relative w-full h-48 overflow-hidden bg-gray-100 group">
      {/* Images */}
      <div 
        className="flex transition-transform duration-500 ease-out h-full"
        style={{ transform: `translateX(-${currentIndex * 100}%)` }}
      >
        {images.map((image, index) => (
          <img
            key={index}
            src={image}
            alt={`Slide ${index + 1}`}
            className="w-full h-full object-cover flex-shrink-0"
            loading="lazy"
          />
        ))}
      </div>

      {/* Navigation Arrows - only show if multiple images */}
      {images.length > 1 && (
        <>
          <button
            onClick={goToPrevious}
            className="absolute left-2 top-1/2 -translate-y-1/2 bg-black/50 text-white p-2 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Previous image"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={goToNext}
            className="absolute right-2 top-1/2 -translate-y-1/2 bg-black/50 text-white p-2 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Next image"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

          {/* Dots Indicator */}
          <div className="absolute bottom-2 left-1/2 -translate-x-1/2 flex gap-1.5">
            {images.map((_, index) => (
              <button
                key={index}
                onClick={(e) => goToSlide(index, e)}
                className={`w-2 h-2 rounded-full transition-all ${
                  index === currentIndex 
                    ? 'bg-white w-6' 
                    : 'bg-white/60 hover:bg-white/80'
                }`}
                aria-label={`Go to slide ${index + 1}`}
              />
            ))}
          </div>
        </>
      )}

      {/* Image counter */}
      {images.length > 1 && (
        <div className="absolute top-2 right-2 bg-black/60 text-white text-xs px-2 py-1 rounded-full">
          {currentIndex + 1} / {images.length}
        </div>
      )}
    </div>
  );
}

export default function News(){
  const { updateMeta } = useMeta();
  const [searchParams, setSearchParams] = useSearchParams();

  // State management
  const [articles, setArticles] = useState([]);
  const [featuredArticle, setFeaturedArticle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchQuery, setSearchQuery] = useState(searchParams.get('q') || '');
  const [selectedCategory, setSelectedCategory] = useState(searchParams.get('category') || 'all');
  const [sortBy, setSortBy] = useState(searchParams.get('sort') || 'newest');
  const [currentPage, setCurrentPage] = useState(parseInt(searchParams.get('page')) || 1);
  const [totalCount, setTotalCount] = useState(0);

  // Fetch featured article
  useEffect(() => {
    const fetchFeatured = async () => {
      try {
        const featured = await getFeaturedArticles(1);
        if (featured && featured.length > 0) {
          setFeaturedArticle(featured[0]);
        }
      } catch (err) {
        console.error('Error fetching featured article:', err);
      }
    };

    fetchFeatured();
  }, []);

  // Fetch articles based on filters
  useEffect(() => {
    const fetchArticles = async () => {
      setLoading(true);
      setError(null);
      
      try {
        const params = {
          category: selectedCategory !== 'all' ? selectedCategory : null,
          search: searchQuery || null,
          sortBy,
          page: currentPage,
          pageSize: ITEMS_PER_PAGE,
        };

        const response = await getArticles(params);
        setArticles(response.articles);
        setTotalCount(response.totalCount);
      } catch (err) {
        console.error('Error fetching articles:', err);
        setError('Failed to load articles. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    fetchArticles();
  }, [searchQuery, selectedCategory, sortBy, currentPage]);

  // Update meta tags
  useEffect(() => {
    updateMeta({
      title: 'News & Updates - New Kenya Campaign',
      description: 'Latest news, press releases, and policy updates from the New Kenya campaign.',
      url: '/news'
    });
  }, [updateMeta]);

  // Update URL when filters change
  useEffect(() => {
    const params = {};
    if (selectedCategory !== 'all') params.category = selectedCategory;
    if (sortBy !== 'newest') params.sort = sortBy;
    if (currentPage > 1) params.page = currentPage.toString();
    if (searchQuery) params.q = searchQuery;
    setSearchParams(params);
  }, [selectedCategory, sortBy, currentPage, searchQuery, setSearchParams]);

  // Reset to page 1 when filters change
  useEffect(() => {
    if (currentPage !== 1) {
      setCurrentPage(1);
    }
  }, [searchQuery, selectedCategory, sortBy]);

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  };

  const totalPages = Math.ceil(totalCount / ITEMS_PER_PAGE);

  // Category mapping for display
  const getCategoryLabel = (category) => {
    const categoryMap = {
      'policy': 'Policy',
      'events': 'Events',
      'press': 'Press Release',
      'community': 'Community'
    };
    return categoryMap[category.toLowerCase()] || category;
  };

  return (
    <section className="max-w-7xl mx-auto px-4 py-12">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900">News & Updates</h1>
        <p className="mt-2 text-lg text-gray-600">
          Stay informed with the latest news and updates from our campaign
        </p>
      </div>

      {/* Featured Article */}
      {featuredArticle && !searchQuery && selectedCategory === 'all' && currentPage === 1 && !loading && (
        <Link 
          to={`/news/${featuredArticle.slug}`}
          className="block mb-12 bg-gradient-to-r from-green-50 to-red-50 rounded-lg overflow-hidden card-shadow hover:shadow-xl transition-shadow"
        >
          <div className="grid md:grid-cols-2 gap-6">
            <div className="relative h-64 md:h-auto">
              <img 
                src={featuredArticle.featuredImageUrl} 
                alt={featuredArticle.title}
                className="absolute inset-0 w-full h-full object-cover"
                loading="eager"
              />
            </div>
            <div className="p-8 flex flex-col justify-center">
              <div className="inline-block px-3 py-1 bg-[var(--kenya-red)] text-white text-xs font-semibold rounded-full mb-4 w-fit">
                FEATURED
              </div>
              <h2 className="text-3xl font-bold text-gray-900 mb-3">{featuredArticle.title}</h2>
              <p className="text-gray-700 text-lg mb-4">{featuredArticle.excerpt}</p>
              <div className="flex items-center gap-4 text-sm text-gray-600">
                <span>{featuredArticle.author}</span>
                <span>•</span>
                <span>{formatDate(featuredArticle.publishedDate)}</span>
                <span>•</span>
                <span>{featuredArticle.readTimeMinutes} min read</span>
              </div>
            </div>
          </div>
        </Link>
      )}

      {/* Search and Filters */}
      <div className="mb-8 space-y-4">
        {/* Search Bar */}
        <div className="relative">
          <input
            type="text"
            placeholder="Search articles..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            className="w-full px-4 py-3 pl-10 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
          />
          <svg className="absolute left-3 top-3.5 h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>

        {/* Filters Row */}
        <div className="flex flex-col sm:flex-row gap-4 items-start sm:items-center justify-between">
          {/* Category Filter */}
          <div className="flex flex-wrap gap-2">
            {['all', 'policy', 'events', 'press', 'community'].map(cat => (
              <button
                key={cat}
                onClick={() => setSelectedCategory(cat)}
                className={`px-4 py-2 rounded-full text-sm font-medium transition-colors ${
                  selectedCategory === cat
                    ? 'bg-[var(--kenya-green)] text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {getCategoryLabel(cat)}
              </button>
            ))}
          </div>

          {/* Sort Dropdown */}
          <div className="flex items-center gap-2">
            <label className="text-sm font-medium text-gray-700">Sort by:</label>
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
            >
              <option value="newest">Newest First</option>
              <option value="oldest">Oldest First</option>
              <option value="popular">Most Popular</option>
            </select>
          </div>
        </div>

        {/* Results Count */}
        {searchQuery && !loading && (
          <div className="text-sm text-gray-600">
            {totalCount === 0 
              ? 'No articles found'
              : `Found ${totalCount} article${totalCount !== 1 ? 's' : ''}`
            }
          </div>
        )}
      </div>

      {/* Loading State */}
      {loading && (
        <div className="flex items-center justify-center py-20">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--kenya-green)]"></div>
        </div>
      )}

      {/* Error State */}
      {error && !loading && (
        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6">
          {error}
        </div>
      )}

      {/* Articles Grid */}
      {!loading && !error && articles.length > 0 && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
            {articles.map(article => {
              const images = article.imageUrls && article.imageUrls.length > 0 
                ? article.imageUrls 
                : (article.featuredImageUrl ? [article.featuredImageUrl] : []);
              
              return (
                <article key={article.id} className="bg-white rounded-lg overflow-hidden card-shadow hover:shadow-xl transition-shadow flex flex-col">
                  {/* Image */}
                  <Link to={`/news/${article.slug}`} className="block">
                    <ImageCarousel images={images} />
                  </Link>
                  
                  <div className="p-6 flex-1 flex flex-col">
                    <div className="flex items-center justify-between mb-3">
                      <span className="text-xs font-medium text-[var(--kenya-green)] uppercase">
                        {getCategoryLabel(article.category)}
                      </span>
                      <span className="text-xs text-gray-500">{formatDate(article.publishedDate)}</span>
                    </div>
                    
                    <h3 className="text-xl font-bold text-gray-900 mb-2 flex-1">
                      {article.title}
                    </h3>
                    
                    <p className="text-gray-600 text-sm mb-4 line-clamp-3">
                      {article.excerpt}
                    </p>
                    
                    <div className="flex items-center justify-between text-xs text-gray-500 mb-4">
                      <span>{article.author}</span>
                      <span>{article.readTimeMinutes} min read</span>
                    </div>
                    
                    <Link 
                      to={`/news/${article.slug}`} 
                      className="inline-flex items-center text-[var(--kenya-green)] font-semibold hover:text-[var(--kenya-red)] transition-colors"
                    >
                      Read More →
                    </Link>
                  </div>
                </article>
              );
            })}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-center gap-4">
              <button
                onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                disabled={currentPage === 1}
                className="px-4 py-2 border border-gray-300 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50 transition-colors"
              >
                Previous
              </button>
              
              <span className="text-sm text-gray-600">
                Page {currentPage} of {totalPages}
              </span>
              
              <button
                onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                disabled={currentPage === totalPages}
                className="px-4 py-2 border border-gray-300 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50 transition-colors"
              >
                Next
              </button>
            </div>
          )}
        </>
      )}

      {/* No Results */}
      {!loading && !error && articles.length === 0 && (
        <div className="text-center py-12">
          <svg className="mx-auto h-16 w-16 text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <p className="text-gray-500 text-lg">No articles found</p>
          {searchQuery && (
            <button
              onClick={() => {
                setSearchQuery('');
                setSelectedCategory('all');
              }}
              className="mt-4 text-[var(--kenya-green)] hover:underline"
            >
              Clear filters
            </button>
          )}
        </div>
      )}
    </section>
  )
}
