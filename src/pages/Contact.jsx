import React, { useEffect, useState } from 'react'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'
import { contactService } from '../services/contactService'

export default function Contact(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    updateMeta({
      title: 'Contact - New Kenya Campaign',
      description: 'Get in touch with the New Kenya campaign team for media, volunteering, or general inquiries.',
      url: '/contact'
    });
  }, []);

  async function handleSubmit(e){
    e.preventDefault();
    setError(null);
    setSuccess(false);
    setLoading(true);

    const data = Object.fromEntries(new FormData(e.target).entries());
    console.log('Contact form submitted:', data);

    try {
      await contactService.create({
        name: data.name,
        email: data.email,
        subject: data.subject || null,
        message: data.message
      });
      
      setSuccess(true);
      e.target.reset();
    } catch (err) {
      console.error('Contact form error:', err);
      
      if (err.response?.data?.errors) {
        const validationErrors = Object.values(err.response.data.errors).flat();
        setError(validationErrors.join(', '));
      } else if (err.code === 'ERR_NETWORK') {
        setError(t('contact.errors.network') || 'Network error. Please check if the backend is running.');
      } else {
        setError(t('contact.errors.general') || 'Failed to send message. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="max-w-3xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">{t('contact.title')}</h1>
      <p className="mt-3 text-gray-600">{t('contact.description')}</p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4 bg-white p-6 rounded-lg card-shadow">
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
            {error}
          </div>
        )}
        {success && (
          <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded">
            {t('contact.success') || 'Thank you! We will get back to you soon.'}
          </div>
        )}
        <div>
          <label className="block text-sm font-medium">{t('contact.form.name')}</label>
          <input name="name" required className="fluent-input mt-1" />
        </div>
        <div>
          <label className="block text-sm font-medium">{t('contact.form.email')}</label>
          <input name="email" type="email" required className="fluent-input mt-1" />
        </div>
        <div>
          <label className="block text-sm font-medium">{t('contact.form.subject')}</label>
          <input name="subject" className="fluent-input mt-1" />
        </div>
        <div>
          <label className="block text-sm font-medium">{t('contact.form.message')}</label>
          <textarea name="message" required className="fluent-input mt-1" rows={5} />
        </div>
        <button 
          type="submit" 
          disabled={loading}
          className="fluent-btn fluent-btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
        >
          {loading ? t('contact.form.sending') || 'Sending...' : t('contact.form.send')}
        </button>
      </form>
    </section>
  )
}
