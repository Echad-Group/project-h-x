import api from './api';

export const donationService = {
  // Get all donations
  getAll: async () => {
    const response = await api.get('/donations');
    return response.data;
  },

  // Get donation by ID
  getById: async (id) => {
    const response = await api.get(`/donations/${id}`);
    return response.data;
  },

  // Get donation statistics
  getStats: async () => {
    const response = await api.get('/donations/stats');
    return response.data;
  },

  // Create new donation
  create: async (donationData) => {
    const response = await api.post('/donations', donationData);
    return response.data;
  },

  // Delete donation
  delete: async (id) => {
    const response = await api.delete(`/donations/${id}`);
    return response.data;
  },
};
