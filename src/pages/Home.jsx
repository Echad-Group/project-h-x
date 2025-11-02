import React, { useState, useEffect } from 'react'
import Hero from '../sections/Hero'
import NewsCarousel from '../sections/NewsCarousel'
import IssuesPreview from '../sections/IssuesPreview'
import { useMeta } from '../components/MetaTags'

export default function Home({onOpenDonate}) {
  const { updateMeta } = useMeta();
  
  useEffect(() => {
    updateMeta({
      title: 'New Kenya Campaign - A Vision for Unity and Progress',
      description: 'Join us in building a New Kenya that invests in youth, strengthens communities, and modernizes institutions. Together for unity, jobs, and opportunity.',
      image: '/assets/og-image.svg',
      url: '/',
      structuredData: {
        '@context': 'https://schema.org',
        '@type': 'Organization',
        'name': 'New Kenya Campaign',
        'url': 'https://newkenya.org/',
        'logo': 'https://newkenya.org/assets/app-icon.svg',
        'sameAs': [
          'https://twitter.com/newkenya',
          'https://facebook.com/newkenya',
          'https://instagram.com/newkenya'
        ],
        'description': 'A modern vision for unity, jobs, and opportunity in Kenya'
      }
    });
  }, []);
  return (
    <div>
      <Hero onOpenDonate={onOpenDonate} />
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 space-y-16 py-12">
        <NewsCarousel />
        <IssuesPreview />
      </main>
    </div>
  )
}
