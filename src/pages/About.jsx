import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

export default function About(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();
  
  useEffect(() => {
    updateMeta({
      title: 'About - New Kenya Campaign',
      description: 'Learn about our vision for a New Kenya and our track record of public service.',
      image: '/assets/og-image.svg',
      structuredData: {
        '@context': 'https://schema.org',
        '@type': 'Person',
        name: 'New Kenya Campaign Leader',
        description: 'A modern, dynamic campaign — focused on renewing institutions, investing in youth, and delivering inclusive growth.',
        url: 'https://newkenya.org/about',
        image: 'https://newkenya.org/assets/og-image.svg',
        jobTitle: 'Political Leader',
        worksFor: {
          '@type': 'Organization',
          name: 'New Kenya Campaign'
        }
      }
    });
  }, []);
  return (
    <section className="max-w-4xl mx-auto py-16 px-4">
      <h1 className="text-3xl font-extrabold brand-gradient">{t('about.title')}</h1>
      
      {/* Mission Statement */}
      <div className="mt-8">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">{t('about.missionHeading')}</h2>
        <p className="mt-4 text-lg text-gray-700">{t('about.missionText')}</p>
      </div>

      {/* Core Values */}
      <div className="mt-12">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">{t('about.coreValuesHeading')}</h2>
        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">{t('about.values.unity.title')}</h3>
            <p className="mt-2 text-gray-600">{t('about.values.unity.desc')}</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">{t('about.values.progress.title')}</h3>
            <p className="mt-2 text-gray-600">{t('about.values.progress.desc')}</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">{t('about.values.justice.title')}</h3>
            <p className="mt-2 text-gray-600">{t('about.values.justice.desc')}</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">{t('about.values.transparency.title')}</h3>
            <p className="mt-2 text-gray-600">{t('about.values.transparency.desc')}</p>
          </div>
        </div>
      </div>

      {/* Vision & Track Record */}
      <div className="mt-12">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">{t('about.visionHeading')}</h2>
        <p className="mt-4 text-lg text-gray-700">{t('about.visionText')}</p>
        
        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold">{t('about.vision.visionTitle')}</h3>
            <p className="mt-2 text-gray-600">{t('about.vision.visionText')}</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold">{t('about.vision.trackRecordTitle')}</h3>
            <p className="mt-2 text-gray-600">{t('about.vision.trackRecordText')}</p>
          </div>
        </div>
      </div>

      {/* Signature Statements */}
      <div className="mt-12 p-8 bg-gradient-to-r from-[var(--kenya-green)]/10 to-transparent rounded-lg">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">{t('about.promise.heading')}</h2>
        <blockquote className="mt-4 text-xl italic text-gray-700">{t('about.promise.quote')}</blockquote>
      </div>
    </section>
  )
}
