import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { userProfileService } from '../services/userProfileService';
import { useTranslation } from 'react-i18next';

export default function Profile() {
  const { t } = useTranslation();
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  
  const [activeTab, setActiveTab] = useState('profile');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  
  // Profile state
  const [profile, setProfile] = useState(null);
  const [profileData, setProfileData] = useState({
    firstName: '',
    lastName: '',
    phoneNumber: '',
    bio: '',
    location: '',
    website: '',
    twitter: '',
    facebook: '',
    linkedIn: ''
  });
  
  // Password state
  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  
  // Email state
  const [emailData, setEmailData] = useState({
    newEmail: '',
    password: ''
  });
  
  // Delete account state
  const [deletePassword, setDeletePassword] = useState('');
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  useEffect(() => {
    loadProfile();
  }, []);

  async function loadProfile() {
    try {
      const data = await userProfileService.getProfile();
      setProfile(data);
      setProfileData({
        firstName: data.firstName || '',
        lastName: data.lastName || '',
        phoneNumber: data.phoneNumber || '',
        bio: data.bio || '',
        location: data.location || '',
        website: data.website || '',
        twitter: data.twitter || '',
        facebook: data.facebook || '',
        linkedIn: data.linkedIn || ''
      });
    } catch (err) {
      setError('Failed to load profile');
    } finally {
      setLoading(false);
    }
  }

  async function handleProfileUpdate(e) {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    setSaving(true);

    try {
      await userProfileService.updateProfile(profileData);
      setSuccess('Profile updated successfully');
      await loadProfile();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update profile');
    } finally {
      setSaving(false);
    }
  }

  async function handlePasswordChange(e) {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    setSaving(true);

    if (passwordData.newPassword !== passwordData.confirmPassword) {
      setError('Passwords do not match');
      setSaving(false);
      return;
    }

    try {
      await userProfileService.changePassword(passwordData);
      setSuccess('Password changed successfully');
      setPasswordData({
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      });
    } catch (err) {
      setError(err.response?.data?.errors?.join(', ') || 'Failed to change password');
    } finally {
      setSaving(false);
    }
  }

  async function handleEmailUpdate(e) {
    e.preventDefault();
    setError(null);
    setSuccess(null);
    setSaving(true);

    try {
      await userProfileService.updateEmail(emailData);
      setSuccess('Email updated successfully. Please log in again.');
      setTimeout(() => {
        logout();
        navigate('/login');
      }, 2000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to update email');
    } finally {
      setSaving(false);
    }
  }

  async function handleDeleteAccount() {
    if (!deletePassword) {
      setError('Please enter your password');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await userProfileService.deleteAccount(deletePassword);
      setSuccess('Account deleted successfully');
      setTimeout(() => {
        logout();
        navigate('/');
      }, 2000);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete account');
      setSaving(false);
    }
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading profile...</p>
        </div>
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p className="text-red-600">Failed to load profile</p>
      </div>
    );
  }

  const tabs = [
    { id: 'profile', label: 'Profile Information', icon: '👤' },
    { id: 'security', label: 'Security', icon: '🔒' },
    { id: 'account', label: 'Account Settings', icon: '⚙️' }
  ];

  return (
    <div className="min-h-screen bg-gray-50 py-12">
      <div className="max-w-5xl mx-auto px-4">
        {/* Header */}
        <div className="bg-white rounded-lg card-shadow p-6 mb-6">
          <div className="flex items-center gap-4">
            <div className="w-20 h-20 bg-[var(--kenya-green)] rounded-full flex items-center justify-center text-white text-3xl font-bold">
              {profile.firstName?.charAt(0)}{profile.lastName?.charAt(0)}
            </div>
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                {profile.firstName} {profile.lastName}
              </h1>
              <p className="text-gray-600">{profile.email}</p>
              <div className="flex gap-2 mt-2">
                {profile.roles.map(role => (
                  <span 
                    key={role}
                    className="px-2 py-1 text-xs rounded-full bg-[var(--kenya-green)] text-white"
                  >
                    {role}
                  </span>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* Alerts */}
        {error && (
          <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg mb-6">
            {error}
          </div>
        )}
        {success && (
          <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-lg mb-6">
            {success}
          </div>
        )}

        {/* Tabs */}
        <div className="bg-white rounded-lg card-shadow overflow-hidden">
          <div className="border-b border-gray-200">
            <div className="flex">
              {tabs.map(tab => (
                <button
                  key={tab.id}
                  onClick={() => {
                    setActiveTab(tab.id);
                    setError(null);
                    setSuccess(null);
                  }}
                  className={`flex-1 px-6 py-4 text-center transition-colors ${
                    activeTab === tab.id
                      ? 'border-b-2 border-[var(--kenya-green)] text-[var(--kenya-green)] font-semibold'
                      : 'text-gray-600 hover:text-gray-900'
                  }`}
                >
                  <span className="mr-2">{tab.icon}</span>
                  {tab.label}
                </button>
              ))}
            </div>
          </div>

          <div className="p-6">
            {/* Profile Tab */}
            {activeTab === 'profile' && (
              <form onSubmit={handleProfileUpdate} className="space-y-6">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      First Name *
                    </label>
                    <input
                      type="text"
                      required
                      value={profileData.firstName}
                      onChange={(e) => setProfileData({...profileData, firstName: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Last Name *
                    </label>
                    <input
                      type="text"
                      required
                      value={profileData.lastName}
                      onChange={(e) => setProfileData({...profileData, lastName: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Phone Number
                  </label>
                  <input
                    type="tel"
                    value={profileData.phoneNumber}
                    onChange={(e) => setProfileData({...profileData, phoneNumber: e.target.value})}
                    placeholder="+254712345678"
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Bio
                  </label>
                  <textarea
                    value={profileData.bio}
                    onChange={(e) => setProfileData({...profileData, bio: e.target.value})}
                    rows="4"
                    maxLength="200"
                    placeholder="Tell us about yourself..."
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                  />
                  <p className="text-sm text-gray-500 mt-1">
                    {profileData.bio?.length || 0}/200 characters
                  </p>
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Location
                  </label>
                  <input
                    type="text"
                    value={profileData.location}
                    onChange={(e) => setProfileData({...profileData, location: e.target.value})}
                    placeholder="Nairobi, Kenya"
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">
                    Website
                  </label>
                  <input
                    type="url"
                    value={profileData.website}
                    onChange={(e) => setProfileData({...profileData, website: e.target.value})}
                    placeholder="https://example.com"
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                  />
                </div>

                <div className="border-t pt-6">
                  <h3 className="text-lg font-semibold mb-4">Social Media</h3>
                  <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Twitter
                      </label>
                      <input
                        type="text"
                        value={profileData.twitter}
                        onChange={(e) => setProfileData({...profileData, twitter: e.target.value})}
                        placeholder="@username"
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                      />
                    </div>
                    
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        Facebook
                      </label>
                      <input
                        type="text"
                        value={profileData.facebook}
                        onChange={(e) => setProfileData({...profileData, facebook: e.target.value})}
                        placeholder="username"
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                      />
                    </div>
                    
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        LinkedIn
                      </label>
                      <input
                        type="text"
                        value={profileData.linkedIn}
                        onChange={(e) => setProfileData({...profileData, linkedIn: e.target.value})}
                        placeholder="username"
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                      />
                    </div>
                  </div>
                </div>

                <div className="flex justify-end">
                  <button
                    type="submit"
                    disabled={saving}
                    className="px-6 py-3 bg-[var(--kenya-green)] text-white rounded-lg hover:bg-opacity-90 disabled:opacity-50 transition-colors"
                  >
                    {saving ? 'Saving...' : 'Save Changes'}
                  </button>
                </div>
              </form>
            )}

            {/* Security Tab */}
            {activeTab === 'security' && (
              <div className="space-y-8">
                {/* Change Password */}
                <form onSubmit={handlePasswordChange} className="space-y-4">
                  <h3 className="text-lg font-semibold mb-4">Change Password</h3>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Current Password *
                    </label>
                    <input
                      type="password"
                      required
                      value={passwordData.currentPassword}
                      onChange={(e) => setPasswordData({...passwordData, currentPassword: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      New Password * (min 6 characters)
                    </label>
                    <input
                      type="password"
                      required
                      minLength="6"
                      value={passwordData.newPassword}
                      onChange={(e) => setPasswordData({...passwordData, newPassword: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Confirm New Password *
                    </label>
                    <input
                      type="password"
                      required
                      minLength="6"
                      value={passwordData.confirmPassword}
                      onChange={(e) => setPasswordData({...passwordData, confirmPassword: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={saving}
                    className="px-6 py-3 bg-[var(--kenya-green)] text-white rounded-lg hover:bg-opacity-90 disabled:opacity-50 transition-colors"
                  >
                    {saving ? 'Updating...' : 'Update Password'}
                  </button>
                </form>

                {/* Account Info */}
                <div className="border-t pt-6">
                  <h3 className="text-lg font-semibold mb-4">Account Information</h3>
                  <div className="space-y-3 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Member since:</span>
                      <span className="font-medium">{new Date(profile.createdAt).toLocaleDateString()}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Last login:</span>
                      <span className="font-medium">
                        {profile.lastLoginAt ? new Date(profile.lastLoginAt).toLocaleString() : 'N/A'}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">Email verified:</span>
                      <span className={profile.emailConfirmed ? 'text-green-600' : 'text-orange-600'}>
                        {profile.emailConfirmed ? '✓ Verified' : '⚠ Not verified'}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Account Settings Tab */}
            {activeTab === 'account' && (
              <div className="space-y-8">
                {/* Update Email */}
                <form onSubmit={handleEmailUpdate} className="space-y-4">
                  <h3 className="text-lg font-semibold mb-4">Update Email Address</h3>
                  <p className="text-sm text-gray-600 mb-4">
                    Current email: <strong>{profile.email}</strong>
                  </p>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      New Email *
                    </label>
                    <input
                      type="email"
                      required
                      value={emailData.newEmail}
                      onChange={(e) => setEmailData({...emailData, newEmail: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Confirm Password *
                    </label>
                    <input
                      type="password"
                      required
                      value={emailData.password}
                      onChange={(e) => setEmailData({...emailData, password: e.target.value})}
                      className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                    />
                  </div>

                  <button
                    type="submit"
                    disabled={saving}
                    className="px-6 py-3 bg-[var(--kenya-green)] text-white rounded-lg hover:bg-opacity-90 disabled:opacity-50 transition-colors"
                  >
                    {saving ? 'Updating...' : 'Update Email'}
                  </button>
                </form>

                {/* Delete Account */}
                <div className="border-t border-red-200 pt-6">
                  <h3 className="text-lg font-semibold text-red-600 mb-4">Danger Zone</h3>
                  <p className="text-sm text-gray-600 mb-4">
                    Once you delete your account, there is no going back. Please be certain.
                  </p>
                  <button
                    onClick={() => setShowDeleteModal(true)}
                    className="px-6 py-3 bg-red-600 text-white rounded-lg hover:bg-red-700 transition-colors"
                  >
                    Delete Account
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Delete Modal */}
        {showDeleteModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4">
              <h3 className="text-xl font-bold text-red-600 mb-4">Delete Account</h3>
              <p className="text-gray-600 mb-4">
                Are you absolutely sure? This action cannot be undone. All your data will be permanently deleted.
              </p>
              
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Enter your password to confirm
                </label>
                <input
                  type="password"
                  value={deletePassword}
                  onChange={(e) => setDeletePassword(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-transparent"
                />
              </div>

              <div className="flex gap-3">
                <button
                  onClick={() => {
                    setShowDeleteModal(false);
                    setDeletePassword('');
                    setError(null);
                  }}
                  disabled={saving}
                  className="flex-1 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
                >
                  Cancel
                </button>
                <button
                  onClick={handleDeleteAccount}
                  disabled={saving || !deletePassword}
                  className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50 transition-colors"
                >
                  {saving ? 'Deleting...' : 'Delete Account'}
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
