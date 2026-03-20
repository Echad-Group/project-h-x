import api from './api';

export const commandDashboardService = {
  async getSummary() {
    const response = await api.get('/command-dashboard/summary');
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
  }
};

export const campaignTasksService = {
  async getMine() {
    const response = await api.get('/tasks/my-tasks');
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
  }
};