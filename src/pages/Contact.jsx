import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'

export default function Contact(){
  const { updateMeta } = useMeta();

  useEffect(() => {
    updateMeta({
      title: 'Contact - New Kenya Campaign',
      description: 'Get in touch with the New Kenya campaign team for media, volunteering, or general inquiries.',
      url: '/contact'
    });
  }, []);

  function handleSubmit(e){
    e.preventDefault();
    const data = Object.fromEntries(new FormData(e.target).entries());
    console.log('Contact form submitted:', data);
    alert('Thank you — we will get back to you soon. (This is a demo)');
    e.target.reset();
  }

  return (
    <section className="max-w-3xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Contact</h1>
      <p className="mt-3 text-gray-600">Questions about the campaign, press inquiries, or partnership requests — reach out to us.</p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4 bg-white p-6 rounded-lg card-shadow">
        <div>
          <label className="block text-sm font-medium">Name</label>
          <input name="name" required className="mt-1 w-full border rounded p-2" />
        </div>
        <div>
          <label className="block text-sm font-medium">Email</label>
          <input name="email" type="email" required className="mt-1 w-full border rounded p-2" />
        </div>
        <div>
          <label className="block text-sm font-medium">Message</label>
          <textarea name="message" required className="mt-1 w-full border rounded p-2" rows={5} />
        </div>
        <button type="submit" className="px-4 py-2 bg-[var(--kenya-green)] text-white rounded">Send Message</button>
      </form>
    </section>
  )
}
