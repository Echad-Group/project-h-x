import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

const ARTICLES = {
  'youth-agenda': {
    titleKey: 'news.articles.youthAgenda.title',
    date: 'Oct 2025',
    contentKey: 'news.articles.youthAgenda.content'
  },
  'nakuru-townhall': {
    titleKey: 'news.articles.nakuruTownhall.title',
    date: 'Sep 2025',
    contentKey: 'news.articles.nakuruTownhall.content'
  },
  'rural-connectivity': {
    titleKey: 'news.articles.ruralConnectivity.title',
    date: 'Sep 2025',
    contentKey: 'news.articles.ruralConnectivity.content'
  }
}

export default function NewsArticle(){
  const { id } = useParams();
  const article = ARTICLES[id] || null;
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    if(article){
      updateMeta({
        title: `${article.title}`,
        description: article.content.slice(0, 150),
        url: `/news/${id}`
      });
    }
  }, [id]);

  if(!article) return <div className="p-6">{t('news.notFound')}</div>

  return (
    <article className="max-w-3xl mx-auto px-4 py-12 bg-white rounded card-shadow">
      <div className="text-xs text-gray-500">{article.date}</div>
      <h1 className="text-2xl font-bold mt-2">{t(article.titleKey)}</h1>
      <div className="mt-4 text-gray-700">{t(article.contentKey)}</div>
    </article>
  )
}
