import React, {useState} from 'react'
import Hero from '../sections/Hero'
import NewsCarousel from '../sections/NewsCarousel'
import IssuesPreview from '../sections/IssuesPreview'

export default function Home(){
  return (
    <div>
      <Hero />
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 space-y-16 py-12">
        <NewsCarousel />
        <IssuesPreview />
      </main>
    </div>
  )
}
