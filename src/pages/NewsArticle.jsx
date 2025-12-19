import React, { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import ShareButtons from '../components/ShareButtons'
import { getArticleBySlug, incrementArticleViews, getArticles } from '../services/newsService'

// Article Image Carousel Component
function ArticleImageCarousel({ images }) {
  const [currentIndex, setCurrentIndex] = useState(0);

  if (!images || images.length === 0) return null;

  const goToNext = () => {
    setCurrentIndex((prev) => (prev + 1) % images.length);
  };

  const goToPrevious = () => {
    setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
  };

  const goToSlide = (index) => {
    setCurrentIndex(index);
  };

  return (
    <div className="relative w-full h-96 overflow-hidden bg-gray-100 rounded-lg mb-8 group">
      {/* Images */}
      <div 
        className="flex transition-transform duration-500 ease-out h-full"
        style={{ transform: `translateX(-${currentIndex * 100}%)` }}
      >
        {images.map((image, index) => (
          <img
            key={index}
            src={image}
            alt={`Article image ${index + 1}`}
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
            className="absolute left-4 top-1/2 -translate-y-1/2 bg-black/50 text-white p-3 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Previous image"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={goToNext}
            className="absolute right-4 top-1/2 -translate-y-1/2 bg-black/50 text-white p-3 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Next image"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

          {/* Thumbnail Navigation */}
          <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
            {images.map((image, index) => (
              <button
                key={index}
                onClick={() => goToSlide(index)}
                className={`w-16 h-16 rounded overflow-hidden border-2 transition-all ${
                  index === currentIndex 
                    ? 'border-white scale-110' 
                    : 'border-white/50 hover:border-white/80 opacity-70 hover:opacity-100'
                }`}
                aria-label={`Go to image ${index + 1}`}
              >
                <img src={image} alt={`Thumbnail ${index + 1}`} className="w-full h-full object-cover" />
              </button>
            ))}
          </div>

          {/* Image counter */}
          <div className="absolute top-4 right-4 bg-black/60 text-white text-sm px-3 py-1 rounded-full">
            {currentIndex + 1} / {images.length}
          </div>
        </>
      )}
    </div>
  );
}

export default function NewsArticle(){
  const { slug } = useParams();
  const [article, setArticle] = useState(null);
  const [relatedArticles, setRelatedArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { updateMeta } = useMeta();

  console.log('NewsArticle component rendered, slug:', slug);

  useEffect(() => {
    const fetchArticle = async () => {
      console.log('Fetching article with slug:', slug);
      setLoading(true);
      setError(null);
      
      try {
        // Fetch the article
        console.log('Calling getArticleBySlug...');
        const articleData = await getArticleBySlug(slug);
        console.log('Article data received:', articleData);
        setArticle(articleData);

        // Increment view count (fire and forget - don't await)
        incrementArticleViews(articleData.id).catch(err => 
          console.log('Failed to increment views:', err)
        );

        // Fetch related articles from the same category
        console.log('Fetching related articles...');
        const relatedResponse = await getArticles({
          category: articleData.category,
          status: 'published',
          pageSize: 4,
        });
        console.log('Related articles received:', relatedResponse);

        // Filter out current article and take first 3
        const related = relatedResponse.articles
          .filter(a => a.id !== articleData.id)
          .slice(0, 3);
        
        setRelatedArticles(related);
        console.log('Article loading complete');
      } catch (err) {
        console.error('Error fetching article:', err);
        setError('Failed to load article. Please try again later.');
      } finally {
        setLoading(false);
      }
    };

    if (slug) {
      fetchArticle();
    }
  }, [slug]);

  useEffect(() => {
    if (article) {
      updateMeta({
        title: `${article.title} - New Kenya Campaign`,
        description: article.excerpt.slice(0, 150),
        url: `/news/${slug}`
      });
    }
  }, [article, slug, updateMeta]);

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
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

  // Loading State
  if (loading) {
    return (
      <div className="bg-gray-50 min-h-screen">
        <div className="max-w-4xl mx-auto px-4 py-12">
          <div className="flex items-center justify-center py-20">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--kenya-green)]"></div>
          </div>
        </div>
      </div>
    );
  }

  // Error State
  if (error || !article) {
    return (
      <div className="bg-gray-50 min-h-screen">
        <div className="max-w-4xl mx-auto px-4 py-12">
          <div className="text-center">
            <h1 className="text-2xl font-bold text-gray-900 mb-4">
              {error || 'Article Not Found'}
            </h1>
            <Link to="/news" className="text-[var(--kenya-green)] hover:underline">
              ← Back to News
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-gray-50 min-h-screen">
      <article className="max-w-4xl mx-auto px-4 py-12">
        {/* Back to News Link */}
        <Link 
          to="/news" 
          className="inline-flex items-center text-[var(--kenya-green)] hover:text-[var(--kenya-red)] mb-6 transition-colors"
        >
          ← Back to News
        </Link>

        {/* Article Header */}
        <div className="bg-white rounded-lg p-8 card-shadow mb-6">
          <div className="flex items-center gap-3 mb-4">
            <span className="px-3 py-1 bg-[var(--kenya-green)] text-white text-xs font-semibold rounded-full uppercase">
              {getCategoryLabel(article.category)}
            </span>
            <span className="text-sm text-gray-500">{formatDate(article.publishedDate)}</span>
          </div>

          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            {article.title}
          </h1>

          <div className="flex items-center gap-4 text-sm text-gray-600 pb-6 border-b border-gray-200">
            <span>{article.author}</span>
            <span>•</span>
            <span>{article.readTimeMinutes} min read</span>
            <span>•</span>
            <span>{article.views} views</span>
          </div>
        </div>

        {/* Article Images Carousel */}
        {article.imageUrls && article.imageUrls.length > 0 && (
          <div className="px-8">
            <ArticleImageCarousel images={article.imageUrls} />
          </div>
        )}

        {/* Article Content */}
        <div className="px-8 pb-8">
          <div className="prose prose-lg max-w-none text-gray-700 leading-relaxed whitespace-pre-wrap">
            {article.content}
          </div>

          {/* Tags */}
          {article.tags && article.tags.length > 0 && (
            <div className="mt-8 pt-6 border-t border-gray-200">
              <h3 className="text-sm font-semibold text-gray-700 mb-3">Tags</h3>
              <div className="flex flex-wrap gap-2">
                {article.tags.map((tag, index) => (
                  <span 
                    key={index}
                    className="px-3 py-1 bg-gray-100 text-gray-700 text-sm rounded-full"
                  >
                    #{tag.trim()}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Share Buttons */}
          <div className="mt-8 pt-6 border-t border-gray-200">
            <h3 className="text-sm font-semibold text-gray-700 mb-3">Share this article</h3>
            <ShareButtons 
              url={`${window.location.origin}/news/${slug}`}
              title={article.title}
            />
          </div>
        </div>

        {/* Related Articles */}
        {relatedArticles.length > 0 && (
          <div className="mt-12">
            <h2 className="text-2xl font-bold text-gray-900 mb-6">Related Articles</h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {relatedArticles.map((related) => (
                <Link 
                  key={related.id}
                  to={`/news/${related.slug}`}
                  className="bg-white rounded-lg overflow-hidden card-shadow hover:shadow-xl transition-shadow"
                >
                  {related.featuredImageUrl && (
                    <div className="aspect-video bg-gray-100 relative overflow-hidden">
                      <img 
                        src={related.featuredImageUrl} 
                        alt={related.title}
                        className="absolute inset-0 w-full h-full object-cover"
                        loading="lazy"
                      />
                    </div>
                  )}
                  <div className="p-4">
                    <div className="flex items-center gap-2 mb-2">
                      <span className="text-xs text-gray-500">{formatDate(related.publishedDate)}</span>
                    </div>
                    <h3 className="font-semibold text-gray-900 line-clamp-2 mb-2">
                      {related.title}
                    </h3>
                    <p className="text-sm text-gray-600 line-clamp-2">
                      {related.excerpt}
                    </p>
                  </div>
                </Link>
              ))}
            </div>
          </div>
        )}
      </article>
    </div>
  )
}
