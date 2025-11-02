import React, { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'

const PRODUCTS = [
  { id: 'shirt-01', title: 'New Kenya T-Shirt', price: 1200, image: '/src/assets/shop/shirt-01.jpg' },
  { id: 'cap-01', title: 'Campaign Cap', price: 800, image: '/src/assets/shop/cap-01.jpg' }
]

export default function Shop(){
  const { updateMeta } = useMeta();

  useEffect(()=>{
    updateMeta({ title: 'Shop - New Kenya Campaign', description: 'Official campaign merchandise (demo).', url: '/shop' })
  },[])

  return (
    <section className="max-w-6xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Shop</h1>
      <p className="text-gray-600 mt-2">Official merchandise — demo store (no real payments).</p>

      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
        {PRODUCTS.map(p=> (
          <article key={p.id} className="bg-white p-4 rounded card-shadow">
            <div className="h-40 bg-gray-100 rounded mb-3 flex items-center justify-center">Image</div>
            <div className="font-semibold">{p.title}</div>
            <div className="text-sm text-gray-500">KES {p.price}</div>
            <Link to={`/product/${p.id}`} className="mt-3 inline-block text-[var(--kenya-green)]">View →</Link>
          </article>
        ))}
      </div>
    </section>
  )
}
