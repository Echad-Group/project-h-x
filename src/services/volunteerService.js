import api from './api';

export const volunteerService = {
  // Get all volunteers
  getAll: async () => {
    const response = await api.get('/volunteers');
    return response.data;
  },

  // Get volunteer by ID
  getById: async (id) => {
    const response = await api.get(`/volunteers/${id}`);
    return response.data;
  },

  // Create new volunteer signup
  create: async (volunteerData) => {
    const response = await api.post('/volunteers', volunteerData);
    return response.data;
  },

  // Delete volunteer
  delete: async (id) => {
    const response = await api.delete(`/volunteers/${id}`);
    return response.data;
  },
};
