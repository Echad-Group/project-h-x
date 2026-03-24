import api from './api';

export const commandDashboardService = {
  async getSummary() {
    const response = await api.get('/command-dashboard/summary');
    return response.data;
  },

  async getOperationsHealth() {
    const response = await api.get('/ops-health');
    return response.data;
  }
};

export const complianceService = {
  async getSummary() {
    const response = await api.get('/compliance/summary');
    return response.data;
  },

  async queueReminder(reminderData) {
    const response = await api.post('/compliance/reminder', reminderData);
    return response.data;
  }
};

export const downlinesService = {
  async getTree(rootUserId = null, maxDepth = 3) {
    const response = await api.get('/downlines/tree', {
      params: {
        rootUserId: rootUserId || undefined,
        maxDepth
      }
    });
    return response.data;
  },

  async add(requestData) {
    const response = await api.post('/downlines/add', requestData);
    return response.data;
  },

  async reassign(requestData) {
    const response = await api.post('/downlines/reassign', requestData);
    return response.data;
  },

  async remove(requestData) {
    const response = await api.post('/downlines/remove', requestData);
    return response.data;
  },

  async getLeaderCapacity() {
    const response = await api.get('/downlines/leader-capacity');
    return response.data;
  }
};

export const campaignTasksService = {
  async getMine() {
    const response = await api.get('/tasks/my-tasks');
    return response.data;
  },

  async getManage(params = {}) {
    const response = await api.get('/tasks/manage', { params });
    return response.data;
  },

  async create(taskData) {
    const response = await api.post('/tasks/create', taskData);
    return response.data;
  },

  async assign(taskData) {
    const response = await api.post('/tasks/assign', taskData);
    return response.data;
  },

  async updateStatus(statusData) {
    const response = await api.post('/tasks/status', statusData);
    return response.data;
  },

  async complete(taskData) {
    const response = await api.post('/tasks/complete', taskData);
    return response.data;
  },

  async update(taskId, payload) {
    const response = await api.put(`/tasks/${taskId}`, payload);
    return response.data;
  },

  async remove(taskId) {
    const response = await api.delete(`/tasks/${taskId}`);
    return response.data;
  }
};

export const campaignMessagingService = {
  async broadcast(messageData) {
    const response = await api.post('/messages/broadcast', messageData);
    return response.data;
  },

  async target(messageData) {
    const response = await api.post('/messages/target', messageData);
    return response.data;
  },

  async getAnalytics() {
    const response = await api.get('/messages/analytics');
    return response.data;
  },

  async getInbox() {
    const response = await api.get('/messages/inbox');
    return response.data;
  },

  async acknowledgeRead(messageId) {
    const response = await api.post(`/messages/${messageId}/read`);
    return response.data;
  }
};

export const electionResultsService = {
  async submit(resultData) {
    const response = await api.post('/results/submit', resultData);
    return response.data;
  },

  async getAggregate(params = {}) {
    const response = await api.get('/results/aggregate', { params });
    return response.data;
  },

  async getStatus(params = {}) {
    const response = await api.get('/results/status', { params });
    return response.data;
  },

  async getPendingReview(params = {}) {
    const response = await api.get('/results/pending-review', { params });
    return response.data;
  },

  async reviewResult(resultId, payload) {
    const response = await api.post(`/results/${resultId}/review`, payload);
    return response.data;
  },

  async getConflicts(params = {}) {
    const response = await api.get('/results/conflicts', { params });
    return response.data;
  },

  async adjudicateConflict(conflictGroupKey, payload) {
    const response = await api.post(`/results/conflicts/${encodeURIComponent(conflictGroupKey)}/adjudicate`, payload);
    return response.data;
  }
};

export const verificationService = {
  async getQueue(status = 'Pending') {
    const response = await api.get('/verification/queue', {
      params: { status }
    });
    return response.data;
  },

  async getQueueItem(userId) {
    const response = await api.get(`/verification/queue/${userId}`);
    return response.data;
  },

  async decide(userId, payload) {
    const response = await api.post(`/verification/queue/${userId}/decision`, payload);
    return response.data;
  }
};

