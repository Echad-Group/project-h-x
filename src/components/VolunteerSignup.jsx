import React, { useState } from 'react';
import { useTranslation } from 'react-i18next'

export default function VolunteerSignup({ onSubmit }) {
  const [form, setForm] = useState({
    name: '',
    email: '',
    phone: '',
    interests: ''
  });
  const [submitted, setSubmitted] = useState(false);
  const [error, setError] = useState(null);

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setError(null);
    if (!form.name || !form.email) {
      setError(t('volunteer.errors.required'));
      return;
    }
    setSubmitted(true);
    if (onSubmit) onSubmit(form);
    // TODO: Integrate with backend or email service
  };

  const { t } = useTranslation();

  if (submitted) {
    return (
      <div className="fluent-card text-center bg-green-50">
        <h3 className="text-lg font-semibold text-green-700">{t('volunteer.success.title')}</h3>
        <p className="mt-2 text-green-600">{t('volunteer.success.desc')}</p>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4 p-6 bg-white rounded-lg card-shadow max-w-md mx-auto">
      <h2 className="text-2xl font-bold mb-2">{t('volunteer.heading')}</h2>
      {error && <div className="text-red-600 text-sm">{error}</div>}
      <div>
        <label className="block text-sm font-medium">{t('volunteer.form.name')}</label>
        <input name="name" value={form.name} onChange={handleChange} required className="fluent-input mt-1" />
      </div>
      <div>
        <label className="block text-sm font-medium">{t('volunteer.form.email')}</label>
        <input name="email" type="email" value={form.email} onChange={handleChange} required className="fluent-input mt-1" />
      </div>
      <div>
        <label className="block text-sm font-medium">{t('volunteer.form.phone')}</label>
        <input name="phone" type="tel" value={form.phone} onChange={handleChange} className="fluent-input mt-1" />
      </div>
      <div>
        <label className="block text-sm font-medium">{t('volunteer.form.interests')}</label>
        <textarea name="interests" value={form.interests} onChange={handleChange} className="fluent-input mt-1" placeholder={t('volunteer.form.interestsPlaceholder')} />
      </div>
      <button type="submit" className="fluent-btn fluent-btn-primary w-full">{t('volunteer.form.submit')}</button>
    </form>
  );
}

