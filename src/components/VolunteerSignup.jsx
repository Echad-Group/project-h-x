import React, { useState } from 'react';

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
      setError('Name and email are required.');
      return;
    }
    setSubmitted(true);
    if (onSubmit) onSubmit(form);
    // TODO: Integrate with backend or email service
  };

  if (submitted) {
    return (
      <div className="p-6 bg-green-50 rounded-lg text-center">
        <h3 className="text-lg font-semibold text-green-700">Thank you for signing up!</h3>
        <p className="mt-2 text-green-600">We appreciate your support. Our team will contact you soon.</p>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4 p-6 bg-white rounded-lg card-shadow max-w-md mx-auto">
      <h2 className="text-2xl font-bold mb-2">Volunteer with Us</h2>
      {error && <div className="text-red-600 text-sm">{error}</div>}
      <div>
        <label className="block text-sm font-medium">Name*</label>
        <input name="name" value={form.name} onChange={handleChange} required className="mt-1 w-full border rounded p-2" />
      </div>
      <div>
        <label className="block text-sm font-medium">Email*</label>
        <input name="email" type="email" value={form.email} onChange={handleChange} required className="mt-1 w-full border rounded p-2" />
      </div>
      <div>
        <label className="block text-sm font-medium">Phone</label>
        <input name="phone" type="tel" value={form.phone} onChange={handleChange} className="mt-1 w-full border rounded p-2" />
      </div>
      <div>
        <label className="block text-sm font-medium">Interests</label>
        <textarea name="interests" value={form.interests} onChange={handleChange} className="mt-1 w-full border rounded p-2" placeholder="How would you like to help?" />
      </div>
      <button type="submit" className="w-full bg-[var(--kenya-green)] text-white py-2 rounded font-semibold hover:bg-green-700">Sign Up</button>
    </form>
  );
}