export const leaderboardService = {
  async recalculate() {
    const response = await api.post('/leaderboard/recalculate');
    return response.data;
  },

  async get(params = {}) {
    const response = await api.get('/leaderboard', { params });
    return response.data;
  },

  async getMyRank(params = {}) {
    const response = await api.get('/leaderboard/my-rank', { params });
    return response.data;
  }
};

export const geolocationService = {
  async ingest(payload) {
    const response = await api.post('/geolocation/ingest', payload);
    return response.data;
  },

  async getCoverage(params = {}) {
    const response = await api.get('/geolocation/coverage', { params });
    return response.data;
  }
};

export const warRoomService = {
  async getState() {
    const response = await api.get('/warroom/state');
    return response.data;
  },

  async getCommandPods() {
    const response = await api.get('/warroom/pods');
    return response.data;
  },

  async createCommandPod(payload) {
    const response = await api.post('/warroom/pods', payload);
    return response.data;
  },

  async updateCommandPod(podId, payload) {
    const response = await api.put(`/warroom/pods/${podId}`, payload);
    return response.data;
  },

  async getBattleRhythm(date = null) {
    const response = await api.get('/warroom/battle-rhythm', {
      params: {
        date: date || undefined
      }
    });
    return response.data;
  },

  async createIncident(payload) {
    const response = await api.post('/warroom/incidents', payload);
    return response.data;
  },

  async updateIncident(incidentId, payload) {
    const response = await api.put(`/warroom/incidents/${incidentId}`, payload);
    return response.data;
  },

  async escalateIncident(incidentId, payload) {
    const response = await api.post(`/warroom/incidents/${incidentId}/escalate`, payload);
    return response.data;
  },

  async completeBattleRhythmItem(itemId, payload) {
    const response = await api.post(`/warroom/battle-rhythm/${itemId}/complete`, payload);
    return response.data;
  },

  async getRedZoneState() {
    const response = await api.get('/warroom/red-zone');
    return response.data;
  },

  async toggleRedZoneMode(payload) {
    const response = await api.post('/warroom/red-zone/toggle', payload);
    return response.data;
  },

  async addRedZoneDecision(payload) {
    const response = await api.post('/warroom/red-zone/decisions', payload);
    return response.data;
  },

  async getCommandGrid() {
    const response = await api.get('/warroom/command-grid');
    return response.data;
  },

  async updateCommandGridNode(nodeId, payload) {
    const response = await api.put(`/warroom/command-grid/${nodeId}`, payload);
    return response.data;
  },

  async getCoalitions() {
    const response = await api.get('/warroom/coalitions');
    return response.data;
  },

  async createCoalition(payload) {
    const response = await api.post('/warroom/coalitions', payload);
    return response.data;
  },

  async updateCoalitionModule(coalitionId, payload) {
    const response = await api.put(`/warroom/coalitions/${coalitionId}/modules`, payload);
    return response.data;
  },

  async getMobilizationRoles() {
    const response = await api.get('/warroom/mobilization-roles');
    return response.data;
  },

  async updateMobilizationRole(roleCode, payload) {
    const response = await api.put(`/warroom/mobilization-roles/${roleCode}`, payload);
    return response.data;
  },

  async getCampaignPhases() {
    const response = await api.get('/warroom/campaign-phases');
    return response.data;
  },

  async switchCampaignPhase(payload) {
    const response = await api.post('/warroom/campaign-phases/switch', payload);
    return response.data;
  },

  async getLegalCases() {
    const response = await api.get('/warroom/legal-cases');
    return response.data;
  },

  async createLegalCase(payload) {
    const response = await api.post('/warroom/legal-cases', payload);
    return response.data;
  },

  async updateLegalCase(caseId, payload) {
    const response = await api.put(`/warroom/legal-cases/${caseId}`, payload);
    return response.data;
  }
};