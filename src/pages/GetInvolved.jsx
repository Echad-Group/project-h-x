import React, { useEffect } from 'react';
import VolunteerSignup from '../components/VolunteerSignup';
import { useMeta } from '../components/MetaTags';
import { useTranslation } from 'react-i18next';

export default function GetInvolved() {
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    updateMeta({
      title: 'Get Involved - New Kenya Campaign',
      description: 'Join the movement for a New Kenya. Volunteer opportunities, community organizing, and ways to support the campaign.',
      image: '/assets/og-image.svg',
      url: '/get-involved',
      breadcrumbs: [
        { name: 'Home', url: '/' },
        { name: 'Get Involved', url: '/get-involved' }
      ],
      structuredData: {
        '@context': 'https://schema.org',
        '@type': 'VolunteerAction',
        'name': 'Volunteer for New Kenya Campaign',
        'description': 'Join our campaign as a volunteer to help build a better Kenya.',
        'provider': {
          '@type': 'Organization',
          'name': 'New Kenya Campaign'
        }
      }
    });
  }, []);
  return (
    <section id="get-involved" className="max-w-3xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">{t('getInvolved.title')}</h1>
      <p className="mt-3 text-gray-600">{t('getInvolved.description')}</p>
      <div className="mt-6">
        <VolunteerSignup />
      </div>
    </section>
  );
}
