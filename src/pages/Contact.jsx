import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

export default function Contact(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

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
      <h1 className="text-3xl font-bold">{t('contact.title')}</h1>
      <p className="mt-3 text-gray-600">{t('contact.description')}</p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4 bg-white p-6 rounded-lg card-shadow">
        <div>
          <label className="block text-sm font-medium">{t('contact.form.name')}</label>
          <input name="name" required className="fluent-input mt-1" />
        </div>
        <div>
          <label className="block text-sm font-medium">{t('contact.form.email')}</label>
          <input name="email" type="email" required className="fluent-input mt-1" />
        </div>
        <div>
          <label className="block text-sm font-medium">{t('contact.form.message')}</label>
          <textarea name="message" required className="fluent-input mt-1" rows={5} />
        </div>
        <button type="submit" className="fluent-btn fluent-btn-primary">{t('contact.form.send')}</button>
      </form>
    </section>
  )
}
