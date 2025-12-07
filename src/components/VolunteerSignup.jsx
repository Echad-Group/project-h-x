import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { volunteerService } from '../services/volunteerService';
import { useAuth } from '../contexts/AuthContext';

export default function VolunteerSignup({ onSubmit }) {
  const [form, setForm] = useState({
    name: '',
    email: '',
    phone: '',
    interests: ''
  });
  const [submitted, setSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [checkingStatus, setCheckingStatus] = useState(false);
  const [volunteerStatus, setVolunteerStatus] = useState(null);
  const { isAuthenticated, user } = useAuth();
  const { t } = useTranslation();

  // Check volunteer status if user is authenticated
  useEffect(() => {
    const checkVolunteerStatus = async () => {
      if (isAuthenticated) {
        setCheckingStatus(true);
        try {
          const status = await volunteerService.checkStatus();
          setVolunteerStatus(status);
          
          // Pre-fill form with user data if not already a volunteer
          if (!status.isVolunteer && user) {
            setForm(prev => ({
              ...prev,
              name: user.firstName && user.lastName ? `${user.firstName} ${user.lastName}` : '',
              email: user.email || ''
            }));
          }
        } catch (err) {
          console.error('Error checking volunteer status:', err);
        } finally {
          setCheckingStatus(false);
        }
      }
    };

    checkVolunteerStatus();
  }, [isAuthenticated, user]);

  const handleChange = e => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    if (!form.name || !form.email) {
      setError(t('volunteer.errors.required'));
      setLoading(false);
      return;
    }

    try {
      const response = await volunteerService.create(form);
      
      // Check if it's a linking response (existing volunteer account linked to user)
      if (response.message && response.message.includes('linked')) {
        // Update volunteer status to show the linked account
        if (isAuthenticated) {
          const status = await volunteerService.checkStatus();
          setVolunteerStatus(status);
        }
      } else {
        setSubmitted(true);
        
        // Update volunteer status after successful signup
        if (isAuthenticated) {
          const status = await volunteerService.checkStatus();
          setVolunteerStatus(status);
        }
      }
      
      if (onSubmit) onSubmit(form);
    } catch (err) {
      console.error('Volunteer signup error:', err);
      
      // Handle specific error cases
      if (err.response?.status === 409) {
        setError(t('volunteer.errors.duplicate') || 'You are already registered as a volunteer.');
      } else if (err.response?.data?.errors) {
        // Validation errors from backend
        const validationErrors = Object.values(err.response.data.errors).flat();
        setError(validationErrors.join(', '));
      } else if (err.code === 'ERR_NETWORK') {
        setError(t('volunteer.errors.network') || 'Network error. Please check if the backend is running.');
      } else {
        setError(t('volunteer.errors.general') || 'Failed to submit. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  // Show loading state while checking volunteer status
  if (checkingStatus) {
    return (
      <div className="fluent-card text-center p-6">
        <p className="text-gray-600">Checking volunteer status...</p>
      </div>
    );
  }

  // Show different content if user is already a volunteer
  if (isAuthenticated && volunteerStatus?.isVolunteer) {
    return (
      <div className="fluent-card p-6 bg-gradient-to-br from-green-50 to-emerald-50">
        <div className="text-center">
          <div className="inline-flex items-center justify-center w-16 h-16 bg-green-100 rounded-full mb-4">
            <svg className="w-8 h-8 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
            </svg>
          </div>
          <h3 className="text-2xl font-bold text-gray-900 mb-2">
            {t('volunteer.alreadyRegistered.title') || 'You\'re Already a Volunteer!'}
          </h3>
          <p className="text-gray-700 mb-4">
            {t('volunteer.alreadyRegistered.message') || 'Thank you for being part of the New Kenya movement!'}
          </p>
          <div className="bg-white rounded-lg p-4 text-left space-y-2">
            <div className="flex items-start">
              <span className="font-medium text-gray-600 w-24">Name:</span>
              <span className="text-gray-900">{volunteerStatus.volunteer.name}</span>
            </div>
            <div className="flex items-start">
              <span className="font-medium text-gray-600 w-24">Email:</span>
              <span className="text-gray-900">{volunteerStatus.volunteer.email}</span>
            </div>
            {volunteerStatus.volunteer.phone && (
              <div className="flex items-start">
                <span className="font-medium text-gray-600 w-24">Phone:</span>
                <span className="text-gray-900">{volunteerStatus.volunteer.phone}</span>
              </div>
            )}
            {volunteerStatus.volunteer.interests && (
              <div className="flex items-start">
                <span className="font-medium text-gray-600 w-24">Interests:</span>
                <span className="text-gray-900">{volunteerStatus.volunteer.interests}</span>
              </div>
            )}
            <div className="flex items-start">
              <span className="font-medium text-gray-600 w-24">Joined:</span>
              <span className="text-gray-900">
                {new Date(volunteerStatus.volunteer.createdAt).toLocaleDateString()}
              </span>
            </div>
          </div>
          <p className="text-sm text-gray-600 mt-4">
            {t('volunteer.alreadyRegistered.stay_tuned') || 'Stay tuned for volunteer opportunities and updates!'}
          </p>
        </div>
      </div>
    );
  }

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
      <button 
        type="submit" 
        disabled={loading}
        className="fluent-btn fluent-btn-primary w-full disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {loading ? t('volunteer.form.submitting') || 'Submitting...' : t('volunteer.form.submit')}
      </button>
    </form>
  );
}

