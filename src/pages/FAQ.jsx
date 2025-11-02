import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

const faqKeys = ['volunteer', 'donations', 'events'];

export default function FAQ(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    updateMeta({
      title: 'FAQ - New Kenya Campaign',
      description: 'Frequently asked questions about the campaign, volunteering, and donations.',
      url: '/faq'
    });
  }, []);

  return (
    <section className="max-w-4xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">{t('faq.title')}</h1>
      <div className="mt-6 space-y-4">
        {faqKeys.map((key) => (
          <details key={key} className="bg-white p-4 rounded card-shadow">
            <summary className="font-medium">{t(`faq.items.${key}.q`)}</summary>
            <p className="mt-2 text-gray-600">{t(`faq.items.${key}.a`)}</p>
          </details>
        ))}
      </div>
    </section>
  )
}
