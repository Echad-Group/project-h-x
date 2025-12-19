import api from './api';

const NEWS_ENDPOINTS = {
  base: '/news',
  featured: '/news/featured',
  bySlug: (slug) => `/news/${slug}`,
  byId: (id) => `/news/${id}`,
  incrementView: (id) => `/news/${id}/increment-view`,
  categories: '/news/categories',
  tags: '/news/tags',
};

/**
 * Get all articles with filtering, searching, sorting and pagination
 * @param {Object} params - Query parameters
 * @param {string} params.category - Filter by category (optional)
 * @param {string} params.search - Search query (optional)
 * @param {string} params.status - Filter by status (default: 'published')
 * @param {string} params.sortBy - Sort order: 'newest', 'oldest', 'popular' (default: 'newest')
 * @param {number} params.page - Page number (default: 1)
 * @param {number} params.pageSize - Items per page (default: 12)
 * @returns {Promise<{articles: Array, totalCount: number, page: number, pageSize: number}>}
 */
export const getArticles = async (params = {}) => {
  try {
    const {
      category = null,
      search = null,
      status = 'published',
      sortBy = 'newest',
      page = 1,
      pageSize = 12,
    } = params;

    const queryParams = new URLSearchParams();
    if (category && category !== 'all') queryParams.append('category', category);
    if (search) queryParams.append('search', search);
    if (status) queryParams.append('status', status);
    if (sortBy) queryParams.append('sortBy', sortBy);
    queryParams.append('page', page);
    queryParams.append('pageSize', pageSize);

    const response = await api.get(`${NEWS_ENDPOINTS.base}?${queryParams.toString()}`);

    return {
      articles: response.data,
      totalCount: parseInt(response.headers['x-total-count'] || '0'),
      page: parseInt(response.headers['x-page'] || '1'),
      pageSize: parseInt(response.headers['x-page-size'] || '12'),
    };
  } catch (error) {
    console.error('Error fetching articles:', error);
    throw error;
  }
};

/**
 * Get featured articles
 * @param {number} limit - Number of articles to fetch (default: 4)
 * @returns {Promise<Array>}
 */
export const getFeaturedArticles = async (limit = 4) => {
  try {
    const response = await api.get(`${NEWS_ENDPOINTS.featured}?limit=${limit}`);
    return response.data;
  } catch (error) {
    console.error('Error fetching featured articles:', error);
    throw error;
  }
};

/**
 * Get a single article by slug
 * @param {string} slug - Article slug
 * @returns {Promise<Object>}
 */
export const getArticleBySlug = async (slug) => {
  try {
    const response = await api.get(NEWS_ENDPOINTS.bySlug(slug));
    return response.data;
  } catch (error) {
    console.error(`Error fetching article with slug ${slug}:`, error);
    throw error;
  }
};

/**
 * Increment view count for an article
 * @param {string} id - Article ID
 * @returns {Promise<{views: number}>}
 */
export const incrementArticleViews = async (id) => {
  try {
    const response = await api.post(NEWS_ENDPOINTS.incrementView(id));
    return response.data;
  } catch (error) {
    console.error(`Error incrementing views for article ${id}:`, error);
    // Don't throw - view tracking is not critical
    return { views: 0 };
  }
};

/**
 * Create a new article (Admin only)
 * @param {Object} articleData - Article data
 * @returns {Promise<Object>}
 */
export const createArticle = async (articleData) => {
  try {
    const response = await api.post(NEWS_ENDPOINTS.base, articleData);
    return response.data;
  } catch (error) {
    console.error('Error creating article:', error);
    throw error;
  }
};

/**
 * Update an existing article (Admin only)
 * @param {string} id - Article ID
 * @param {Object} updates - Fields to update
 * @returns {Promise<void>}
 */
export const updateArticle = async (id, updates) => {
  try {
    await api.put(NEWS_ENDPOINTS.byId(id), updates);
  } catch (error) {
    console.error(`Error updating article ${id}:`, error);
    throw error;
  }
};

/**
 * Delete an article (Admin only)
 * @param {string} id - Article ID
 * @returns {Promise<void>}
 */
export const deleteArticle = async (id) => {
  try {
    await api.delete(NEWS_ENDPOINTS.byId(id));
  } catch (error) {
    console.error(`Error deleting article ${id}:`, error);
    throw error;
  }
};

/**
 * Get all available categories
 * @returns {Promise<Array<string>>}
 */
export const getCategories = async () => {
  try {
    const response = await api.get(NEWS_ENDPOINTS.categories);
    return response.data;
  } catch (error) {
    console.error('Error fetching categories:', error);
    throw error;
  }
};

/**
 * Get all available tags
 * @returns {Promise<Array<string>>}
 */
export const getTags = async () => {
  try {
    const response = await api.get(NEWS_ENDPOINTS.tags);
    return response.data;
  } catch (error) {
    console.error('Error fetching tags:', error);
    throw error;
  }
};

export default {
  getArticles,
  getFeaturedArticles,
  getArticleBySlug,
  incrementArticleViews,
  createArticle,
  updateArticle,
  deleteArticle,
  getCategories,
  getTags,
};
