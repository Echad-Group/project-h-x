import React, { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

const posts = [
  { id: 'youth-agenda', title: 'Campaign launches nationwide youth agenda', date: 'Oct 2025', excerpt: 'A bold plan to support startups, apprenticeships, and internships.' },
  { id: 'nakuru-townhall', title: 'Townhall in Nakuru draws thousands', date: 'Sep 2025', excerpt: 'Community dialogue on jobs and local governance.' },
  { id: 'rural-connectivity', title: 'Policy paper: Rural digital connectivity', date: 'Aug 2025', excerpt: 'Bringing high-speed internet to every county.' }
];

export default function News(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    updateMeta({
      title: 'News - New Kenya Campaign',
      description: 'Latest news, press releases, and policy updates from the New Kenya campaign.',
      url: '/news'
    });
  }, []);

  return (
    <section className="max-w-6xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">{t('news.title')}</h1>
      <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-6">
        {posts.map(p => (
          <article key={p.id} className="bg-white p-4 rounded card-shadow">
            <div className="text-xs text-gray-500">{p.date}</div>
            <h3 className="font-semibold mt-1">{p.title}</h3>
            <p className="mt-2 text-sm text-gray-600">{p.excerpt}</p>
            <Link to={`/news/${p.id}`} className="mt-3 inline-block text-[var(--kenya-green)]">{t('news.readMore')}</Link>
          </article>
        ))}
      </div>
    </section>
  )
}
