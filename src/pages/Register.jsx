import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useTranslation } from 'react-i18next';

export default function Register() {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // Validate passwords match
    if (formData.password !== formData.confirmPassword) {
      setError(t('auth.register.errors.passwordMismatch') || 'Passwords do not match');
      return;
    }

    // Validate password length
    if (formData.password.length < 6) {
      setError(t('auth.register.errors.passwordLength') || 'Password must be at least 6 characters');
      return;
    }

    setLoading(true);

    try {
      await register({
        email: formData.email,
        password: formData.password,
        firstName: formData.firstName,
        lastName: formData.lastName
      });
      navigate('/');
    } catch (err) {
      console.error('Registration error:', err);
      if (err.response?.data?.errors) {
        const errors = err.response.data.errors;
        if (Array.isArray(errors)) {
          setError(errors.join(', '));
        } else {
          setError(Object.values(errors).flat().join(', '));
        }
      } else if (err.code === 'ERR_NETWORK') {
        setError(t('auth.errors.network') || 'Network error. Please check if the backend is running.');
      } else {
        setError(t('auth.errors.general') || 'Registration failed. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4">
      <div className="max-w-md w-full">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">{t('auth.register.title') || 'Create Account'}</h1>
          <p className="mt-2 text-gray-600">{t('auth.register.subtitle') || 'Join the New Kenya movement'}</p>
        </div>

        <form onSubmit={handleSubmit} className="bg-white rounded-lg card-shadow p-8 space-y-6">
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded text-sm">
              {error}
            </div>
          )}

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                {t('auth.register.firstName') || 'First Name'}
              </label>
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                className="fluent-input"
                placeholder="John"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                {t('auth.register.lastName') || 'Last Name'}
              </label>
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                className="fluent-input"
                placeholder="Doe"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('auth.register.email') || 'Email'}
            </label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              className="fluent-input"
              placeholder="you@example.com"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('auth.register.password') || 'Password'}
            </label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required
              className="fluent-input"
              placeholder="••••••••"
            />
            <p className="text-xs text-gray-500 mt-1">
              {t('auth.register.passwordHint') || 'Must be at least 6 characters'}
            </p>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('auth.register.confirmPassword') || 'Confirm Password'}
            </label>
            <input
              type="password"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              className="fluent-input"
              placeholder="••••••••"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="fluent-btn fluent-btn-primary w-full disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loading ? (t('auth.register.creating') || 'Creating account...') : (t('auth.register.submit') || 'Create Account')}
          </button>

          <div className="text-center text-sm text-gray-600">
            {t('auth.register.haveAccount') || 'Already have an account?'}{' '}
            <Link to="/login" className="text-[var(--kenya-green)] hover:underline font-medium">
              {t('auth.register.signIn') || 'Sign in'}
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}
