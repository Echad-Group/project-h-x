import api from './api';

export const eventService = {
  // Get all RSVPs (optionally filtered by event)
  getAll: async (eventId = null) => {
    const url = eventId ? `/eventrsvps?eventId=${eventId}` : '/eventrsvps';
    const response = await api.get(url);
    return response.data;
  },

  // Get RSVP by ID
  getById: async (id) => {
    const response = await api.get(`/eventrsvps/${id}`);
    return response.data;
  },

  // Get RSVP count for an event
  getCount: async (eventId) => {
    const response = await api.get(`/eventrsvps/count/${eventId}`);
    return response.data;
  },

  // Create new RSVP
  create: async (rsvpData) => {
    const response = await api.post('/eventrsvps', rsvpData);
    return response.data;
  },

  // Update existing RSVP
  update: async (id, rsvpData) => {
    const response = await api.put(`/eventrsvps/${id}`, rsvpData);
    return response.data;
  },

  // Delete RSVP
  delete: async (id) => {
    const response = await api.delete(`/eventrsvps/${id}`);
    return response.data;
  },
};
