import React from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'

// Sync with News.jsx - Latest 4 articles
const getNewsItems = (t) => [
  {
    id: 'youth-agenda',
    titleKey: 'news.articles.youthAgenda.title',
    excerptKey: 'news.articles.youthAgenda.excerpt',
    date: '2025-10-15',
    categoryKey: 'news.articles.youthAgenda.category',
    image: 'https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=800'
  },
  {
    id: 'healthcare-reform',
    titleKey: 'news.articles.healthcareReform.title',
    excerptKey: 'news.articles.healthcareReform.excerpt',
    date: '2025-10-10',
    categoryKey: 'news.articles.healthcareReform.category',
    image: 'https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=800'
  },
  {
    id: 'nakuru-townhall',
    titleKey: 'news.articles.nakuruTownhall.title',
    excerptKey: 'news.articles.nakuruTownhall.excerpt',
    date: '2025-09-28',
    categoryKey: 'news.articles.nakuruTownhall.category',
    image: 'https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=800'
  },
  {
    id: 'kisii-rally',
    titleKey: 'news.articles.kisiiRally.title',
    excerptKey: 'news.articles.kisiiRally.excerpt',
    date: '2025-09-20',
    categoryKey: 'news.articles.kisiiRally.category',
    image: 'https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=800'
  }
];

export default function NewsCarousel() {
  const { t } = useTranslation();
  const newsItems = getNewsItems(t);

  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short' });
  };

  return (
    <section aria-label="Latest news" className="py-8">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold">{t('news.latest')}</h2>
        <Link to="/news" className="text-[var(--kenya-green)] hover:underline">
          {t('news.readMore')} →
        </Link>
      </div>
      
      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {newsItems.map((item) => (
          <article key={item.id} className="bg-white rounded-lg card-shadow overflow-hidden flex flex-col h-full transform transition hover:scale-105">
            <Link to={`/news/${item.id}`} className="aspect-video bg-gray-100 relative overflow-hidden">
              <img 
                src={item.image} 
                alt={t(item.titleKey)}
                className="absolute inset-0 w-full h-full object-cover"
                loading="lazy"
              />
            </Link>
            
            <div className="p-4 flex-1 flex flex-col">
              <div className="flex items-center gap-2 mb-2">
                <span className="text-xs text-gray-500">{formatDate(item.date)}</span>
                <span className="text-xs px-2 py-1 bg-[var(--kenya-green)]/10 text-[var(--kenya-green)] rounded-full">
                  {t(`news.categories.${t(item.categoryKey)}`)}
                </span>
              </div>
              
              <h3 className="font-semibold text-gray-900 flex-1">
                {t(item.titleKey)}
              </h3>
              
              <p className="mt-2 text-sm text-gray-600 line-clamp-2">
                {t(item.excerptKey)}
              </p>
              
              <Link 
                to={`/news/${item.id}`}
                className="mt-4 inline-flex items-center gap-1 text-[var(--kenya-green)] font-medium hover:gap-2 transition-all"
              >
                {t('news.readMore')}
                {/* <span>→</span> */}
              </Link>
            </div>
          </article>
        ))}
      </div>
      
      {/* Social Media Feed Preview */}
      <div className="mt-12">
        <h3 className="text-xl font-semibold mb-4">Follow Our Campaign</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <a 
            href="https://twitter.com/newkenya" 
            target="_blank"
            rel="noopener noreferrer"
            className="p-4 bg-white rounded-lg card-shadow hover:shadow-lg transition"
          >
            <div className="flex items-center gap-2 text-[#1DA1F2]">
              <span className="font-bold">Twitter</span>
              <span>@newkenya</span>
            </div>
          </a>
          
          <a 
            href="https://facebook.com/newkenya" 
            target="_blank"
            rel="noopener noreferrer"
            className="p-4 bg-white rounded-lg card-shadow hover:shadow-lg transition"
          >
            <div className="flex items-center gap-2 text-[#4267B2]">
              <span className="font-bold">Facebook</span>
              <span>/newkenya</span>
            </div>
          </a>
          
          <a 
            href="https://instagram.com/newkenya" 
            target="_blank"
            rel="noopener noreferrer"
            className="p-4 bg-white rounded-lg card-shadow hover:shadow-lg transition"
          >
            <div className="flex items-center gap-2 text-[#E1306C]">
              <span className="font-bold">Instagram</span>
              <span>@newkenya</span>
            </div>
          </a>
        </div>
      </div>
    </section>
  )
}
