import api from './api';

// Events Service
export const eventsService = {
  // Get all events
  getAll: async (includeUnpublished = false) => {
    const response = await api.get('/events', {
      params: { includeUnpublished }
    });
    return response.data;
  },

  // Get event by ID
  getById: async (id) => {
    const response = await api.get(`/events/${id}`);
    return response.data;
  },

  // Get event by slug
  getBySlug: async (slug) => {
    const response = await api.get(`/events/slug/${slug}`);
    return response.data;
  },

  // Get upcoming events
  getUpcoming: async (limit = 10) => {
    const response = await api.get('/events/upcoming', {
      params: { limit }
    });
    return response.data;
  },

  // Get past events
  getPast: async (limit = 10) => {
    const response = await api.get('/events/past', {
      params: { limit }
    });
    return response.data;
  },

  // Create new event (Admin only)
  create: async (eventData) => {
    const response = await api.post('/events', eventData);
    return response.data;
  },

  // Update event (Admin only)
  update: async (id, eventData) => {
    const response = await api.put(`/events/${id}`, eventData);
    return response.data;
  },

  // Delete event (Admin only)
  delete: async (id) => {
    const response = await api.delete(`/events/${id}`);
    return response.data;
  },
};

// Event RSVPs Service
export const eventRSVPService = {
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

// Backwards compatibility
export const eventService = eventRSVPService;
