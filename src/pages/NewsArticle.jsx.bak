import React, { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'
import ShareButtons from '../components/ShareButtons'

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

const ARTICLES = {
  'youth-agenda': {
    titleKey: 'news.articles.youthAgenda.title',
    excerptKey: 'news.articles.youthAgenda.excerpt',
    date: '2025-10-15',
    contentKey: 'news.articles.youthAgenda.content',
    categoryKey: 'news.articles.youthAgenda.category',
    tagsKey: 'news.articles.youthAgenda.tags',
    author: 'Campaign Team',
    readTime: 4,
    images: [
      'https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=1200',
      'https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=1200'
    ]
  },
  'healthcare-reform': {
    titleKey: 'news.articles.healthcareReform.title',
    excerptKey: 'news.articles.healthcareReform.excerpt',
    date: '2025-10-10',
    contentKey: 'news.articles.healthcareReform.content',
    categoryKey: 'news.articles.healthcareReform.category',
    tagsKey: 'news.articles.healthcareReform.tags',
    author: 'Policy Team',
    readTime: 5,
    images: [
      'https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=1200'
    ]
  },
  'nakuru-townhall': {
    titleKey: 'news.articles.nakuruTownhall.title',
    excerptKey: 'news.articles.nakuruTownhall.excerpt',
    date: '2025-09-28',
    contentKey: 'news.articles.nakuruTownhall.content',
    categoryKey: 'news.articles.nakuruTownhall.category',
    tagsKey: 'news.articles.nakuruTownhall.tags',
    author: 'Field Team',
    readTime: 3,
    images: [
      'https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=1200',
      'https://images.unsplash.com/photo-1515187029135-18ee286d815b?w=1200',
      'https://images.unsplash.com/photo-1431540015161-0bf868a2d407?w=1200'
    ]
  },
  'kisii-rally': {
    titleKey: 'news.articles.kisiiRally.title',
    excerptKey: 'news.articles.kisiiRally.excerpt',
    date: '2025-09-20',
    contentKey: 'news.articles.kisiiRally.content',
    categoryKey: 'news.articles.kisiiRally.category',
    tagsKey: 'news.articles.kisiiRally.tags',
    author: 'Field Team',
    readTime: 3,
    images: [
      'https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=1200',
      'https://images.unsplash.com/photo-1557804506-669a67965ba0?w=1200'
    ]
  },
  'education-plan': {
    titleKey: 'news.articles.educationPlan.title',
    excerptKey: 'news.articles.educationPlan.excerpt',
    date: '2025-09-15',
    contentKey: 'news.articles.educationPlan.content',
    categoryKey: 'news.articles.educationPlan.category',
    tagsKey: 'news.articles.educationPlan.tags',
    author: 'Policy Team',
    readTime: 5,
    images: [
      'https://images.unsplash.com/photo-1503676260728-1c00da094a0b?w=1200'
    ]
  },
  'rural-connectivity': {
    titleKey: 'news.articles.ruralConnectivity.title',
    excerptKey: 'news.articles.ruralConnectivity.excerpt',
    date: '2025-08-30',
    contentKey: 'news.articles.ruralConnectivity.content',
    categoryKey: 'news.articles.ruralConnectivity.category',
    tagsKey: 'news.articles.ruralConnectivity.tags',
    author: 'Policy Team',
    readTime: 4,
    images: [
      'https://images.unsplash.com/photo-1451187580459-43490279c0fa?w=1200',
      'https://images.unsplash.com/photo-1488590528505-98d2b5aba04b?w=1200'
    ]
  },
  'mombasa-port': {
    titleKey: 'news.articles.mombasaPort.title',
    excerptKey: 'news.articles.mombasaPort.excerpt',
    date: '2025-08-22',
    contentKey: 'news.articles.mombasaPort.content',
    categoryKey: 'news.articles.mombasaPort.category',
    tagsKey: 'news.articles.mombasaPort.tags',
    author: 'Press Office',
    readTime: 4,
    images: [
      'https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=1200'
    ]
  },
  'farmer-subsidies': {
    titleKey: 'news.articles.farmerSubsidies.title',
    excerptKey: 'news.articles.farmerSubsidies.excerpt',
    date: '2025-08-10',
    contentKey: 'news.articles.farmerSubsidies.content',
    categoryKey: 'news.articles.farmerSubsidies.category',
    tagsKey: 'news.articles.farmerSubsidies.tags',
    author: 'Policy Team',
    readTime: 4,
    images: [
      'https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=1200',
      'https://images.unsplash.com/photo-1464226184884-fa280b87c399?w=1200'
    ]
  },
  'womens-empowerment': {
    titleKey: 'news.articles.womensEmpowerment.title',
    excerptKey: 'news.articles.womensEmpowerment.excerpt',
    date: '2025-07-28',
    contentKey: 'news.articles.womensEmpowerment.content',
    categoryKey: 'news.articles.womensEmpowerment.category',
    tagsKey: 'news.articles.womensEmpowerment.tags',
    author: 'Community Team',
    readTime: 4,
    images: [
      'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=1200'
    ]
  }
}

export default function NewsArticle(){
  const { id } = useParams();
  const article = ARTICLES[id] || null;
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    if(article){
      const title = t(article.titleKey);
      const excerpt = t(article.excerptKey);
      updateMeta({
        title: `${title} - New Kenya Campaign`,
        description: excerpt.slice(0, 150),
        url: `/news/${id}`
      });
    }
  }, [id, article, t, updateMeta]);

  if(!article) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-12">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 mb-4">{t('news.notFound')}</h1>
          <Link to="/news" className="text-[var(--kenya-green)] hover:underline">
            {t('news.article.backToNews')}
          </Link>
        </div>
      </div>
    );
  }

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' });
  };

  // Get related articles (same category, excluding current)
  const relatedArticles = Object.entries(ARTICLES)
    .filter(([articleId, art]) => 
      articleId !== id && 
      t(art.categoryKey) === t(article.categoryKey)
    )
    .slice(0, 3)
    .map(([articleId, art]) => ({ id: articleId, ...art }));

  const tags = t(article.tagsKey, { returnObjects: true });
  const category = t(article.categoryKey);

  return (
    <div className="bg-gray-50 min-h-screen">
      <article className="max-w-4xl mx-auto px-4 py-12">
        {/* Back to News Link */}
        <Link 
          to="/news" 
          className="inline-flex items-center text-[var(--kenya-green)] hover:text-[var(--kenya-red)] mb-6 transition-colors"
        >
          {t('news.article.backToNews')}
        </Link>

        {/* Article Header */}
        <div className="bg-white rounded-lg p-8 card-shadow mb-6">
          <div className="flex items-center gap-3 mb-4">
            <span className="px-3 py-1 bg-[var(--kenya-green)] text-white text-xs font-semibold rounded-full uppercase">
              {t(`news.categories.${category}`)}
            </span>
            <span className="text-sm text-gray-500">{formatDate(article.date)}</span>
          </div>

          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            {t(article.titleKey)}
          </h1>

          <div className="flex items-center gap-4 text-sm text-gray-600 pb-6 border-b border-gray-200">
            <span>{t('news.article.author', { name: article.author })}</span>
            <span>•</span>
            <span>{t('news.article.readTime', { min: article.readTime })}</span>
          </div>
        </div>

        {/* Article Images Carousel */}
        {article.images && article.images.length > 0 && (
          <div className="px-8">
            <ArticleImageCarousel images={article.images} />
          </div>
        )}

        {/* Article Content */}
        <div className="px-8 pb-8">
          <div className="prose prose-lg max-w-none text-gray-700 leading-relaxed">
            {t(article.contentKey)}
          </div>

          {/* Tags */}
          {tags && tags.length > 0 && (
            <div className="mt-8 pt-6 border-t border-gray-200">
              <h3 className="text-sm font-semibold text-gray-700 mb-3">{t('news.article.tags')}</h3>
              <div className="flex flex-wrap gap-2">
                {tags.map((tag, index) => (
                  <span 
                    key={index}
                    className="px-3 py-1 bg-gray-100 text-gray-700 text-sm rounded-full"
                  >
                    #{tag}
                  </span>
                ))}
              </div>
            </div>
          )}

          {/* Share Buttons */}
          <div className="mt-8 pt-6 border-t border-gray-200">
            <h3 className="text-sm font-semibold text-gray-700 mb-3">{t('news.article.share')}</h3>
            <ShareButtons 
              url={`${window.location.origin}/news/${id}`}
              title={t(article.titleKey)}
            />
          </div>
        </div>

        {/* Related Articles */}
        {relatedArticles.length > 0 && (
          <div className="bg-white rounded-lg p-8 card-shadow">
            <h2 className="text-2xl font-bold text-gray-900 mb-6">
              {t('news.article.relatedTitle')}
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {relatedArticles.map(related => (
                <Link
                  key={related.id}
                  to={`/news/${related.id}`}
                  className="group"
                >
                  <div className="border border-gray-200 rounded-lg p-4 hover:shadow-lg transition-shadow">
                    <div className="text-xs text-gray-500 mb-2">
                      {formatDate(related.date)}
                    </div>
                    <h3 className="font-semibold text-gray-900 group-hover:text-[var(--kenya-green)] transition-colors mb-2">
                      {t(related.titleKey)}
                    </h3>
                    <p className="text-sm text-gray-600 line-clamp-2">
                      {t(related.excerptKey)}
                    </p>
                    <div className="mt-3 text-[var(--kenya-green)] text-sm font-medium">
                      {t('news.readMore')}
                    </div>
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
