import React, { useState, useEffect } from 'react';
import { volunteerService } from '../../services/volunteerService';
import { assignmentsService, unitsService, teamsService } from '../../services/organizationService';

export default function AdminVolunteers() {
  const [volunteers, setVolunteers] = useState([]);
  const [units, setUnits] = useState([]);
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [filters, setFilters] = useState({
    search: '',
    unit: '',
    team: '',
    skills: '',
    region: ''
  });
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

  async function loadData() {
    try {
      setLoading(true);
      const [volunteersData, unitsData, teamsData] = await Promise.all([
        volunteerService.getAll(),
        unitsService.getAll(),
        teamsService.getAll()
      ]);
      setVolunteers(volunteersData);
      setUnits(unitsData);
      setTeams(teamsData);
    } catch (error) {
      console.error('Error loading volunteers:', error);
    } finally {
      setLoading(false);
    }
  }

  async function handleBulkAssign() {
    try {
      await assignmentsService.bulkAssign({
        volunteerIds: selectedVolunteers,
        unitId: parseInt(assignForm.unitId),
        teamId: assignForm.teamId ? parseInt(assignForm.teamId) : null,
        notes: assignForm.notes
      });
      alert(`Successfully assigned ${selectedVolunteers.length} volunteers`);
      setShowAssignModal(false);
      setSelectedVolunteers([]);
      setAssignForm({ unitId: '', teamId: '', notes: '' });
    } catch (error) {
      console.error('Error assigning volunteers:', error);
      alert('Failed to assign volunteers');
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

  const filteredVolunteers = volunteers.filter(v => {
    if (filters.search && !v.name.toLowerCase().includes(filters.search.toLowerCase()) &&
        !v.email.toLowerCase().includes(filters.search.toLowerCase())) {
      return false;
    }
    if (filters.region && v.region !== filters.region) return false;
    if (filters.skills && (!v.skills || !v.skills.includes(filters.skills))) return false;
    return true;
  });

  const regions = [...new Set(volunteers.map(v => v.region).filter(Boolean))];
  const allSkills = [...new Set(volunteers.flatMap(v => v.skills ? v.skills.split(',').map(s => s.trim()) : []))];

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
          <p className="text-gray-600">{filteredVolunteers.length} volunteers</p>
        </div>
        <div className="flex gap-2">
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

      {/* Filters */}
      <div className="bg-white border rounded-lg p-4 mb-6">
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <input
            type="text"
            placeholder="Search by name or email..."
            className="fluent-input"
            value={filters.search}
            onChange={e => setFilters({ ...filters, search: e.target.value })}
          />
          <select
            className="fluent-input"
            value={filters.unit}
            onChange={e => setFilters({ ...filters, unit: e.target.value })}
          >
            <option value="">All Units</option>
            {units.map(u => (
              <option key={u.id} value={u.id}>{u.name}</option>
            ))}
          </select>
          <select
            className="fluent-input"
            value={filters.region}
            onChange={e => setFilters({ ...filters, region: e.target.value })}
          >
            <option value="">All Regions</option>
            {regions.map(r => (
              <option key={r} value={r}>{r}</option>
            ))}
          </select>
          <select
            className="fluent-input"
            value={filters.skills}
            onChange={e => setFilters({ ...filters, skills: e.target.value })}
          >
            <option value="">All Skills</option>
            {allSkills.map(s => (
              <option key={s} value={s}>{s}</option>
            ))}
          </select>
          <button
            onClick={() => setFilters({ search: '', unit: '', team: '', skills: '', region: '' })}
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
