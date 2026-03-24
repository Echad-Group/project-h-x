import api from './api';

export const volunteerService = {
  // Get all volunteers (legacy, unfiltered)
  getAll: async () => {
    const response = await api.get('/volunteers');
    return response.data;
  },

  // Get paged volunteers for admin panel
  // Returns { volunteers, totalCount, page, pageSize }
  getPaged: async ({ page = 1, pageSize = 25, search, region, skills } = {}) => {
    const params = { page, pageSize };
    if (search) params.search = search;
    if (region) params.region = region;
    if (skills) params.skills = skills;
    const response = await api.get('/volunteers/admin', { params });
    return response.data;
  },

  // Get volunteer by ID
  getById: async (id) => {
    const response = await api.get(`/volunteers/${id}`);
    return response.data;
  },

  // Check if current user is already a volunteer
  checkStatus: async () => {
    const response = await api.get('/volunteers/check-status');
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
