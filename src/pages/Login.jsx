import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useTranslation } from 'react-i18next';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await login({ email, password });
      navigate('/');
    } catch (err) {
      console.error('Login error:', err);
      if (err.response?.status === 401) {
        setError(t('auth.errors.invalidCredentials') || 'Invalid email or password');
      } else if (err.code === 'ERR_NETWORK') {
        setError(t('auth.errors.network') || 'Network error. Please check if the backend is running.');
      } else {
        setError(t('auth.errors.general') || 'Login failed. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4">
      <div className="max-w-md w-full">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">{t('auth.login.title') || 'Welcome Back'}</h1>
          <p className="mt-2 text-gray-600">{t('auth.login.subtitle') || 'Sign in to your account'}</p>
        </div>

        <form onSubmit={handleSubmit} className="bg-white rounded-lg card-shadow p-8 space-y-6">
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('auth.login.email') || 'Email'}
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="fluent-input"
              placeholder="you@example.com"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              {t('auth.login.password') || 'Password'}
            </label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
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
            {loading ? (t('auth.login.signingIn') || 'Signing in...') : (t('auth.login.submit') || 'Sign In')}
          </button>

          <div className="text-center text-sm text-gray-600">
            {t('auth.login.noAccount') || "Don't have an account?"}{' '}
            <Link to="/register" className="text-[var(--kenya-green)] hover:underline font-medium">
              {t('auth.login.signUp') || 'Sign up'}
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}
