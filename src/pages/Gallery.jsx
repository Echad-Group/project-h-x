import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'

export default function Gallery(){
  const { updateMeta } = useMeta();

  useEffect(() => {
    updateMeta({ title: 'Gallery - New Kenya Campaign', description: 'Photo gallery of campaign events and milestones.', url: '/gallery' });
  }, []);

  const images = new Array(8).fill(0).map((_,i) => `/src/assets/gallery/${i+1}.jpg`);

  return (
    <section className="max-w-6xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Gallery</h1>
      <div className="mt-6 grid grid-cols-2 md:grid-cols-4 gap-4">
        {images.map((src, i) => (
          <div key={i} className="h-40 bg-gray-100 rounded overflow-hidden flex items-center justify-center">
            <span className="text-sm text-gray-500">Image {i+1}</span>
          </div>
        ))}
      </div>
    </section>
  )
}
