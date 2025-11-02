
import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

const team = [
  { nameKey: 'team.members.leader.name', titleKey: 'team.members.leader.title', bioKey: 'team.members.leader.bio' },
  { nameKey: 'team.members.manager.name', titleKey: 'team.members.manager.title', bioKey: 'team.members.manager.bio' }
]

export default function Team(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    updateMeta({
      title: `${t('team.title')} - New Kenya Campaign`,
      description: t('team.description'),
      url: '/team'
    });
  }, [t]);

  return (
    <section className="max-w-6xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">{t('team.title')}</h1>
      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
        {team.map((m, i) => (
          <div key={i} className="p-4 bg-white rounded card-shadow">
            <div className="font-semibold">{t(m.nameKey)}</div>
            <div className="text-xs text-gray-500">{t(m.titleKey)}</div>
            <p className="mt-2 text-sm text-gray-600">{t(m.bioKey)}</p>
          </div>
        ))}
      </div>
    </section>
  )
}
