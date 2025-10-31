import React from 'react'

const items = [
  {title: 'Jobs & Growth', desc: 'Programs to unlock private sector hiring and support SMEs.'},
  {title: 'Healthcare', desc: 'Invest in clinics, preventive care and health workers.'},
  {title: 'Education', desc: 'Skills, technical training and digital classrooms.'}
]

export default function IssuesPreview(){
  return (
    <section aria-label="Key issues">
      <h2 className="text-2xl font-bold">Key Issues</h2>
      <div className="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
        {items.map(i=> (
          <div key={i.title} className="p-5 bg-white rounded-lg card-shadow">
            <h3 className="text-lg font-semibold text-[var(--kenya-red)]">{i.title}</h3>
            <p className="mt-2 text-sm text-gray-600">{i.desc}</p>
            <a className="mt-3 inline-block text-[var(--kenya-green)] font-medium" href="/issues">Learn more</a>
          </div>
        ))}
      </div>
    </section>
  )
}
