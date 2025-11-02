import React from 'react'
import { useCart } from './CartContext'

export default function Cart(){
  const { items, removeFromCart, clearCart } = useCart();

  const total = items.reduce((s,i) => s + (i.price || 0) * i.qty, 0);

  if(items.length === 0) return (
    <div className="fixed bottom-4 left-4 z-40">
      <div className="bg-white p-3 rounded shadow">Cart is empty</div>
    </div>
  )

  return (
    <div className="fixed bottom-4 left-4 z-40 w-80 bg-white rounded-lg card-shadow p-4">
      <div className="flex justify-between items-center">
        <div className="font-semibold">Cart</div>
        <button onClick={clearCart} className="text-xs text-red-500">Clear</button>
      </div>
      <div className="mt-3 space-y-2">
        {items.map(it => (
          <div key={it.id} className="flex justify-between items-center">
            <div>
              <div className="font-medium">{it.title}</div>
              <div className="text-xs text-gray-500">{it.qty} x KES {it.price}</div>
            </div>
            <button onClick={()=> removeFromCart(it.id)} className="text-sm text-red-500">Remove</button>
          </div>
        ))}
      </div>
      <div className="mt-3 flex justify-between items-center">
        <div className="font-semibold">Total</div>
        <div className="font-bold">KES {total}</div>
      </div>
      <div className="mt-3">
        <button onClick={()=> alert('Checkout demo - implement payment gateway')} className="w-full px-4 py-2 bg-[var(--kenya-red)] text-white rounded">Checkout</button>
      </div>
    </div>
  )
}
