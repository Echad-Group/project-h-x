import React, {useState} from 'react'

export default function GetInvolved(){
  const [form, setForm] = useState({name:'', email:'', phone:''})
  const [sent, setSent] = useState(false)

  function submit(e){
    e.preventDefault()
    // client-side demo only
    setSent(true)
  }

  return (
    <section id="get-involved" className="max-w-3xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Get Involved</h1>
      <p className="mt-3 text-gray-600">Volunteer, host an event, or join a local chapter.</p>

      <form onSubmit={submit} className="mt-6 bg-white p-6 rounded-lg card-shadow">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <input required placeholder="Full name" value={form.name} onChange={e=>setForm({...form, name: e.target.value})} className="px-3 py-2 border rounded-md" />
          <input required placeholder="Email" value={form.email} onChange={e=>setForm({...form, email: e.target.value})} className="px-3 py-2 border rounded-md" />
          <input placeholder="Phone" value={form.phone} onChange={e=>setForm({...form, phone: e.target.value})} className="px-3 py-2 border rounded-md" />
        </div>
        <div className="mt-4 flex items-center gap-3">
          <button className="px-5 py-2 bg-[var(--kenya-red)] text-white rounded-md">Sign up</button>
          {sent && <div className="text-sm text-green-600">Thanks — we received your interest.</div>}
        </div>
      </form>
    </section>
  )
}
