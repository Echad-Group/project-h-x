import React, { useEffect, useState } from 'react';
import { campaignTasksService } from '../../services/campaignCommandService';

const initialForm = {
  title: '',
  description: '',
  status: 'Pending',
  priority: 'Medium',
  deadline: '',
  location: '',
  region: '',
  county: '',
  constituency: '',
  ward: ''
};

function formatDate(value) {
  if (!value) {
    return '-';
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleDateString();
}

export default function AdminProjects() {
  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');
  const [search, setSearch] = useState('');
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [priorityFilter, setPriorityFilter] = useState('all');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 25;
  const [showModal, setShowModal] = useState(false);
  const [editingProject, setEditingProject] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [form, setForm] = useState(initialForm);

  // Debounce search
  useEffect(() => {
    const t = setTimeout(() => setDebouncedSearch(search), 350);
    return () => clearTimeout(t);
  }, [search]);

  const loadProjects = async () => {
    try {
      setLoading(true);
      setError('');

      const params = {
        page: currentPage,
        pageSize,
        search: debouncedSearch || undefined,
        status: statusFilter !== 'all' ? statusFilter : undefined,
        priority: priorityFilter !== 'all' ? priorityFilter : undefined
      };

      const data = await campaignTasksService.getManage(params);
      if (Array.isArray(data)) {
        setProjects(data);
        setTotalCount(data.length);
      } else {
        setProjects(data.tasks ?? []);
        setTotalCount(data.totalCount ?? 0);
      }
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load projects.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    setCurrentPage(1);
  }, [statusFilter, priorityFilter, debouncedSearch]);

  useEffect(() => {
    loadProjects();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage, statusFilter, priorityFilter, debouncedSearch]);

  const filteredProjects = projects; // already filtered server-side

  const openCreateModal = () => {
    setEditingProject(null);
    setForm(initialForm);
    setShowModal(true);
    setError('');
  };

  const openEditModal = (project) => {
    setEditingProject(project);
    setForm({
      title: project.title || '',
      description: project.description || '',
      status: project.status || 'Pending',
      priority: project.priority || 'Medium',
      deadline: project.dueDate ? new Date(project.dueDate).toISOString().slice(0, 16) : '',
      location: project.location || '',
      region: project.region || '',
      county: project.county || '',
      constituency: project.constituency || '',
      ward: project.ward || ''
    });
    setShowModal(true);
    setError('');
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingProject(null);
    setForm(initialForm);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setSubmitting(true);
    setError('');
    setNotice('');

    try {
      if (!form.deadline) {
        throw new Error('Deadline is required.');
      }

      if (editingProject) {
        await campaignTasksService.update(editingProject.id, {
          title: form.title,
          description: form.description,
          status: form.status,
          priority: form.priority,
          deadline: new Date(form.deadline).toISOString(),
          location: form.location,
          region: form.region,
          county: form.county,
          constituency: form.constituency,
          ward: form.ward
        });
        setNotice('Project updated successfully.');
      } else {
        await campaignTasksService.create({
          title: form.title,
          description: form.description,
          deadline: new Date(form.deadline).toISOString(),
          priority: form.priority,
          location: form.location,
          region: form.region,
          county: form.county,
          constituency: form.constituency,
          ward: form.ward
        });
        setNotice('Project created successfully.');
      }

      closeModal();
      await loadProjects();
    } catch (err) {
      setError(err.response?.data?.message || err.message || 'Failed to save project.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDelete = async (projectId) => {
    const confirmed = window.confirm('Delete this project? This cannot be undone.');
    if (!confirmed) {
      return;
    }

    try {
      setError('');
      setNotice('');
      await campaignTasksService.remove(projectId);
      setNotice('Project deleted successfully.');
      await loadProjects();
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete project.');
    }
  };

  const statusPill = (status) => {
    const normalized = (status || '').toLowerCase();
    if (normalized === 'completed') return 'bg-emerald-100 text-emerald-800';
    if (normalized === 'in progress') return 'bg-blue-100 text-blue-800';
    if (normalized === 'cancelled') return 'bg-red-100 text-red-800';
    return 'bg-amber-100 text-amber-800';
  };

  return (
    <div className="p-8">
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Campaign Projects</h2>
          <p className="text-gray-600">API-backed project/task planning and execution</p>
        </div>
        <button onClick={openCreateModal} className="fluent-btn fluent-btn-primary">Create Project</button>
      </div>

      {notice && <div className="mb-4 rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{notice}</div>}
      {error && <div className="mb-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}

      <div className="grid grid-cols-1 md:grid-cols-4 gap-3 mb-6">
        <input
          className="fluent-input"
          placeholder="Search title, description, region, county"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
        <select className="fluent-input" value={statusFilter} onChange={(event) => setStatusFilter(event.target.value)}>
          <option value="all">All status</option>
          <option value="Pending">Pending</option>
          <option value="In Progress">In Progress</option>
          <option value="Completed">Completed</option>
          <option value="Cancelled">Cancelled</option>
        </select>
        <select className="fluent-input" value={priorityFilter} onChange={(event) => setPriorityFilter(event.target.value)}>
          <option value="all">All priority</option>
          <option value="Low">Low</option>
          <option value="Medium">Medium</option>
          <option value="High">High</option>
          <option value="Urgent">Urgent</option>
        </select>
        <button className="fluent-btn fluent-btn-ghost" onClick={loadProjects} disabled={loading}>{loading ? 'Refreshing...' : 'Refresh'}</button>
      </div>

      {loading ? (
        <div className="p-8 flex items-center justify-center"><div className="animate-spin text-4xl">↻</div></div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {filteredProjects.map((project) => (
            <div key={project.id} className="rounded-lg border bg-white p-5 shadow-sm">
              <div className="mb-3 flex items-start justify-between gap-3">
                <div>
                  <h3 className="text-lg font-semibold text-gray-900">{project.title}</h3>
                  <p className="text-sm text-gray-600">Due: {formatDate(project.dueDate)}</p>
                </div>
                <span className={`rounded-full px-2 py-1 text-xs font-medium ${statusPill(project.status)}`}>{project.status || 'Pending'}</span>
              </div>

              <p className="mb-3 text-sm text-gray-700 line-clamp-3">{project.description}</p>

              <div className="grid grid-cols-2 gap-2 text-sm mb-4">
                <div>
                  <p className="text-gray-500">Priority</p>
                  <p className="font-medium">{project.priority || '-'}</p>
                </div>
                <div>
                  <p className="text-gray-500">Region</p>
                  <p className="font-medium">{project.region || '-'}</p>
                </div>
                <div>
                  <p className="text-gray-500">County</p>
                  <p className="font-medium">{project.county || '-'}</p>
                </div>
                <div>
                  <p className="text-gray-500">Created</p>
                  <p className="font-medium">{formatDate(project.createdAt)}</p>
                </div>
              </div>

              <div className="flex gap-2">
                <button className="fluent-btn fluent-btn-secondary flex-1" onClick={() => openEditModal(project)}>Edit</button>
                <button className="fluent-btn fluent-btn-ghost text-red-600 hover:bg-red-50" onClick={() => handleDelete(project.id)}>Delete</button>
              </div>
            </div>
          ))}
          {!filteredProjects.length && <div className="text-sm text-gray-500">No projects found with the current filters.</div>}
        </div>
      )}

      {/* Pagination */}
      {!loading && Math.ceil(totalCount / pageSize) > 1 && (
        <div className="mt-6 flex items-center justify-between">
          <p className="text-sm text-gray-600">
            Page {currentPage} of {Math.ceil(totalCount / pageSize)} &bull; {totalCount} total projects
          </p>
          <div className="flex gap-2">
            <button
              onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
              disabled={currentPage === 1}
              className="px-3 py-1.5 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-40 text-sm"
            >
              ← Prev
            </button>
            {Array.from({ length: Math.min(5, Math.ceil(totalCount / pageSize)) }, (_, i) => {
              const start = Math.max(1, currentPage - 2);
              const pg = start + i;
              if (pg > Math.ceil(totalCount / pageSize)) return null;
              return (
                <button
                  key={pg}
                  onClick={() => setCurrentPage(pg)}
                  className={`px-3 py-1.5 border rounded text-sm ${currentPage === pg ? 'bg-[var(--kenya-green)] text-white border-[var(--kenya-green)]' : 'border-gray-300 hover:bg-gray-50'}`}
                >
                  {pg}
                </button>
              );
            })}
            <button
              onClick={() => setCurrentPage((p) => Math.min(Math.ceil(totalCount / pageSize), p + 1))}
              disabled={currentPage === Math.ceil(totalCount / pageSize)}
              className="px-3 py-1.5 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-40 text-sm"
            >
              Next →
            </button>
          </div>
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="max-h-[90vh] w-full max-w-2xl overflow-y-auto rounded-lg bg-white">
            <div className="border-b p-6">
              <h3 className="text-xl font-bold">{editingProject ? 'Edit Project' : 'Create Project'}</h3>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Title</label>
                <input className="fluent-input" value={form.title} onChange={(event) => setForm((current) => ({ ...current, title: event.target.value }))} required />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Description</label>
                <textarea className="fluent-input" rows={5} value={form.description} onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))} required />
              </div>

              <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
                <div>
                  <label className="mb-1 block text-sm font-medium">Status</label>
                  <select className="fluent-input" value={form.status} onChange={(event) => setForm((current) => ({ ...current, status: event.target.value }))}>
                    <option value="Pending">Pending</option>
                    <option value="In Progress">In Progress</option>
                    <option value="Completed">Completed</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium">Priority</label>
                  <select className="fluent-input" value={form.priority} onChange={(event) => setForm((current) => ({ ...current, priority: event.target.value }))}>
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                    <option value="Urgent">Urgent</option>
                  </select>
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium">Deadline</label>
                  <input type="datetime-local" className="fluent-input" value={form.deadline} onChange={(event) => setForm((current) => ({ ...current, deadline: event.target.value }))} required />
                </div>
              </div>

              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <label className="mb-1 block text-sm font-medium">Region</label>
                  <input className="fluent-input" value={form.region} onChange={(event) => setForm((current) => ({ ...current, region: event.target.value }))} />
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium">County</label>
                  <input className="fluent-input" value={form.county} onChange={(event) => setForm((current) => ({ ...current, county: event.target.value }))} />
                </div>
              </div>

              <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                  <label className="mb-1 block text-sm font-medium">Constituency</label>
                  <input className="fluent-input" value={form.constituency} onChange={(event) => setForm((current) => ({ ...current, constituency: event.target.value }))} />
                </div>
                <div>
                  <label className="mb-1 block text-sm font-medium">Ward</label>
                  <input className="fluent-input" value={form.ward} onChange={(event) => setForm((current) => ({ ...current, ward: event.target.value }))} />
                </div>
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Location Notes</label>
                <input className="fluent-input" value={form.location} onChange={(event) => setForm((current) => ({ ...current, location: event.target.value }))} />
              </div>

              <div className="flex gap-3 pt-2">
                <button type="submit" className="fluent-btn fluent-btn-primary flex-1" disabled={submitting}>{submitting ? 'Saving...' : editingProject ? 'Update Project' : 'Create Project'}</button>
                <button type="button" className="fluent-btn fluent-btn-ghost flex-1" onClick={closeModal} disabled={submitting}>Cancel</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
