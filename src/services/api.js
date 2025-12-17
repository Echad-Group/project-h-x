import axios from 'axios';

// Base API configuration
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5065/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 5000, // 5 seconds - fail faster
});

// Request interceptor for logging (optional)
api.interceptors.request.use(
  (config) => {
    console.log(`API Request: ${config.method?.toUpperCase()} ${config.url}`);
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Silently handle timeout and network errors to avoid blocking UI
    if (error.code === 'ECONNABORTED' || error.code === 'ERR_NETWORK') {
      console.warn('API connection issue:', error.message);
    } else if (error.response) {
      // Server responded with error status
      console.error('API Error:', error.response.status, error.response.data);
    } else if (error.request) {
      // Request made but no response received
      console.warn('Network Error: No response received');
    } else {
      // Error setting up the request
      console.error('Request Error:', error.message);
    }
    return Promise.reject(error);
  }
);

export default api;
