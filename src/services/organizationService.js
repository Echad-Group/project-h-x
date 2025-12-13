import api from './api';

const unitsService = {
  // Get all units with their teams
  async getAll(includeInactive = false) {
    const response = await api.get('/units', {
      params: { includeInactive }
    });
    return response.data;
  },

  // Get a specific unit with details
  async getById(id) {
    const response = await api.get(`/units/${id}`);
    return response.data;
  },

  // Get volunteers for a specific unit
  async getVolunteers(unitId, teamId = null) {
    const response = await api.get(`/units/${unitId}/volunteers`, {
      params: { teamId }
    });
    return response.data;
  },

  // Create a new unit (admin only)
  async create(unitData) {
    const response = await api.post('/units', unitData);
    return response.data;
  },

  // Update a unit (admin only)
  async update(id, unitData) {
    const response = await api.put(`/units/${id}`, unitData);
    return response.data;
  },

  // Delete a unit (admin only)
  async delete(id) {
    const response = await api.delete(`/units/${id}`);
    return response.data;
  }
};

const teamsService = {
  // Get all teams
  async getAll(unitId = null) {
    const response = await api.get('/teams', {
      params: { unitId }
    });
    return response.data;
  },

  // Get a specific team with details
  async getById(id) {
    const response = await api.get(`/teams/${id}`);
    return response.data;
  },

  // Create a new team (admin only)
  async create(teamData) {
    const response = await api.post('/teams', teamData);
    return response.data;
  },

  // Update a team (admin only)
  async update(id, teamData) {
    const response = await api.put(`/teams/${id}`, teamData);
    return response.data;
  },

  // Delete a team (admin only)
  async delete(id) {
    const response = await api.delete(`/teams/${id}`);
    return response.data;
  }
};

const assignmentsService = {
  // Get assignments for a volunteer
  async getByVolunteer(volunteerId) {
    const response = await api.get(`/assignments/volunteer/${volunteerId}`);
    return response.data;
  },

  // Assign volunteer to unit/team
  async create(assignmentData) {
    const response = await api.post('/assignments', assignmentData);
    return response.data;
  },

  // Update assignment
  async update(id, assignmentData) {
    const response = await api.put(`/assignments/${id}`, assignmentData);
    return response.data;
  },

  // Remove assignment
  async remove(id) {
    const response = await api.delete(`/assignments/${id}`);
    return response.data;
  },

  // Bulk assign volunteers
  async bulkAssign(bulkData) {
    const response = await api.post('/assignments/bulk', bulkData);
    return response.data;
  }
};

export { unitsService, teamsService, assignmentsService };
