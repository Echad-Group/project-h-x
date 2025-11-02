
import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useCart } from '../components/CartContext'
import { useTranslation } from 'react-i18next'

const PRODUCTS = {
  'shirt-01': { id: 'shirt-01', titleKey: 'shop.product.shirt01.title', price: 1200, descKey: 'shop.product.shirt01.desc' },
  'cap-01': { id: 'cap-01', titleKey: 'shop.product.cap01.title', price: 800, descKey: 'shop.product.cap01.desc' }
}

export default function Product(){
  const { id } = useParams();
  const product = PRODUCTS[id];
  const { updateMeta } = useMeta();
  const { addToCart } = useCart();
  const { t } = useTranslation();

  useEffect(()=>{
    if(product) updateMeta({ title: t(product.titleKey), description: t(product.descKey), url: `/product/${id}` })
  },[id, t, product])

  if(!product) return <div className="p-6">{t('shop.product.notFound')}</div>

  return (
    <section className="max-w-3xl mx-auto px-4 py-12 bg-white rounded card-shadow">
      <h1 className="text-2xl font-bold">{t(product.titleKey)}</h1>
      <p className="mt-2 text-gray-600">{t(product.descKey)}</p>
      <div className="mt-4 text-lg font-semibold">KES {product.price}</div>
      <div className="mt-4">
        <button onClick={()=> addToCart(product)} className="fluent-btn fluent-btn-primary">{t('shop.product.addToCart')}</button>
      </div>
    </section>
  )
}
