import React from 'react'
import { useCart } from './CartContext'
import { useTranslation } from 'react-i18next'

export default function Cart(){
  const { items, removeFromCart, clearCart } = useCart();
  const { t } = useTranslation();

  const total = items.reduce((s,i) => s + (i.price || 0) * i.qty, 0);

  if(items.length === 0) return (
    <div className="fixed bottom-4 left-4 z-40">
      <div className="fluent-card p-3 rounded">{t('cart.empty')}</div>
    </div>
  )

  return (
    <div className="fixed bottom-4 left-4 z-40 w-80 fluent-card p-4">
      <div className="flex justify-between items-center">
        <div className="font-semibold">{t('cart.title')}</div>
        <button onClick={clearCart} className="text-xs text-red-500 fluent-btn fluent-btn-ghost">{t('cart.clear')}</button>
      </div>
      <div className="mt-3 space-y-2">
        {items.map(it => (
          <div key={it.id} className="flex justify-between items-center">
            <div>
              <div className="font-medium">{it.title}</div>
              <div className="text-xs text-gray-500">{it.qty} x KES {it.price}</div>
            </div>
            <button onClick={()=> removeFromCart(it.id)} className="text-sm text-red-500 fluent-btn fluent-btn-ghost">{t('cart.remove')}</button>
          </div>
        ))}
      </div>
      <div className="mt-3 flex justify-between items-center">
        <div className="font-semibold">{t('cart.total')}</div>
        <div className="font-bold">KES {total}</div>
      </div>
      <div className="mt-3">
        <button onClick={()=> alert(t('cart.checkoutAlert'))} className="fluent-btn fluent-btn-action w-full">{t('cart.checkout')}</button>
      </div>
    </div>
  )
}
