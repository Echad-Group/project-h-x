import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'

export default function About(){
  const { updateMeta } = useMeta();
  
  useEffect(() => {
    updateMeta({
      title: 'About - New Kenya Campaign',
      description: 'Learn about our vision for a New Kenya and our track record of public service.',
      image: '/assets/og-image.svg',
      structuredData: {
        '@context': 'https://schema.org',
        '@type': 'Person',
        name: 'New Kenya Campaign Leader',
        description: 'A modern, dynamic campaign — focused on renewing institutions, investing in youth, and delivering inclusive growth.',
        url: 'https://newkenya.org/about',
        image: 'https://newkenya.org/assets/og-image.svg',
        jobTitle: 'Political Leader',
        worksFor: {
          '@type': 'Organization',
          name: 'New Kenya Campaign'
        }
      }
    });
  }, []);
  return (
    <section className="max-w-4xl mx-auto py-16 px-4">
      <h1 className="text-3xl font-extrabold brand-gradient">About the Campaign</h1>
      
      {/* Mission Statement */}
      <div className="mt-8">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">Mission Statement</h2>
        <p className="mt-4 text-lg text-gray-700">
          To build a unified, prosperous, and innovative Kenya through inclusive governance, economic empowerment, and sustainable development, ensuring every citizen has the opportunity to thrive in a modern, dynamic nation.
        </p>
      </div>

      {/* Core Values */}
      <div className="mt-12">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">Core Values</h2>
        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">Unity</h3>
            <p className="mt-2 text-gray-600">Fostering national cohesion and celebrating our diversity as our strength.</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">Progress</h3>
            <p className="mt-2 text-gray-600">Embracing innovation and sustainable development for future generations.</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">Justice</h3>
            <p className="mt-2 text-gray-600">Ensuring equal opportunities and fair treatment for all Kenyans.</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold text-[var(--kenya-red)]">Transparency</h3>
            <p className="mt-2 text-gray-600">Maintaining open, accountable leadership and governance.</p>
          </div>
        </div>
      </div>

      {/* Vision & Track Record */}
      <div className="mt-12">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">Vision & Leadership</h2>
        <p className="mt-4 text-lg text-gray-700">This is a modern, dynamic campaign — focused on renewing institutions, investing in youth, and delivering inclusive growth. The candidate brings public service experience, a clear plan for economic transformation, and a commitment to national unity.</p>
        
        <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold">Vision</h3>
            <p className="mt-2 text-gray-600">A New Kenya — united, prosperous and secure for every citizen. Where innovation drives growth and opportunity reaches every corner of our nation.</p>
          </div>
          <div className="p-6 bg-white card-shadow rounded-lg">
            <h3 className="font-semibold">Track Record</h3>
            <p className="mt-2 text-gray-600">Public leadership across sectors with a focus on results and transparency. Proven success in economic development and institutional reform.</p>
          </div>
        </div>
      </div>

      {/* Signature Statements */}
      <div className="mt-12 p-8 bg-gradient-to-r from-[var(--kenya-green)]/10 to-transparent rounded-lg">
        <h2 className="text-2xl font-bold text-[var(--kenya-green)]">Campaign Promise</h2>
        <blockquote className="mt-4 text-xl italic text-gray-700">
          "Together, we will build a New Kenya where unity is our strength, innovation drives our progress, and prosperity reaches every citizen. This is not just a campaign — it's a movement for transformative change."
        </blockquote>
      </div>
    </section>
  )
}
