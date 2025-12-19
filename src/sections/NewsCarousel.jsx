import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getArticles } from '../services/newsService'

// Image Carousel Component
function ImageCarousel({ images }) {
  const [currentIndex, setCurrentIndex] = useState(0);

  if (!images || images.length === 0) {
    return (
      <div className="w-full aspect-video bg-gradient-to-br from-green-100 to-red-100 flex items-center justify-center">
        <svg className="w-16 h-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      </div>
    );
  }

  const goToNext = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIndex((prev) => (prev + 1) % images.length);
  };

  const goToPrevious = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
  };

  const goToSlide = (index, e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIndex(index);
  };

  return (
    <div className="relative w-full aspect-video overflow-hidden bg-gray-100 group">
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
            className="absolute left-2 top-1/2 -translate-y-1/2 bg-black/50 text-white p-1.5 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70 z-10"
            aria-label="Previous image"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={goToNext}
            className="absolute right-2 top-1/2 -translate-y-1/2 bg-black/50 text-white p-1.5 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70 z-10"
            aria-label="Next image"
          >
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

          {/* Dots Indicator */}
          <div className="absolute bottom-2 left-1/2 -translate-x-1/2 flex gap-1.5 z-10">
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
    </div>
  );
}

export default function NewsCarousel() {
  const [newsItems, setNewsItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchNews = async () => {
      try {
        const response = await getArticles({ 
          status: 'published', 
          sortBy: 'newest', 
          page: 1, 
          pageSize: 4 
        });
        setNewsItems(response.articles);
      } catch (err) {
        console.error('Error fetching news:', err);
        setError('Failed to load news');
      } finally {
        setLoading(false);
      }
    };

    fetchNews();
  }, []);

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short' });
  };

  const getCategoryLabel = (category) => {
    const categoryMap = {
      'policy': 'Policy',
      'events': 'Events',
      'press': 'Press Release',
      'community': 'Community'
    };
    return categoryMap[category.toLowerCase()] || category;
  };

  if (error) {
    return null; // Silently fail for better UX
  }

  if (loading) {
    return (
      <section aria-label="Latest news" className="py-8">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold">Latest News</h2>
          <Link to="/news" className="text-[var(--kenya-green)] hover:underline">
            View All →
          </Link>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="bg-white rounded-lg card-shadow overflow-hidden animate-pulse">
              <div className="aspect-video bg-gray-200"></div>
              <div className="p-4 space-y-3">
                <div className="h-4 bg-gray-200 rounded w-1/4"></div>
                <div className="h-6 bg-gray-200 rounded w-3/4"></div>
                <div className="h-4 bg-gray-200 rounded w-full"></div>
              </div>
            </div>
          ))}
        </div>
      </section>
    );
  }

  if (newsItems.length === 0) {
    return null; // Don't show section if no news
  }

  return (
    <section aria-label="Latest news" className="py-8">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-2xl font-bold">Latest News</h2>
        <Link to="/news" className="text-[var(--kenya-green)] hover:underline">
          View All →
        </Link>
      </div>
      
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {newsItems.map((item) => {
          const images = item.imageUrls && item.imageUrls.length > 0 
            ? item.imageUrls 
            : (item.featuredImageUrl ? [item.featuredImageUrl] : []);
          
          return (
            <article key={item.id} className="bg-white rounded-lg card-shadow overflow-hidden flex flex-col h-full transform transition hover:scale-105">
              <Link to={`/news/${item.slug}`} className="block">
                <ImageCarousel images={images} />
              </Link>
            
            <div className="p-4 flex-1 flex flex-col">
              <div className="flex items-center gap-2 mb-2">
                <span className="text-xs text-gray-500">{formatDate(item.publishedDate)}</span>
                <span className="text-xs px-2 py-1 bg-[var(--kenya-green)]/10 text-[var(--kenya-green)] rounded-full">
                  {getCategoryLabel(item.category)}
                </span>
              </div>
              
              <Link to={`/news/${item.slug}`}>
                <h3 className="font-semibold text-gray-900 hover:text-[var(--kenya-green)] transition-colors line-clamp-2">
                  {item.title}
                </h3>
              </Link>
              
              <p className="text-sm text-gray-600 mt-2 line-clamp-2 flex-1">
                {item.excerpt}
              </p>
              
              <Link 
                to={`/news/${item.slug}`}
                className="text-sm text-[var(--kenya-green)] font-medium hover:text-[var(--kenya-red)] transition-colors mt-3 inline-flex items-center"
              >
                Read more →
              </Link>
            </div>
          </article>
        );
        })}
      </div>
    </section>
  );
}
