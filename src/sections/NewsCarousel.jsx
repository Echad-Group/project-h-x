import React from 'react'

const sample = [
  {title: 'Campaign launches nationwide youth agenda', date: 'Oct 2025', excerpt: 'A bold plan to support startups, apprenticeships, and internships.'},
  {title: 'Townhall in Nakuru draws thousands', date: 'Sep 2025', excerpt: 'Community dialogue on jobs and local governance.'},
  {title: 'Policy paper: Rural digital connectivity', date: 'Aug 2025', excerpt: 'Bringing high-speed internet to every county.'}
]

export default function NewsCarousel(){
  return (
    <section aria-label="Latest news">
      <h2 className="text-2xl font-bold">Latest News</h2>
      <div className="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
        {sample.map((s, i)=> (
          <article key={i} className="p-4 bg-white rounded-lg card-shadow">
            <div className="text-xs text-gray-500">{s.date}</div>
            <h3 className="font-semibold mt-1">{s.title}</h3>
            <p className="mt-2 text-sm text-gray-600">{s.excerpt}</p>
            <a className="mt-3 inline-block text-[var(--kenya-green)] font-medium" href="#">Read more →</a>
          </article>
        ))}
      </div>
    </section>
  )
}
