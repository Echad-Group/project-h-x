import React, { useState, useEffect } from 'react';
import api from '../../services/api';

export default function AdminProjects() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [editingProject, setEditingProject] = useState(null);
  const [form, setForm] = useState({
    title: '',
    description: '',
    category: 'campaign',
    status: 'planning',
    priority: 'medium',
    startDate: '',
    endDate: '',
    budget: '',
    assignedTo: '',
    tags: ''
  });

  useEffect(() => {
    loadProjects();
  }, []);

  async function loadProjects() {
    try {
      setLoading(true);
      // TODO: Replace with actual projects endpoint when available
      const mockProjects = [
        {
          id: 1,
          title: 'Youth Mobilization Campaign',
          description: 'Engage young voters across universities',
          category: 'campaign',
          status: 'active',
          priority: 'high',
          startDate: '2025-12-01',
          endDate: '2026-02-28',
          budget: 500000,
          progress: 45,
          assignedTo: 'Communications Unit'
        },
        {
          id: 2,
          title: 'Rural Outreach Initiative',
          description: 'Door-to-door campaigns in rural areas',
          category: 'outreach',
          status: 'active',
          priority: 'high',
          startDate: '2025-11-15',
          endDate: '2026-03-31',
          budget: 750000,
          progress: 60,
          assignedTo: 'Outreach Unit'
        },
        {
          id: 3,
          title: 'Social Media Strategy 2026',
          description: 'Comprehensive social media presence',
          category: 'digital',
          status: 'planning',
          priority: 'medium',
          startDate: '2026-01-01',
          endDate: '2026-12-31',
          budget: 300000,
          progress: 10,
          assignedTo: 'Digital Unit'
        }
      ];
      setProjects(mockProjects);
    } catch (error) {
      console.error('Error loading projects:', error);
    } finally {
      setLoading(false);
    }
  }

  function handleSubmit(e) {
    e.preventDefault();
    // TODO: Implement project creation/update
    console.log('Project form:', form);
    setShowCreateModal(false);
    setEditingProject(null);
  }

  const statusColors = {
    planning: 'bg-gray-100 text-gray-700',
    active: 'bg-green-100 text-green-700',
    paused: 'bg-yellow-100 text-yellow-700',
    completed: 'bg-blue-100 text-blue-700',
    cancelled: 'bg-red-100 text-red-700'
  };

  const priorityColors = {
    low: 'text-gray-600',
    medium: 'text-yellow-600',
    high: 'text-red-600'
  };

  if (loading) {
    return (
      <div className="p-8 flex items-center justify-center">
        <div className="animate-spin text-4xl">↻</div>
      </div>
    );
  }

  return (
    <div className="p-8">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Campaign Projects</h2>
          <p className="text-gray-600">Manage ongoing campaign initiatives</p>
        </div>
        <button
          onClick={() => setShowCreateModal(true)}
          className="fluent-btn fluent-btn-primary"
        >
          <span className="text-xl mr-2">+</span>
          Create Project
        </button>
      </div>

      {/* Filters */}
      <div className="flex gap-4 mb-6">
        <select className="fluent-input">
          <option>All Status</option>
          <option>Planning</option>
          <option>Active</option>
          <option>Completed</option>
        </select>
        <select className="fluent-input">
          <option>All Categories</option>
          <option>Campaign</option>
          <option>Outreach</option>
          <option>Digital</option>
        </select>
        <select className="fluent-input">
          <option>All Priorities</option>
          <option>High</option>
          <option>Medium</option>
          <option>Low</option>
        </select>
      </div>

      {/* Projects Grid */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {projects.map(project => (
          <div key={project.id} className="border rounded-lg p-6 hover:shadow-md transition-shadow bg-white">
            <div className="flex justify-between items-start mb-4">
              <div>
                <h3 className="text-lg font-bold text-gray-900">{project.title}</h3>
                <p className="text-sm text-gray-600">{project.assignedTo}</p>
              </div>
              <div className="flex items-center gap-2">
                <span className={`px-2 py-1 rounded text-xs font-medium ${statusColors[project.status]}`}>
                  {project.status}
                </span>
                <span className={`text-2xl ${priorityColors[project.priority]}`}>
                  {project.priority === 'high' ? '🔴' : project.priority === 'medium' ? '🟡' : '🟢'}
                </span>
              </div>
            </div>

            <p className="text-gray-700 text-sm mb-4">{project.description}</p>

            {/* Progress */}
            <div className="mb-4">
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">Progress</span>
                <span className="font-medium">{project.progress}%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div
                  className="bg-[var(--kenya-green)] h-2 rounded-full transition-all"
                  style={{ width: `${project.progress}%` }}
                ></div>
              </div>
            </div>

            {/* Details */}
            <div className="grid grid-cols-2 gap-4 text-sm mb-4">
              <div>
                <p className="text-gray-600">Start Date</p>
                <p className="font-medium">{new Date(project.startDate).toLocaleDateString()}</p>
              </div>
              <div>
                <p className="text-gray-600">End Date</p>
                <p className="font-medium">{new Date(project.endDate).toLocaleDateString()}</p>
              </div>
              <div>
                <p className="text-gray-600">Budget</p>
                <p className="font-medium">KSh {project.budget.toLocaleString()}</p>
              </div>
              <div>
                <p className="text-gray-600">Category</p>
                <p className="font-medium capitalize">{project.category}</p>
              </div>
            </div>

            {/* Actions */}
            <div className="flex gap-2">
              <button className="flex-1 fluent-btn fluent-btn-secondary text-sm">
                View Details
              </button>
              <button className="flex-1 fluent-btn fluent-btn-ghost text-sm">
                Edit
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Create/Edit Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b">
              <h3 className="text-xl font-bold">Create New Project</h3>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Project Title</label>
                <input
                  type="text"
                  className="fluent-input"
                  value={form.title}
                  onChange={e => setForm({ ...form, title: e.target.value })}
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Description</label>
                <textarea
                  className="fluent-input"
                  rows={4}
                  value={form.description}
                  onChange={e => setForm({ ...form, description: e.target.value })}
                  required
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Category</label>
                  <select
                    className="fluent-input"
                    value={form.category}
                    onChange={e => setForm({ ...form, category: e.target.value })}
                  >
                    <option value="campaign">Campaign</option>
                    <option value="outreach">Outreach</option>
                    <option value="digital">Digital</option>
                    <option value="logistics">Logistics</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Priority</label>
                  <select
                    className="fluent-input"
                    value={form.priority}
                    onChange={e => setForm({ ...form, priority: e.target.value })}
                  >
                    <option value="low">Low</option>
                    <option value="medium">Medium</option>
                    <option value="high">High</option>
                  </select>
                </div>
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Start Date</label>
                  <input
                    type="date"
                    className="fluent-input"
                    value={form.startDate}
                    onChange={e => setForm({ ...form, startDate: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">End Date</label>
                  <input
                    type="date"
                    className="fluent-input"
                    value={form.endDate}
                    onChange={e => setForm({ ...form, endDate: e.target.value })}
                    required
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Budget (KSh)</label>
                <input
                  type="number"
                  className="fluent-input"
                  value={form.budget}
                  onChange={e => setForm({ ...form, budget: e.target.value })}
                />
              </div>
              <div className="flex gap-3 pt-4">
                <button type="submit" className="flex-1 fluent-btn fluent-btn-primary">
                  Create Project
                </button>
                <button
                  type="button"
                  onClick={() => setShowCreateModal(false)}
                  className="flex-1 fluent-btn fluent-btn-ghost"
                >
                  Cancel
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
