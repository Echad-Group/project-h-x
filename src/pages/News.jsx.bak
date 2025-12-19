import React, { useEffect, useState, useMemo } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

// Article database with metadata
const ALL_ARTICLES = [
  { 
    id: 'youth-agenda', 
    titleKey: 'news.articles.youthAgenda.title',
    excerptKey: 'news.articles.youthAgenda.excerpt',
    date: '2025-10-15',
    categoryKey: 'news.articles.youthAgenda.category',
    tagsKey: 'news.articles.youthAgenda.tags',
    author: 'Campaign Team',
    readTime: 4,
    featured: true,
    views: 2340,
    images: [
      'https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=800',
      'https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=800'
    ]
  },
  { 
    id: 'healthcare-reform', 
    titleKey: 'news.articles.healthcareReform.title',
    excerptKey: 'news.articles.healthcareReform.excerpt',
    date: '2025-10-10',
    categoryKey: 'news.articles.healthcareReform.category',
    tagsKey: 'news.articles.healthcareReform.tags',
    author: 'Policy Team',
    readTime: 5,
    featured: false,
    views: 1890,
    images: [
      'https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=800'
    ]
  },
  { 
    id: 'nakuru-townhall', 
    titleKey: 'news.articles.nakuruTownhall.title',
    excerptKey: 'news.articles.nakuruTownhall.excerpt',
    date: '2025-09-28',
    categoryKey: 'news.articles.nakuruTownhall.category',
    tagsKey: 'news.articles.nakuruTownhall.tags',
    author: 'Field Team',
    readTime: 3,
    featured: false,
    views: 3120,
    images: [
      'https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=800',
      'https://images.unsplash.com/photo-1515187029135-18ee286d815b?w=800',
      'https://images.unsplash.com/photo-1431540015161-0bf868a2d407?w=800'
    ]
  },
  { 
    id: 'kisii-rally', 
    titleKey: 'news.articles.kisiiRally.title',
    excerptKey: 'news.articles.kisiiRally.excerpt',
    date: '2025-09-20',
    categoryKey: 'news.articles.kisiiRally.category',
    tagsKey: 'news.articles.kisiiRally.tags',
    author: 'Field Team',
    readTime: 3,
    featured: false,
    views: 2780,
    images: [
      'https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=800',
      'https://images.unsplash.com/photo-1557804506-669a67965ba0?w=800'
    ]
  },
  { 
    id: 'education-plan', 
    titleKey: 'news.articles.educationPlan.title',
    excerptKey: 'news.articles.educationPlan.excerpt',
    date: '2025-09-15',
    categoryKey: 'news.articles.educationPlan.category',
    tagsKey: 'news.articles.educationPlan.tags',
    author: 'Policy Team',
    readTime: 5,
    featured: true,
    views: 4200,
    images: [
      'https://images.unsplash.com/photo-1503676260728-1c00da094a0b?w=800'
    ]
  },
  { 
    id: 'rural-connectivity', 
    titleKey: 'news.articles.ruralConnectivity.title',
    excerptKey: 'news.articles.ruralConnectivity.excerpt',
    date: '2025-08-30',
    categoryKey: 'news.articles.ruralConnectivity.category',
    tagsKey: 'news.articles.ruralConnectivity.tags',
    author: 'Policy Team',
    readTime: 4,
    featured: false,
    views: 1560,
    images: [
      'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=800',
      'https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?w=800'
    ]
  },
  { 
    id: 'mombasa-port', 
    titleKey: 'news.articles.mombasaPort.title',
    excerptKey: 'news.articles.mombasaPort.excerpt',
    date: '2025-08-22',
    categoryKey: 'news.articles.mombasaPort.category',
    tagsKey: 'news.articles.mombasaPort.tags',
    author: 'Press Office',
    readTime: 4,
    featured: false,
    views: 2100,
    images: [
      'https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=800'
    ]
  },
  { 
    id: 'farmer-subsidies', 
    titleKey: 'news.articles.farmerSubsidies.title',
    excerptKey: 'news.articles.farmerSubsidies.excerpt',
    date: '2025-08-10',
    categoryKey: 'news.articles.farmerSubsidies.category',
    tagsKey: 'news.articles.farmerSubsidies.tags',
    author: 'Policy Team',
    readTime: 4,
    featured: false,
    views: 1920,
    images: [
      'https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=800',
      'https://images.unsplash.com/photo-1464226184884-fa280b87c399?w=800'
    ]
  },
  { 
    id: 'womens-empowerment', 
    titleKey: 'news.articles.womensEmpowerment.title',
    excerptKey: 'news.articles.womensEmpowerment.excerpt',
    date: '2025-07-28',
    categoryKey: 'news.articles.womensEmpowerment.category',
    tagsKey: 'news.articles.womensEmpowerment.tags',
    author: 'Community Team',
    readTime: 4,
    featured: false,
    views: 2650,
    images: [
      'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=800'
    ]
  }
];

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
  const { t } = useTranslation();
  const [searchParams, setSearchParams] = useSearchParams();

  const [searchQuery, setSearchQuery] = useState('');
  const [selectedCategory, setSelectedCategory] = useState(searchParams.get('category') || 'all');
  const [sortBy, setSortBy] = useState(searchParams.get('sort') || 'newest');
  const [currentPage, setCurrentPage] = useState(parseInt(searchParams.get('page')) || 1);

  useEffect(() => {
    updateMeta({
      title: 'News - New Kenya Campaign',
      description: 'Latest news, press releases, and policy updates from the New Kenya campaign.',
      url: '/news'
    });
  }, []);

  // Update URL when filters change
  useEffect(() => {
    const params = {};
    if (selectedCategory !== 'all') params.category = selectedCategory;
    if (sortBy !== 'newest') params.sort = sortBy;
    if (currentPage > 1) params.page = currentPage.toString();
    if (searchQuery) params.q = searchQuery;
    
    setSearchParams(params, { replace: true });
  }, [selectedCategory, sortBy, currentPage, searchQuery, setSearchParams]);

  // Get category value from translation
  const getCategoryValue = (article) => {
    const category = t(article.categoryKey);
    return category;
  };

  // Filter and sort articles
  const filteredAndSortedArticles = useMemo(() => {
    let results = [...ALL_ARTICLES];

    // Filter by search query
    if (searchQuery.trim()) {
      const query = searchQuery.toLowerCase();
      results = results.filter(article => 
        t(article.titleKey).toLowerCase().includes(query) ||
        t(article.excerptKey).toLowerCase().includes(query) ||
        article.author.toLowerCase().includes(query)
      );
    }

    // Filter by category
    if (selectedCategory !== 'all') {
      results = results.filter(article => getCategoryValue(article) === selectedCategory);
    }

    // Sort articles
    results.sort((a, b) => {
      switch (sortBy) {
        case 'newest':
          return new Date(b.date) - new Date(a.date);
        case 'oldest':
          return new Date(a.date) - new Date(b.date);
        case 'popular':
          return b.views - a.views;
        default:
          return 0;
      }
    });

    return results;
  }, [searchQuery, selectedCategory, sortBy, t]);

  // Pagination
  const totalPages = Math.ceil(filteredAndSortedArticles.length / ITEMS_PER_PAGE);
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const paginatedArticles = filteredAndSortedArticles.slice(startIndex, startIndex + ITEMS_PER_PAGE);
  const featuredArticle = ALL_ARTICLES.find(a => a.featured);

  // Reset to page 1 when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [searchQuery, selectedCategory, sortBy]);

  // Initialize from URL params on mount
  useEffect(() => {
    const query = searchParams.get('q');
    if (query) setSearchQuery(query);
  }, []);

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  };

  return (
    <section className="max-w-7xl mx-auto px-4 py-12">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900">{t('news.title')}</h1>
        <p className="mt-2 text-lg text-gray-600">{t('news.subtitle')}</p>
      </div>

      {/* Featured Article */}
      {featuredArticle && !searchQuery && selectedCategory === 'all' && currentPage === 1 && (
        <Link 
          to={`/news/${featuredArticle.id}`}
          className="block mb-12 bg-gradient-to-r from-green-50 to-red-50 rounded-lg overflow-hidden card-shadow hover:shadow-xl transition-shadow"
        >
          <div className="p-8">
            <div className="inline-block px-3 py-1 bg-[var(--kenya-red)] text-white text-xs font-semibold rounded-full mb-4">
              {t('news.featured')}
            </div>
            <h2 className="text-3xl font-bold text-gray-900 mb-3">{t(featuredArticle.titleKey)}</h2>
            <p className="text-gray-700 text-lg mb-4">{t(featuredArticle.excerptKey)}</p>
            <div className="flex items-center gap-4 text-sm text-gray-600">
              <span>{t('news.article.author', { name: featuredArticle.author })}</span>
              <span>•</span>
              <span>{formatDate(featuredArticle.date)}</span>
              <span>•</span>
              <span>{t('news.article.readTime', { min: featuredArticle.readTime })}</span>
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
            placeholder={t('news.search.placeholder')}
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
                {t(`news.categories.${cat}`)}
              </button>
            ))}
          </div>

          {/* Sort Dropdown */}
          <div className="flex items-center gap-2">
            <label className="text-sm font-medium text-gray-700">{t('news.sort.label')}:</label>
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
            >
              <option value="newest">{t('news.sort.newest')}</option>
              <option value="oldest">{t('news.sort.oldest')}</option>
              <option value="popular">{t('news.sort.popular')}</option>
            </select>
          </div>
        </div>

        {/* Results Count */}
        {searchQuery && (
          <div className="text-sm text-gray-600">
            {filteredAndSortedArticles.length === 0 
              ? t('news.search.noResults')
              : t('news.search.results', { count: filteredAndSortedArticles.length })
            }
          </div>
        )}
      </div>

      {/* Articles Grid */}
      {paginatedArticles.length > 0 ? (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
            {paginatedArticles.map(article => (
              <article key={article.id} className="bg-white rounded-lg overflow-hidden card-shadow hover:shadow-xl transition-shadow flex flex-col">
                {/* Image Carousel */}
                <Link to={`/news/${article.id}`} className="block">
                  <ImageCarousel images={article.images} />
                </Link>
                
                <div className="p-6 flex-1 flex flex-col">
                  <div className="flex items-center justify-between mb-3">
                    <span className="text-xs font-medium text-[var(--kenya-green)] uppercase">
                      {t(`news.categories.${getCategoryValue(article)}`)}
                    </span>
                    <span className="text-xs text-gray-500">{formatDate(article.date)}</span>
                  </div>
                  
                  <h3 className="text-xl font-bold text-gray-900 mb-2 flex-1">
                    {t(article.titleKey)}
                  </h3>
                  
                  <p className="text-gray-600 text-sm mb-4 line-clamp-3">
                    {t(article.excerptKey)}
                  </p>
                  
                  <div className="flex items-center justify-between text-xs text-gray-500 mb-4">
                    <span>{article.author}</span>
                    <span>{t('news.article.readTime', { min: article.readTime })}</span>
                  </div>
                  
                  <Link 
                    to={`/news/${article.id}`} 
                    className="inline-flex items-center text-[var(--kenya-green)] font-semibold hover:text-[var(--kenya-red)] transition-colors"
                  >
                    {t('news.readMore')}
                  </Link>
                </div>
              </article>
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-center gap-4">
              <button
                onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                disabled={currentPage === 1}
                className="px-4 py-2 border border-gray-300 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50 transition-colors"
              >
                {t('news.pagination.previous')}
              </button>
              
              <span className="text-sm text-gray-600">
                {t('news.pagination.page', { current: currentPage, total: totalPages })}
              </span>
              
              <button
                onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                disabled={currentPage === totalPages}
                className="px-4 py-2 border border-gray-300 rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-50 transition-colors"
              >
                {t('news.pagination.next')}
              </button>
            </div>
          )}
        </>
      ) : (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">{t('news.search.noResults')}</p>
        </div>
      )}
    </section>
  )
}
