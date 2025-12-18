import api from './api';

export const userProfileService = {
  // Get current user's profile
  getProfile: async () => {
    const response = await api.get('/userprofile');
    return response.data;
  },

  // Update profile information
  updateProfile: async (profileData) => {
    const response = await api.put('/userprofile', profileData);
    return response.data;
  },

  // Change password
  changePassword: async (passwordData) => {
    const response = await api.put('/userprofile/password', passwordData);
    return response.data;
  },

  // Update email
  updateEmail: async (emailData) => {
    const response = await api.put('/userprofile/email', emailData);
    return response.data;
  },

  // Delete account
  deleteAccount: async (password) => {
    const response = await api.delete('/userprofile', {
      data: JSON.stringify(password),
      headers: { 'Content-Type': 'application/json' }
    });
    return response.data;
  }
};
