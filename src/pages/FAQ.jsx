import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'

const faqs = [
  { q: 'How can I volunteer?', a: 'Visit the Get Involved page and fill the volunteer signup form.' },
  { q: 'Is this site secure for donations?', a: 'Donations in this demo are mocked. Integrate a payment gateway for production.' },
  { q: 'How do I attend an event?', a: 'Go to the Events page and RSVP for an event.' }
];

export default function FAQ(){
  const { updateMeta } = useMeta();

  useEffect(() => {
    updateMeta({
      title: 'FAQ - New Kenya Campaign',
      description: 'Frequently asked questions about the campaign, volunteering, and donations.',
      url: '/faq'
    });
  }, []);

  return (
    <section className="max-w-4xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">FAQ</h1>
      <div className="mt-6 space-y-4">
        {faqs.map((f, i) => (
          <details key={i} className="bg-white p-4 rounded card-shadow">
            <summary className="font-medium">{f.q}</summary>
            <p className="mt-2 text-gray-600">{f.a}</p>
          </details>
        ))}
      </div>
    </section>
  )
}
