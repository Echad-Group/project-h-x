import React from 'react'

const stories = [
  { title: 'Community Clinic Upgrade', excerpt: 'Upgraded three county clinics with new equipment and trained staff.' },
  { title: 'Youth Startup Grants', excerpt: 'Distributed seed grants to 120 youth-led startups.' }
]

export default function SuccessStories(){
  return (
    <section className="mt-12">
      <h2 className="text-2xl font-bold">Success Stories</h2>
      <div className="mt-4 grid grid-cols-1 md:grid-cols-2 gap-4">
        {stories.map((s, i) => (
          <div key={i} className="p-4 bg-white rounded card-shadow">
            <h3 className="font-semibold">{s.title}</h3>
            <p className="mt-2 text-sm text-gray-600">{s.excerpt}</p>
          </div>
        ))}
      </div>
    </section>
  )
}
