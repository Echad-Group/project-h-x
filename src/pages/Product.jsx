import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useCart } from '../components/CartContext'

const PRODUCTS = {
  'shirt-01': { id: 'shirt-01', title: 'New Kenya T-Shirt', price: 1200, desc: 'Comfortable cotton tee.' },
  'cap-01': { id: 'cap-01', title: 'Campaign Cap', price: 800, desc: 'Embroidered campaign cap.' }
}

export default function Product(){
  const { id } = useParams();
  const product = PRODUCTS[id];
  const { updateMeta } = useMeta();
  const { addToCart } = useCart();

  useEffect(()=>{
    if(product) updateMeta({ title: product.title, description: product.desc, url: `/product/${id}` })
  },[id])

  if(!product) return <div className="p-6">Product not found</div>

  return (
    <section className="max-w-3xl mx-auto px-4 py-12 bg-white rounded card-shadow">
      <h1 className="text-2xl font-bold">{product.title}</h1>
      <p className="mt-2 text-gray-600">{product.desc}</p>
      <div className="mt-4 text-lg font-semibold">KES {product.price}</div>
      <div className="mt-4">
        <button onClick={()=> addToCart(product)} className="fluent-btn fluent-btn-primary">Add to cart</button>
      </div>
    </section>
  )
}
