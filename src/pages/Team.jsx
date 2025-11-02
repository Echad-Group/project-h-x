import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'

const team = [
  { name: 'Campaign Leader', title: 'Leader', bio: 'Experienced public servant focused on inclusive growth.' },
  { name: 'Campaign Manager', title: 'Manager', bio: 'Leading operations and field teams across the country.' }
]

export default function Team(){
  const { updateMeta } = useMeta();

  useEffect(() => {
    updateMeta({
      title: 'Team - New Kenya Campaign',
      description: 'Meet the campaign team and leadership.',
      url: '/team'
    });
  }, []);

  return (
    <section className="max-w-6xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Team</h1>
      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
        {team.map((m, i) => (
          <div key={i} className="p-4 bg-white rounded card-shadow">
            <div className="font-semibold">{m.name}</div>
            <div className="text-xs text-gray-500">{m.title}</div>
            <p className="mt-2 text-sm text-gray-600">{m.bio}</p>
          </div>
        ))}
      </div>
    </section>
  )
}
