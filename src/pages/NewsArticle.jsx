import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'

const ARTICLES = {
  'youth-agenda': {
    title: 'Campaign launches nationwide youth agenda',
    date: 'Oct 2025',
    content: 'Today we launched a nationwide youth agenda to create apprenticeship and startup opportunities...' 
  },
  'nakuru-townhall': {
    title: 'Townhall in Nakuru draws thousands',
    date: 'Sep 2025',
    content: 'The Nakuru townhall brought community leaders together to discuss local governance and jobs.'
  },
  'rural-connectivity': {
    title: 'Policy paper: Rural digital connectivity',
    date: 'Aug 2025',
    content: 'Our policy paper outlines a roadmap to bring high-speed internet to rural counties.'
  }
}

export default function NewsArticle(){
  const { id } = useParams();
  const article = ARTICLES[id] || null;
  const { updateMeta } = useMeta();

  useEffect(() => {
    if(article){
      updateMeta({
        title: `${article.title}`,
        description: article.content.slice(0, 150),
        url: `/news/${id}`
      });
    }
  }, [id]);

  if(!article) return <div className="p-6">Article not found</div>

  return (
    <article className="max-w-3xl mx-auto px-4 py-12 bg-white rounded card-shadow">
      <div className="text-xs text-gray-500">{article.date}</div>
      <h1 className="text-2xl font-bold mt-2">{article.title}</h1>
      <div className="mt-4 text-gray-700">{article.content}</div>
    </article>
  )
}
