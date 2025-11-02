
import React, { useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

const PRODUCTS = [
  { id: 'shirt-01', titleKey: 'shop.product.shirt01.title', price: 1200, image: '/src/assets/shop/shirt-01.jpg' },
  { id: 'cap-01', titleKey: 'shop.product.cap01.title', price: 800, image: '/src/assets/shop/cap-01.jpg' }
]

export default function Shop(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(()=>{
    updateMeta({ title: `${t('shop.title')} - New Kenya Campaign`, description: t('shop.description'), url: '/shop' })
  },[t])

  return (
    <section className="max-w-6xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">{t('shop.title')}</h1>
      <p className="text-gray-600 mt-2">{t('shop.description')}</p>

      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-6">
        {PRODUCTS.map(p=> (
          <article key={p.id} className="fluent-card">
            <div className="h-40 bg-gray-100 rounded mb-3 flex items-center justify-center">Image</div>
            <div className="font-semibold">{t(p.titleKey)}</div>
            <div className="text-sm text-gray-500">KES {p.price}</div>
            <Link to={`/product/${p.id}`} className="mt-3 inline-block fluent-btn fluent-btn-ghost">{t('shop.product.view')}</Link>
          </article>
        ))}
      </div>
    </section>
  )
}
