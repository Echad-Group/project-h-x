import React from 'react'
import { useTranslation } from 'react-i18next'

export default function ShareButtons({ title, url }){
  const shareData = { title, text: title, url };
  const { t } = useTranslation();

  async function handleShare(){
    if(navigator.share){
      try{ await navigator.share(shareData); }catch(e){ console.warn('Share cancelled', e); }
      return;
    }
    // Fallback - open native share links
    const twitter = `https://twitter.com/intent/tweet?text=${encodeURIComponent(title)}&url=${encodeURIComponent(url)}`;
    window.open(twitter, '_blank');
  }

  return (
    <div className="flex items-center gap-2">
      <button onClick={handleShare} className="px-3 py-1 bg-gray-100 rounded">{t('share.share')}</button>
      <a className="px-3 py-1 bg-blue-500 text-white rounded" href={`https://twitter.com/intent/tweet?text=${encodeURIComponent(title)}&url=${encodeURIComponent(url)}`} target="_blank" rel="noreferrer">{t('share.twitter')}</a>
    </div>
  )
}
