import api from './api';

const campaignTeamService = {
  // Get all active campaign team members
  async getMembers() {
    const response = await api.get('/campaignteam');
    return response.data;
  },

  // Get a single team member by ID
  async getMember(id) {
    const response = await api.get(`/campaignteam/${id}`);
    return response.data;
  },

  // Create a new team member (Admin only)
  async createMember(memberData) {
    const response = await api.post('/campaignteam', memberData);
    return response.data;
  },

  // Update an existing team member (Admin only)
  async updateMember(id, memberData) {
    const response = await api.put(`/campaignteam/${id}`, {
      ...memberData,
      id: id
    });
    return response.data;
  },

  // Delete a team member (Admin only)
  async deleteMember(id) {
    const response = await api.delete(`/campaignteam/${id}`);
    return response.data;
  },

  // Reorder a team member (Admin only)
  async reorderMember(id, newOrder) {
    const response = await api.put(`/campaignteam/${id}/reorder`, newOrder);
    return response.data;
  }
};

export default campaignTeamService;
