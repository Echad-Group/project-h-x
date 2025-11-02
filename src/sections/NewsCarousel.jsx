import React from 'react'

const newsItems = [
  {
    title: 'Campaign launches nationwide youth agenda',
    date: 'Oct 2025',
    category: 'Policy',
    excerpt: 'A bold plan to support startups, apprenticeships, and internships, targeting 1 million youth opportunities.',
    image: '/src/assets/news/youth-agenda.jpg',
    url: '/news/youth-agenda'
  },
  {
    title: 'Townhall in Nakuru draws thousands',
    date: 'Sep 2025',
    category: 'Events',
    excerpt: 'Community dialogue focuses on jobs, local governance, and youth empowerment initiatives.',
    image: '/src/assets/news/nakuru-townhall.jpg',
    url: '/news/nakuru-townhall'
  },
  {
    title: 'Policy paper: Rural digital connectivity',
    date: 'Aug 2025',
    category: 'Technology',
    excerpt: 'Comprehensive plan for bringing high-speed internet to every county through innovative partnerships.',
    image: '/src/assets/news/digital-connectivity.jpg',
    url: '/news/rural-connectivity'
  },
  {
    title: 'New initiative for women entrepreneurs',
    date: 'Aug 2025',
    category: 'Economy',
    excerpt: 'Launch of dedicated fund and mentorship program for women-led businesses across Kenya.',
    image: '/src/assets/news/women-entrepreneurs.jpg',
    url: '/news/women-entrepreneurs'
  }
]

export default function NewsCarousel() {
  return (
    <section aria-label="Latest news" className="py-8">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-bold">Latest News & Updates</h2>
        <a href="/news" className="text-[var(--kenya-green)] hover:underline">View all news →</a>
      </div>
      
      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {newsItems.map((item, i) => (
          <article key={i} className="bg-white rounded-lg card-shadow overflow-hidden flex flex-col h-full transform transition hover:scale-105">
            <div className="aspect-video bg-gray-100 relative">
              {/* Image placeholder - replace with actual images */}
              <div className="absolute inset-0 bg-gradient-to-br from-[var(--kenya-green)]/20 to-transparent" />
            </div>
            
            <div className="p-4 flex-1 flex flex-col">
              <div className="flex items-center gap-2 mb-2">
                <span className="text-xs text-gray-500">{item.date}</span>
                <span className="text-xs px-2 py-1 bg-[var(--kenya-green)]/10 text-[var(--kenya-green)] rounded-full">
                  {item.category}
                </span>
              </div>
              
              <h3 className="font-semibold text-gray-900 flex-1">
                {item.title}
              </h3>
              
              <p className="mt-2 text-sm text-gray-600 line-clamp-2">
                {item.excerpt}
              </p>
              
              <a 
                href={item.url}
                className="mt-4 inline-flex items-center gap-1 text-[var(--kenya-green)] font-medium hover:gap-2 transition-all"
              >
                Read more
                <span>→</span>
              </a>
            </div>
          </article>
        ))}
      </div>
      
      {/* Social Media Feed Preview */}
      <div className="mt-12">
        <h3 className="text-xl font-semibold mb-4">Follow Our Campaign</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <a 
            href="https://twitter.com/newkenya" 
            target="_blank"
            rel="noopener noreferrer"
            className="p-4 bg-white rounded-lg card-shadow hover:shadow-lg transition"
          >
            <div className="flex items-center gap-2 text-[#1DA1F2]">
              <span className="font-bold">Twitter</span>
              <span>@newkenya</span>
            </div>
          </a>
          
          <a 
            href="https://facebook.com/newkenya" 
            target="_blank"
            rel="noopener noreferrer"
            className="p-4 bg-white rounded-lg card-shadow hover:shadow-lg transition"
          >
            <div className="flex items-center gap-2 text-[#4267B2]">
              <span className="font-bold">Facebook</span>
              <span>/newkenya</span>
            </div>
          </a>
          
          <a 
            href="https://instagram.com/newkenya" 
            target="_blank"
            rel="noopener noreferrer"
            className="p-4 bg-white rounded-lg card-shadow hover:shadow-lg transition"
          >
            <div className="flex items-center gap-2 text-[#E1306C]">
              <span className="font-bold">Instagram</span>
              <span>@newkenya</span>
            </div>
          </a>
        </div>
      </div>
    </section>
  )
}
