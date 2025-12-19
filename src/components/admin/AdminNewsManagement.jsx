import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import newsService from '../../services/newsService';

export default function AdminNewsManagement() {
  const navigate = useNavigate();
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedStatus, setSelectedStatus] = useState('all');
  const [selectedCategory, setSelectedCategory] = useState('all');
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [articleToDelete, setArticleToDelete] = useState(null);
  const [deleting, setDeleting] = useState(false);

  const pageSize = 10;
  const totalPages = Math.ceil(totalCount / pageSize);

  const statusOptions = [
    { value: 'all', label: 'All Status', color: 'gray' },
    { value: 'draft', label: 'Draft', color: 'yellow' },
    { value: 'published', label: 'Published', color: 'green' },
    { value: 'archived', label: 'Archived', color: 'red' }
  ];

  const categoryOptions = [
    { value: 'all', label: 'All Categories' },
    { value: 'policy', label: 'Policy' },
    { value: 'events', label: 'Events' },
    { value: 'press', label: 'Press Release' },
    { value: 'community', label: 'Community' }
  ];

  useEffect(() => {
    fetchArticles();
  }, [selectedStatus, selectedCategory, searchQuery, currentPage]);

  const fetchArticles = async () => {
    try {
      setLoading(true);
      setError(null);

      const params = {
        page: currentPage,
        pageSize: pageSize,
        sortBy: 'newest'
      };

      if (selectedStatus !== 'all') {
        params.status = selectedStatus;
      }
      if (selectedCategory !== 'all') {
        params.category = selectedCategory;
      }
      if (searchQuery.trim()) {
        params.search = searchQuery.trim();
      }

      const response = await newsService.getArticles(params);
      setArticles(response.articles);
      setTotalCount(response.totalCount);
    } catch (err) {
      console.error('Failed to fetch articles:', err);
      setError('Failed to load articles. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => {
    navigate('/admin/news/create');
  };

  const handleEdit = (articleId) => {
    navigate(`/admin/news/edit/${articleId}`);
  };

  const handleDeleteClick = (article) => {
    setArticleToDelete(article);
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    if (!articleToDelete) return;

    try {
      setDeleting(true);
      await newsService.deleteArticle(articleToDelete.id);
      
      // Refresh the list
      await fetchArticles();
      
      setShowDeleteModal(false);
      setArticleToDelete(null);
    } catch (err) {
      console.error('Failed to delete article:', err);
      alert('Failed to delete article. Please try again.');
    } finally {
      setDeleting(false);
    }
  };

  const handleStatusChange = async (articleId, newStatus) => {
    try {
      await newsService.updateArticle(articleId, { status: newStatus });
      // Refresh the list
      await fetchArticles();
    } catch (err) {
      console.error('Failed to update article status:', err);
      alert('Failed to update article status. Please try again.');
    }
  };

  const handleToggleFeatured = async (article) => {
    try {
      await newsService.updateArticle(article.id, { isFeatured: !article.isFeatured });
      // Refresh the list
      await fetchArticles();
    } catch (err) {
      console.error('Failed to toggle featured status:', err);
      alert('Failed to update featured status. Please try again.');
    }
  };

  const getStatusBadge = (status) => {
    const statusConfig = statusOptions.find(s => s.value === status) || statusOptions[0];
    const colorClasses = {
      gray: 'bg-gray-100 text-gray-800',
      yellow: 'bg-yellow-100 text-yellow-800',
      green: 'bg-green-100 text-green-800',
      red: 'bg-red-100 text-red-800'
    };

    return (
      <span className={`px-2 py-1 text-xs font-medium rounded-full ${colorClasses[statusConfig.color]}`}>
        {statusConfig.label}
      </span>
    );
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  return (
    <div className="p-6">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">News Management</h2>
          <p className="text-gray-600 mt-1">Create and manage news articles</p>
        </div>
        <button
          onClick={handleCreateNew}
          className="bg-[var(--kenya-green)] text-white px-6 py-2 rounded-lg hover:bg-[var(--kenya-red)] transition-colors flex items-center gap-2"
        >
          <span className="text-xl">+</span>
          Create Article
        </button>
      </div>

      {/* Filters */}
      <div className="bg-white rounded-lg shadow-sm p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          {/* Search */}
          <div className="md:col-span-2">
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Search
            </label>
            <input
              type="text"
              placeholder="Search by title or content..."
              value={searchQuery}
              onChange={(e) => {
                setSearchQuery(e.target.value);
                setCurrentPage(1);
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
            />
          </div>

          {/* Status Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Status
            </label>
            <select
              value={selectedStatus}
              onChange={(e) => {
                setSelectedStatus(e.target.value);
                setCurrentPage(1);
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
            >
              {statusOptions.map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>

          {/* Category Filter */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
              Category
            </label>
            <select
              value={selectedCategory}
              onChange={(e) => {
                setSelectedCategory(e.target.value);
                setCurrentPage(1);
              }}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
            >
              {categoryOptions.map(option => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* Results count */}
        <div className="mt-4 text-sm text-gray-600">
          Showing {articles.length} of {totalCount} articles
        </div>
      </div>

      {/* Loading State */}
      {loading && (
        <div className="flex justify-center items-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-4 border-gray-200 border-t-[var(--kenya-green)]"></div>
        </div>
      )}

      {/* Error State */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-red-800">
          {error}
        </div>
      )}

      {/* Articles Table */}
      {!loading && !error && (
        <div className="bg-white rounded-lg shadow-sm overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Article
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Category
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Views
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Published
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {articles.length === 0 ? (
                <tr>
                  <td colSpan="6" className="px-6 py-8 text-center text-gray-500">
                    No articles found. Create your first article to get started.
                  </td>
                </tr>
              ) : (
                articles.map(article => (
                  <tr key={article.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4">
                      <div className="flex items-center">
                        {article.featuredImageUrl && (
                          <img
                            src={article.featuredImageUrl}
                            alt={article.title}
                            className="h-12 w-12 rounded object-cover mr-3"
                          />
                        )}
                        <div className="flex-1">
                          <div className="text-sm font-medium text-gray-900 flex items-center gap-2">
                            {article.title}
                            {article.isFeatured && (
                              <span className="text-yellow-500" title="Featured">⭐</span>
                            )}
                          </div>
                          <div className="text-xs text-gray-500 mt-1">
                            {article.excerpt?.substring(0, 60)}...
                          </div>
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="text-sm text-gray-900 capitalize">
                        {article.category}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {getStatusBadge(article.status)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {article.views?.toLocaleString() || 0}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {article.publishedDate ? formatDate(article.publishedDate) : 'Not published'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <div className="flex justify-end gap-2">
                        {/* Featured Toggle */}
                        <button
                          onClick={() => handleToggleFeatured(article)}
                          className={`p-2 rounded hover:bg-gray-100 ${article.isFeatured ? 'text-yellow-500' : 'text-gray-400'}`}
                          title={article.isFeatured ? 'Remove from featured' : 'Mark as featured'}
                        >
                          ⭐
                        </button>

                        {/* Status Dropdown */}
                        <select
                          value={article.status}
                          onChange={(e) => handleStatusChange(article.id, e.target.value)}
                          className="text-sm border border-gray-300 rounded px-2 py-1 focus:ring-2 focus:ring-[var(--kenya-green)] focus:border-transparent"
                        >
                          <option value="draft">Draft</option>
                          <option value="published">Published</option>
                          <option value="archived">Archived</option>
                        </select>

                        {/* Edit Button */}
                        <button
                          onClick={() => handleEdit(article.id)}
                          className="text-[var(--kenya-green)] hover:text-[var(--kenya-red)] p-2"
                          title="Edit article"
                        >
                          ✏️
                        </button>

                        {/* Delete Button */}
                        <button
                          onClick={() => handleDeleteClick(article)}
                          className="text-red-600 hover:text-red-800 p-2"
                          title="Delete article"
                        >
                          🗑️
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Pagination */}
      {!loading && !error && totalPages > 1 && (
        <div className="mt-6 flex justify-center items-center gap-2">
          <button
            onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
            disabled={currentPage === 1}
            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Previous
          </button>
          
          <div className="flex gap-2">
            {Array.from({ length: totalPages }, (_, i) => i + 1).map(page => (
              <button
                key={page}
                onClick={() => setCurrentPage(page)}
                className={`px-4 py-2 rounded-lg ${
                  currentPage === page
                    ? 'bg-[var(--kenya-green)] text-white'
                    : 'border border-gray-300 hover:bg-gray-50'
                }`}
              >
                {page}
              </button>
            ))}
          </div>

          <button
            onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
            disabled={currentPage === totalPages}
            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Next
          </button>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {showDeleteModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 max-w-md w-full mx-4">
            <h3 className="text-xl font-bold text-gray-900 mb-4">Delete Article</h3>
            <p className="text-gray-600 mb-6">
              Are you sure you want to delete "{articleToDelete?.title}"? This action cannot be undone.
            </p>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => {
                  setShowDeleteModal(false);
                  setArticleToDelete(null);
                }}
                disabled={deleting}
                className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50"
              >
                Cancel
              </button>
              <button
                onClick={handleDeleteConfirm}
                disabled={deleting}
                className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50"
              >
                {deleting ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
