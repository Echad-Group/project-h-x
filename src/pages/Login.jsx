import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useTranslation } from 'react-i18next';
import { authService } from '../services/authService';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [otpCode, setOtpCode] = useState('');
  const [otpEmail, setOtpEmail] = useState('');
  const [otpStage, setOtpStage] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [otpLoading, setOtpLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');
    setLoading(true);

    try {
      await login({ email, password });
      navigate('/');
    } catch (err) {
      console.error('Login error:', err);

      if (err.response?.status === 401 && err.response?.data?.otpRequired) {
        const challengeEmail = err.response?.data?.email || email;
        setOtpEmail(challengeEmail);
        setOtpStage(true);
        setMessage('We sent a one-time code. Enter it below to complete sign in.');
        return;
      }

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

  const handleVerifyOtp = async (e) => {
    e.preventDefault();
    setError('');
    setMessage('');
    setOtpLoading(true);

    try {
      await authService.verifyOtp({
        email: otpEmail,
        code: otpCode,
        purpose: 'Login',
        level: 'phone'
      });

      await login({ email: otpEmail || email, password });
      navigate('/');
    } catch (err) {
      console.error('OTP verification error:', err);
      if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else {
        setError('OTP verification failed. Please try again.');
      }
    } finally {
      setOtpLoading(false);
    }
  };

  const handleResendOtp = async () => {
    setError('');
    setMessage('');
    setOtpLoading(true);

    try {
      await authService.sendOtp({ email: otpEmail || email, purpose: 'Login' });
      setMessage('A new OTP code has been sent.');
    } catch (err) {
      console.error('OTP resend error:', err);
      setError('Could not resend OTP right now. Please try again.');
    } finally {
      setOtpLoading(false);
    }
  };

  const backToCredentials = () => {
    setOtpStage(false);
    setOtpCode('');
    setError('');
    setMessage('');
  };

  return (
    <div className="min-h-screen bg-gray-50 flex items-center justify-center py-12 px-4">
      <div className="max-w-md w-full">
        <div className="text-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            {t('auth.login.title') || 'Welcome Back'}
          </h1>
          <p className="mt-2 text-gray-600">
            {t('auth.login.subtitle') || 'Sign in to your account'}
          </p>
        </div>

        {/* Login card — always rendered so it stays visible behind the OTP modal */}
        <div className="relative">
          <form onSubmit={handleSubmit} className="bg-white rounded-lg card-shadow p-8 space-y-6">
            {error && !otpStage && (
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

          {/* OTP modal overlay — mounts on top of the login card when challenge is triggered */}
          {otpStage && (
            <div className="absolute inset-0 flex items-center justify-center rounded-lg">
              {/* Frosted backdrop */}
              <div className="absolute inset-0 bg-white/80 backdrop-blur-sm rounded-lg" />

              {/* Modal card */}
              <div className="relative w-full mx-4 rounded-2xl bg-white shadow-2xl ring-1 ring-black/5 p-7 space-y-5">
                {/* Header */}
                <div className="flex items-start justify-between">
                  <div>
                    <p className="text-xs uppercase tracking-widest text-gray-400 mb-1">Two-Factor Verification</p>
                    <p className="text-sm text-gray-600">
                      Code sent to <span className="font-medium text-gray-900">{otpEmail || email}</span>
                    </p>
                  </div>
                  <button
                    type="button"
                    onClick={backToCredentials}
                    disabled={otpLoading}
                    aria-label="Close OTP dialog"
                    className="rounded-full p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700 disabled:opacity-50 transition-colors"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                      <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
                    </svg>
                  </button>
                </div>

                {error && (
                  <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
                    {error}
                  </div>
                )}
                {message && (
                  <div className="bg-emerald-50 border border-emerald-200 text-emerald-700 px-4 py-3 rounded-lg text-sm">
                    {message}
                  </div>
                )}

                <form onSubmit={handleVerifyOtp} className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">One-Time Password</label>
                    <input
                      type="text"
                      value={otpCode}
                      onChange={(e) => setOtpCode(e.target.value.replace(/\D/g, '').slice(0, 6))}
                      required
                      maxLength={6}
                      autoFocus
                      className="fluent-input tracking-[0.4em] text-center text-xl font-semibold"
                      placeholder="123456"
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={otpLoading}
                    className="fluent-btn fluent-btn-primary w-full disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {otpLoading ? 'Verifying...' : 'Verify and Sign In'}
                  </button>
                </form>

                <div className="text-center text-sm">
                  <button
                    type="button"
                    onClick={handleResendOtp}
                    disabled={otpLoading}
                    className="text-[var(--kenya-green)] hover:underline disabled:opacity-50"
                  >
                    Resend OTP
                  </button>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
