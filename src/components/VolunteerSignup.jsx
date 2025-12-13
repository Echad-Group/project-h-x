import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { volunteerService } from '../services/volunteerService';
import { useAuth } from '../contexts/AuthContext';

export default function VolunteerSignup({ onSubmit }) {
  const [form, setForm] = useState({
    name: '',
    email: '',
    phone: '',
    city: '',
    region: '',
    availabilityZones: [],
    skills: [],
    hoursPerWeek: 5,
    availableWeekends: false,
    availableEvenings: false,
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
    const { name, value, type, checked } = e.target;
    if (type === 'checkbox') {
      if (name === 'skills' || name === 'availabilityZones') {
        const currentArray = form[name];
        const newArray = checked
          ? [...currentArray, value]
          : currentArray.filter(item => item !== value);
        setForm({ ...form, [name]: newArray });
      } else {
        setForm({ ...form, [name]: checked });
      }
    } else if (type === 'range') {
      setForm({ ...form, [name]: parseInt(value) });
    } else {
      setForm({ ...form, [name]: value });
    }
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
      // Prepare data with arrays converted to comma-separated strings
      const submitData = {
        ...form,
        skills: form.skills.join(','),
        availabilityZones: form.availabilityZones.join(',')
      };
      
      const response = await volunteerService.create(submitData);
      
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
      <div className="fluent-card bg-green-50 p-8 text-center max-w-2xl mx-auto">
        <div className="inline-flex items-center justify-center w-20 h-20 bg-green-100 rounded-full mb-4">
          <svg className="w-10 h-10 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h3 className="text-2xl font-bold text-green-800 mb-2">
          {t('volunteer.success.title') || 'Registration Successful!'}
        </h3>
        <p className="text-lg text-green-700 mb-4">
          {t('volunteer.success.desc') || 'Thank you for joining the New Kenya movement!'}
        </p>
        
        {!isAuthenticated && (
          <div className="bg-white rounded-lg p-6 text-left space-y-3 mb-4">
            <div className="flex items-start gap-3">
              <svg className="w-6 h-6 text-blue-600 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
              <div>
                <h4 className="font-semibold text-gray-900 mb-1">Check Your Email</h4>
                <p className="text-sm text-gray-700">
                  We've created an account for you and sent instructions to <strong>{form.email}</strong> to set your password.
                </p>
              </div>
            </div>
            
            <div className="flex items-start gap-3">
              <svg className="w-6 h-6 text-green-600 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <div>
                <h4 className="font-semibold text-gray-900 mb-1">Next Steps</h4>
                <ol className="text-sm text-gray-700 list-decimal list-inside space-y-1">
                  <li>Click the link in your email to set your password</li>
                  <li>Log in to your volunteer dashboard</li>
                  <li>View your assignments and connect with your team</li>
                </ol>
              </div>
            </div>
          </div>
        )}
        
        <div className="flex flex-col sm:flex-row gap-3 justify-center">
          {!isAuthenticated ? (
            <>
              <a href="/login" className="fluent-btn fluent-btn-primary">
                Log In
              </a>
              <button 
                onClick={() => {setSubmitted(false); setForm({
                  name: '', email: '', phone: '', city: '', region: '', availabilityZones: [],
                  skills: [], hoursPerWeek: 5, availableWeekends: false, availableEvenings: false, interests: ''
                })}} 
                className="fluent-btn fluent-btn-secondary"
              >
                Register Another Volunteer
              </button>
            </>
          ) : (
            <a href="/volunteer/dashboard" className="fluent-btn fluent-btn-primary">
              Go to Dashboard
            </a>
          )}
        </div>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-6 p-6 bg-white rounded-lg card-shadow max-w-2xl mx-auto">
      <div className="text-center mb-6">
        <h2 className="text-3xl font-bold text-gray-900 mb-2">{t('volunteer.heading') || 'Join as a Volunteer'}</h2>
        <p className="text-gray-600">
          {t('volunteer.subheading') || 'Be part of the movement to build a better Kenya'}
        </p>
        {!isAuthenticated && (
          <div className="mt-4 bg-blue-50 border border-blue-200 rounded-lg p-3 text-sm text-left">
            <div className="flex items-start gap-2">
              <svg className="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <p className="text-blue-800">
                <strong>Account Creation:</strong> We'll automatically create an account for you so you can access your volunteer dashboard, view assignments, and connect with your team. You'll receive an email to set your password.
              </p>
            </div>
          </div>
        )}
      </div>
      
      {error && <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded text-sm">{error}</div>}
      
      {/* Contact Information */}
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
          </svg>
          {t('volunteer.sections.contact') || 'Contact Information'}
        </h3>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium mb-1">{t('volunteer.form.name')}</label>
            <input name="name" value={form.name} onChange={handleChange} required className="fluent-input" />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">{t('volunteer.form.email')}</label>
            <input name="email" type="email" value={form.email} onChange={handleChange} required className="fluent-input" />
          </div>
        </div>
        
        <div>
          <label className="block text-sm font-medium mb-1">
            {t('volunteer.form.phone')}
            <span className="text-xs text-gray-500 ml-2">{t('volunteer.form.phoneHint') || '(SMS/Call/Telegram/WhatsApp)'}</span>
          </label>
          <input name="phone" type="tel" value={form.phone} onChange={handleChange} className="fluent-input" placeholder="+254..." />
        </div>
      </div>

      {/* Location */}
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          {t('volunteer.sections.location') || 'Location'}
        </h3>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium mb-1">{t('volunteer.form.city')}</label>
            <input name="city" value={form.city} onChange={handleChange} className="fluent-input" placeholder="Nairobi" />
          </div>
          <div>
            <label className="block text-sm font-medium mb-1">{t('volunteer.form.region')}</label>
            <select name="region" value={form.region} onChange={handleChange} className="fluent-input">
              <option value="">{t('volunteer.form.selectRegion') || 'Select Region'}</option>
              <option value="Nairobi">Nairobi</option>
              <option value="Coast">Coast</option>
              <option value="Western">Western</option>
              <option value="Eastern">Eastern</option>
              <option value="Central">Central</option>
              <option value="Rift Valley">Rift Valley</option>
              <option value="Nyanza">Nyanza</option>
              <option value="North Eastern">North Eastern</option>
            </select>
          </div>
        </div>
        
        <div>
          <label className="block text-sm font-medium mb-2">{t('volunteer.form.availabilityZones')}</label>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
            {['Central', 'East', 'West', 'North', 'South', 'Suburbs', 'Rural', 'Urban'].map(zone => (
              <label key={zone} className="flex items-center space-x-2 text-sm">
                <input
                  type="checkbox"
                  name="availabilityZones"
                  value={zone}
                  checked={form.availabilityZones.includes(zone)}
                  onChange={handleChange}
                  className="rounded"
                />
                <span>{zone}</span>
              </label>
            ))}
          </div>
        </div>
      </div>

      {/* Skills Offered */}
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
          </svg>
          {t('volunteer.sections.skills') || 'Skills Offered'}
        </h3>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          {[
            { value: 'Design', icon: '🎨' },
            { value: 'Social Media', icon: '📱' },
            { value: 'Logistics', icon: '📦' },
            { value: 'Public Speaking', icon: '🎤' },
            { value: 'Fundraising', icon: '💰' },
            { value: 'Tech Support', icon: '💻' },
            { value: 'Event Planning', icon: '📅' },
            { value: 'Community Outreach', icon: '🤝' }
          ].map(skill => (
            <label key={skill.value} className="flex items-center space-x-2 text-sm p-2 rounded hover:bg-gray-50">
              <input
                type="checkbox"
                name="skills"
                value={skill.value}
                checked={form.skills.includes(skill.value)}
                onChange={handleChange}
                className="rounded"
              />
              <span>{skill.icon}</span>
              <span>{t(`volunteer.skills.${skill.value.toLowerCase().replace(' ', '_')}`) || skill.value}</span>
            </label>
          ))}
        </div>
      </div>

      {/* Availability */}
      <div className="space-y-4">
        <h3 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          {t('volunteer.sections.availability') || 'Availability'}
        </h3>
        
        <div>
          <label className="block text-sm font-medium mb-2">
            {t('volunteer.form.hoursPerWeek') || 'Hours per week'}: <span className="font-bold text-[var(--kenya-green)]">{form.hoursPerWeek}</span>
          </label>
          <input
            type="range"
            name="hoursPerWeek"
            min="0"
            max="40"
            value={form.hoursPerWeek}
            onChange={handleChange}
            className="w-full h-2 bg-gray-200 rounded-lg appearance-none cursor-pointer accent-[var(--kenya-green)]"
          />
          <div className="flex justify-between text-xs text-gray-500 mt-1">
            <span>0h</span>
            <span>20h</span>
            <span>40h</span>
          </div>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <label className="flex items-center justify-between p-3 border rounded-lg cursor-pointer hover:bg-gray-50">
            <span className="text-sm font-medium">{t('volunteer.form.availableWeekends') || 'Available on weekends'}</span>
            <input
              type="checkbox"
              name="availableWeekends"
              checked={form.availableWeekends}
              onChange={handleChange}
              className="rounded"
            />
          </label>
          
          <label className="flex items-center justify-between p-3 border rounded-lg cursor-pointer hover:bg-gray-50">
            <span className="text-sm font-medium">{t('volunteer.form.availableEvenings') || 'Available in evenings'}</span>
            <input
              type="checkbox"
              name="availableEvenings"
              checked={form.availableEvenings}
              onChange={handleChange}
              className="rounded"
            />
          </label>
        </div>
      </div>

      {/* Additional Interests */}
      <div>
        <label className="block text-sm font-medium mb-1">{t('volunteer.form.interests')}</label>
        <textarea 
          name="interests" 
          value={form.interests} 
          onChange={handleChange} 
          className="fluent-input" 
          rows="3"
          placeholder={t('volunteer.form.interestsPlaceholder')}
        />
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

