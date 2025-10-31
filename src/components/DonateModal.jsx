import React, {useState} from 'react'

export default function DonateModal(){
  const [open, setOpen] = useState(false)
  const [amount, setAmount] = useState('')

  return (
    <div>
      <button onClick={()=>setOpen(true)} className="px-4 py-2 bg-[var(--kenya-red)] text-white rounded-md">Donate</button>

      {open && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md">
            <h3 className="text-xl font-semibold">Support New Kenya</h3>
            <p className="mt-2 text-sm text-gray-600">Your contribution helps mobilize volunteers and reach voters.</p>
            <div className="mt-4">
              <input type="number" value={amount} onChange={e=>setAmount(e.target.value)} placeholder="Amount (KES)" className="w-full px-3 py-2 border rounded-md" />
              <div className="mt-4 flex justify-end gap-2">
                <button onClick={()=>setOpen(false)} className="px-4 py-2">Cancel</button>
                <button onClick={()=>{alert('Demo: donate ' + amount); setOpen(false)}} className="px-4 py-2 bg-[var(--kenya-green)] text-white rounded-md">Donate</button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
