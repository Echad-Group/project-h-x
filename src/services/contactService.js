import api from './api';

export const contactService = {
  // Get all contact submissions
  getAll: async () => {
    const response = await api.get('/contacts');
    return response.data;
  },

  // Get contact by ID
  getById: async (id) => {
    const response = await api.get(`/contacts/${id}`);
    return response.data;
  },

  // Create new contact submission
  create: async (contactData) => {
    const response = await api.post('/contacts', contactData);
    return response.data;
  },

  // Delete contact
  delete: async (id) => {
    const response = await api.delete(`/contacts/${id}`);
    return response.data;
  },
};
