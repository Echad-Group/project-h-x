import React from 'react'

const IssueCard = ({title, desc}) => (
  <div className="p-6 bg-white rounded-lg card-shadow">
    <h4 className="font-semibold text-[var(--kenya-green)]">{title}</h4>
    <p className="mt-2 text-sm text-gray-600">{desc}</p>
  </div>
)

export default function Issues(){
  const issues = [
    {title: 'Jobs & Enterprise', desc: 'Create pathways for youth employment and support SMEs.'},
    {title: 'Healthcare', desc: 'Universal primary healthcare and stronger clinics.'},
    {title: 'Education', desc: 'Invest in technical training, scholarships and digital learning.'},
    {title: 'Security & Justice', desc: 'Community safety and accountable institutions.'}
  ]

  return (
    <section className="max-w-7xl mx-auto px-4 py-12 grid grid-cols-1 md:grid-cols-2 gap-6">
      {issues.map(i=> <IssueCard key={i.title} {...i} />)}
    </section>
  )
}
