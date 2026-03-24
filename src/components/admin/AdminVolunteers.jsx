import React, { useState, useEffect, useCallback } from 'react';
import { volunteerService } from '../../services/volunteerService';
import { assignmentsService, unitsService, teamsService } from '../../services/organizationService';

export default function AdminVolunteers() {
  const [volunteers, setVolunteers] = useState([]);
  const [units, setUnits] = useState([]);
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 25;
  const [filters, setFilters] = useState({
    search: '',
    unit: '',
    team: '',
    skills: '',
    region: ''
  });
  const [debouncedSearch, setDebouncedSearch] = useState('');
  const [selectedVolunteers, setSelectedVolunteers] = useState([]);
  const [showAssignModal, setShowAssignModal] = useState(false);
  const [assignForm, setAssignForm] = useState({
    unitId: '',
    teamId: '',
    notes: ''
  });

  useEffect(() => {
    loadData();
  }, []);

  // Debounce the search field 350 ms before triggering a fetch
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(filters.search), 350);
    return () => clearTimeout(timer);
  }, [filters.search]);

  // Re-fetch whenever page or server-side filters change
  useEffect(() => {
    loadVolunteers();
  }, [currentPage, debouncedSearch, filters.region, filters.skills]);

  async function loadData() {
    try {
      setLoading(true);
      setError('');
      const [unitsData, teamsData] = await Promise.all([
        unitsService.getAll(),
        teamsService.getAll()
      ]);
      setUnits(unitsData);
      setTeams(teamsData);
    } catch (error) {
      console.error('Error loading org data:', error);
      setError('Failed to load org data. Please try again.');
    } finally {
      setLoading(false);
    }
    // Volunteers are loaded by the separate useEffect below
  }

  const loadVolunteers = useCallback(async () => {
    try {
      setLoading(true);
      setError('');
      const result = await volunteerService.getPaged({
        page: currentPage,
        pageSize,
        search: debouncedSearch || undefined,
        region: filters.region || undefined,
        skills: filters.skills || undefined,
      });
      setVolunteers(result.volunteers ?? []);
      setTotalCount(result.totalCount ?? 0);
    } catch (error) {
      console.error('Error loading volunteers:', error);
      setError('Failed to load volunteers. Please try again.');
    } finally {
      setLoading(false);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentPage, debouncedSearch, filters.region, filters.skills]);

  async function handleBulkAssign() {
    try {
      await assignmentsService.bulkAssign({
        volunteerIds: selectedVolunteers,
        unitId: parseInt(assignForm.unitId),
        teamId: assignForm.teamId ? parseInt(assignForm.teamId) : null,
        notes: assignForm.notes
      });
      setShowAssignModal(false);
      setSelectedVolunteers([]);
      setAssignForm({ unitId: '', teamId: '', notes: '' });
      setNotice(`Successfully assigned ${selectedVolunteers.length} volunteer${selectedVolunteers.length !== 1 ? 's' : ''}.`);
      setTimeout(() => setNotice(''), 4000);
    } catch (error) {
      console.error('Error assigning volunteers:', error);
      setError(error.response?.data?.message || 'Failed to assign volunteers. Please try again.');
    }
  }

  function exportToCSV() {
    const headers = ['Name', 'Email', 'Phone', 'Region', 'City', 'Skills', 'Hours/Week'];
    const rows = filteredVolunteers.map(v => [
      v.name,
      v.email,
      v.phone || '',
      v.region || '',
      v.city || '',
      v.skills || '',
      v.hoursPerWeek || ''
    ]);
    
    const csv = [headers, ...rows].map(row => row.join(',')).join('\n');
    const blob = new Blob([csv], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `volunteers_${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
  }

  const filteredVolunteers = volunteers; // served from API, already filtered
  const totalPages = Math.ceil(totalCount / pageSize);

  // Reset page when filter values change (except pagination itself)
  function applyFilter(patch) {
    setCurrentPage(1);
    setFilters((prev) => ({ ...prev, ...patch }));
  }

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
          <h2 className="text-2xl font-bold text-gray-900">Volunteer Management</h2>
          <p className="text-gray-600">Showing {volunteers.length} of {totalCount} volunteers</p>
        </div>
        <div className="flex gap-2">
          <button onClick={() => { setError(''); loadData(); loadVolunteers(); }} className="fluent-btn fluent-btn-ghost text-sm" title="Refresh">
            ↻ Refresh
          </button>
          <button
            onClick={exportToCSV}
            className="fluent-btn fluent-btn-secondary"
            disabled={filteredVolunteers.length === 0}
          >
            📊 Export CSV
          </button>
          {selectedVolunteers.length > 0 && (
            <button
              onClick={() => setShowAssignModal(true)}
              className="fluent-btn fluent-btn-primary"
            >
              Assign {selectedVolunteers.length} Selected
            </button>
          )}
        </div>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-800 px-4 py-3 rounded mb-4 flex items-center justify-between">
          <span>{error}</span>
          <button onClick={() => { setError(''); loadData(); loadVolunteers(); }} className="ml-4 text-sm underline hover:no-underline shrink-0">Retry</button>
        </div>
      )}

      {notice && (
        <div className="bg-green-50 border border-green-200 text-green-800 px-4 py-3 rounded mb-4">
          {notice}
        </div>
      )}

      {/* Filters */}
      <div className="bg-white border rounded-lg p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <input
            type="text"
            placeholder="Search by name or email..."
            className="fluent-input"
            value={filters.search}
            onChange={e => applyFilter({ search: e.target.value })}
          />
          <input
            type="text"
            placeholder="Filter by region..."
            className="fluent-input"
            value={filters.region}
            onChange={e => applyFilter({ region: e.target.value })}
          />
          <input
            type="text"
            placeholder="Filter by skill..."
            className="fluent-input"
            value={filters.skills}
            onChange={e => applyFilter({ skills: e.target.value })}
          />
          <button
            onClick={() => { setCurrentPage(1); setFilters({ search: '', unit: '', team: '', skills: '', region: '' }); }}
            className="fluent-btn fluent-btn-ghost"
          >
            Clear Filters
          </button>
        </div>
      </div>

      {/* Volunteers Table */}
      <div className="bg-white border rounded-lg overflow-hidden">
        <table className="w-full">
          <thead className="bg-gray-50 border-b">
            <tr>
              <th className="px-4 py-3 text-left">
                <input
                  type="checkbox"
                  onChange={e => {
                    if (e.target.checked) {
                      setSelectedVolunteers(filteredVolunteers.map(v => v.id));
                    } else {
                      setSelectedVolunteers([]);
                    }
                  }}
                  checked={selectedVolunteers.length === filteredVolunteers.length && filteredVolunteers.length > 0}
                />
              </th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Name</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Email</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Phone</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Location</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Skills</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Availability</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Joined</th>
              <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y">
            {filteredVolunteers.map(volunteer => (
              <tr key={volunteer.id} className="hover:bg-gray-50">
                <td className="px-4 py-3">
                  <input
                    type="checkbox"
                    checked={selectedVolunteers.includes(volunteer.id)}
                    onChange={e => {
                      if (e.target.checked) {
                        setSelectedVolunteers([...selectedVolunteers, volunteer.id]);
                      } else {
                        setSelectedVolunteers(selectedVolunteers.filter(id => id !== volunteer.id));
                      }
                    }}
                  />
                </td>
                <td className="px-4 py-3">
                  <div className="font-medium text-gray-900">{volunteer.name}</div>
                </td>
                <td className="px-4 py-3 text-sm text-gray-600">{volunteer.email}</td>
                <td className="px-4 py-3 text-sm text-gray-600">{volunteer.phone || '-'}</td>
                <td className="px-4 py-3 text-sm text-gray-600">
                  {volunteer.city && volunteer.region ? `${volunteer.city}, ${volunteer.region}` : volunteer.region || volunteer.city || '-'}
                </td>
                <td className="px-4 py-3">
                  <div className="flex flex-wrap gap-1 max-w-xs">
                    {volunteer.skills ? volunteer.skills.split(',').slice(0, 2).map((skill, idx) => (
                      <span key={idx} className="px-2 py-1 bg-blue-100 text-blue-700 text-xs rounded">
                        {skill.trim()}
                      </span>
                    )) : '-'}
                  </div>
                </td>
                <td className="px-4 py-3 text-sm text-gray-600">
                  {volunteer.hoursPerWeek ? `${volunteer.hoursPerWeek}h/week` : '-'}
                </td>
                <td className="px-4 py-3 text-sm text-gray-600">
                  {new Date(volunteer.createdAt).toLocaleDateString()}
                </td>
                <td className="px-4 py-3">
                  <button className="text-[var(--kenya-green)] hover:underline text-sm font-medium">
                    View
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="mt-4 flex items-center justify-between">
          <p className="text-sm text-gray-600">
            Page {currentPage} of {totalPages} &bull; {totalCount} total volunteers
          </p>
          <div className="flex gap-2">
            <button
              onClick={() => setCurrentPage((p) => Math.max(1, p - 1))}
              disabled={currentPage === 1}
              className="px-3 py-1.5 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-40 text-sm"
            >
              ← Prev
            </button>
            {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
              const start = Math.max(1, currentPage - 2);
              const pg = start + i;
              if (pg > totalPages) return null;
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
              onClick={() => setCurrentPage((p) => Math.min(totalPages, p + 1))}
              disabled={currentPage === totalPages}
              className="px-3 py-1.5 border border-gray-300 rounded hover:bg-gray-50 disabled:opacity-40 text-sm"
            >
              Next →
            </button>
          </div>
        </div>
      )}

      {/* Bulk Assign Modal */}
      {showAssignModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-md w-full">
            <div className="p-6 border-b">
              <h3 className="text-xl font-bold">Assign {selectedVolunteers.length} Volunteers</h3>
            </div>
            <div className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Unit *</label>
                <select
                  className="fluent-input"
                  value={assignForm.unitId}
                  onChange={e => setAssignForm({ ...assignForm, unitId: e.target.value, teamId: '' })}
                  required
                >
                  <option value="">Select Unit</option>
                  {units.map(u => (
                    <option key={u.id} value={u.id}>{u.name}</option>
                  ))}
                </select>
              </div>
              {assignForm.unitId && (
                <div>
                  <label className="block text-sm font-medium mb-1">Team (Optional)</label>
                  <select
                    className="fluent-input"
                    value={assignForm.teamId}
                    onChange={e => setAssignForm({ ...assignForm, teamId: e.target.value })}
                  >
                    <option value="">No specific team</option>
                    {teams.filter(t => t.unitId === parseInt(assignForm.unitId)).map(t => (
                      <option key={t.id} value={t.id}>{t.name}</option>
                    ))}
                  </select>
                </div>
              )}
              <div>
                <label className="block text-sm font-medium mb-1">Notes</label>
                <textarea
                  className="fluent-input"
                  rows={3}
                  value={assignForm.notes}
                  onChange={e => setAssignForm({ ...assignForm, notes: e.target.value })}
                  placeholder="Assignment notes..."
                />
              </div>
              <div className="flex gap-3 pt-4">
                <button
                  onClick={handleBulkAssign}
                  className="flex-1 fluent-btn fluent-btn-primary"
                  disabled={!assignForm.unitId}
                >
                  Assign Volunteers
                </button>
                <button
                  onClick={() => {
                    setShowAssignModal(false);
                    setAssignForm({ unitId: '', teamId: '', notes: '' });
                  }}
                  className="flex-1 fluent-btn fluent-btn-ghost"
                >
                  Cancel
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
