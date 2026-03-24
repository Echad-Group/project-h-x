import api from './api';

export const adminEmailArchiveService = {
  async list(limit = 200) {
    const response = await api.get('/adminemailarchive', {
      params: { limit }
    });
    return response.data;
  }
};
