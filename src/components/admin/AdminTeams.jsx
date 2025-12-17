import React, { useState, useEffect } from 'react';
import { unitsService, teamsService } from '../../services/organizationService';

export default function AdminTeams() {
  const [units, setUnits] = useState([]);
  const [teams, setTeams] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedUnit, setSelectedUnit] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [modalType, setModalType] = useState('unit'); // 'unit' or 'team'
  const [form, setForm] = useState({
    name: '',
    description: '',
    icon: '',
    color: '#16A34A',
    displayOrder: 0,
    telegramLink: '',
    whatsAppLink: '',
    requiredSkills: '',
    preferredLocations: '',
    unitId: ''
  });

  useEffect(() => {
    loadData();
  }, []);

  async function loadData() {
    try {
      setLoading(true);
      const [unitsData, teamsData] = await Promise.all([
        unitsService.getAll(),
        teamsService.getAll()
      ]);
      setUnits(unitsData);
      setTeams(teamsData);
    } catch (error) {
      console.error('Error loading teams:', error);
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      if (modalType === 'unit') {
        await unitsService.create(form);
      } else {
        await teamsService.create({
          ...form,
          requiredSkills: form.requiredSkills.split(',').map(s => s.trim()).join(','),
          preferredLocations: form.preferredLocations.split(',').map(s => s.trim()).join(',')
        });
      }
      await loadData();
      setShowCreateModal(false);
      resetForm();
    } catch (error) {
      console.error('Error creating:', error);
      alert('Failed to create. Please try again.');
    }
  }

  function resetForm() {
    setForm({
      name: '',
      description: '',
      icon: '',
      color: '#16A34A',
      displayOrder: 0,
      telegramLink: '',
      whatsAppLink: '',
      requiredSkills: '',
      preferredLocations: '',
      unitId: ''
    });
  }

  function openCreateModal(type) {
    setModalType(type);
    setShowCreateModal(true);
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
          <h2 className="text-2xl font-bold text-gray-900">Teams and Units</h2>
          <p className="text-gray-600">Manage organizational structure</p>
        </div>
        <div className="flex gap-2">
          <button
            onClick={() => openCreateModal('unit')}
            className="fluent-btn fluent-btn-primary"
          >
            + Create Unit
          </button>
          <button
            onClick={() => openCreateModal('team')}
            className="fluent-btn fluent-btn-secondary"
          >
            + Create Team
          </button>
        </div>
      </div>

      {/* Units List */}
      <div className="space-y-6">
        {units.map(unit => {
          const unitTeams = teams.filter(t => t.unitId === unit.id);
          return (
            <div key={unit.id} className="border rounded-lg bg-white overflow-hidden">
              <div
                className="p-6"
                style={{ borderLeft: `4px solid ${unit.color}` }}
              >
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <span className="text-4xl">{unit.icon || '📋'}</span>
                    <div>
                      <h3 className="text-xl font-bold text-gray-900">{unit.name}</h3>
                      <p className="text-gray-600">{unit.description}</p>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <button className="fluent-btn fluent-btn-ghost text-sm">Edit</button>
                    <button className="text-red-600 hover:text-red-700 text-sm font-medium">Delete</button>
                  </div>
                </div>

                {/* Unit Stats */}
                <div className="grid grid-cols-3 gap-4 mb-4">
                  <div className="bg-gray-50 rounded p-3">
                    <p className="text-sm text-gray-600">Teams</p>
                    <p className="text-2xl font-bold">{unitTeams.length}</p>
                  </div>
                  <div className="bg-gray-50 rounded p-3">
                    <p className="text-sm text-gray-600">Volunteers</p>
                    <p className="text-2xl font-bold">{unit.volunteerCount || 0}</p>
                  </div>
                  <div className="bg-gray-50 rounded p-3">
                    <p className="text-sm text-gray-600">Status</p>
                    <p className="text-lg font-medium text-green-600">Active</p>
                  </div>
                </div>

                {/* Communication Links */}
                <div className="flex gap-2">
                  {unit.telegramLink && (
                    <a
                      href={unit.telegramLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-sm text-blue-600 hover:underline"
                    >
                      ✈️ Telegram
                    </a>
                  )}
                  {unit.whatsAppLink && (
                    <a
                      href={unit.whatsAppLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-sm text-green-600 hover:underline"
                    >
                      💬 WhatsApp
                    </a>
                  )}
                </div>
              </div>

              {/* Teams under this unit */}
              {unitTeams.length > 0 && (
                <div className="bg-gray-50 p-6 border-t">
                  <h4 className="font-bold text-gray-900 mb-4">Teams in {unit.name}</h4>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    {unitTeams.map(team => (
                      <div key={team.id} className="bg-white border rounded-lg p-4">
                        <div className="flex items-start justify-between mb-2">
                          <div className="flex items-center gap-2">
                            <span className="text-2xl">{team.icon || '👥'}</span>
                            <div>
                              <h5 className="font-bold text-gray-900">{team.name}</h5>
                              <p className="text-sm text-gray-600">{team.volunteerCount || 0} volunteers</p>
                            </div>
                          </div>
                          <button className="text-gray-400 hover:text-gray-600">⋮</button>
                        </div>
                        <p className="text-sm text-gray-700 mb-2">{team.description}</p>
                        {team.requiredSkills && (
                          <div className="flex flex-wrap gap-1 mt-2">
                            {team.requiredSkills.split(',').slice(0, 3).map((skill, idx) => (
                              <span key={idx} className="px-2 py-1 bg-blue-100 text-blue-700 text-xs rounded">
                                {skill.trim()}
                              </span>
                            ))}
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* Create Modal */}
      {showCreateModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b">
              <h3 className="text-xl font-bold">
                Create New {modalType === 'unit' ? 'Unit' : 'Team'}
              </h3>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Name</label>
                <input
                  type="text"
                  className="fluent-input"
                  value={form.name}
                  onChange={e => setForm({ ...form, name: e.target.value })}
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium mb-1">Description</label>
                <textarea
                  className="fluent-input"
                  rows={3}
                  value={form.description}
                  onChange={e => setForm({ ...form, description: e.target.value })}
                  required
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Icon (Emoji)</label>
                  <input
                    type="text"
                    className="fluent-input"
                    placeholder="📋"
                    value={form.icon}
                    onChange={e => setForm({ ...form, icon: e.target.value })}
                  />
                </div>
                {modalType === 'unit' && (
                  <div>
                    <label className="block text-sm font-medium mb-1">Color</label>
                    <input
                      type="color"
                      className="fluent-input h-10"
                      value={form.color}
                      onChange={e => setForm({ ...form, color: e.target.value })}
                    />
                  </div>
                )}
              </div>
              {modalType === 'team' && (
                <>
                  <div>
                    <label className="block text-sm font-medium mb-1">Unit</label>
                    <select
                      className="fluent-input"
                      value={form.unitId}
                      onChange={e => setForm({ ...form, unitId: e.target.value })}
                      required
                    >
                      <option value="">Select Unit</option>
                      {units.map(unit => (
                        <option key={unit.id} value={unit.id}>{unit.name}</option>
                      ))}
                    </select>
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">Required Skills (comma-separated)</label>
                    <input
                      type="text"
                      className="fluent-input"
                      placeholder="Social Media, Graphic Design"
                      value={form.requiredSkills}
                      onChange={e => setForm({ ...form, requiredSkills: e.target.value })}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">Preferred Locations (comma-separated)</label>
                    <input
                      type="text"
                      className="fluent-input"
                      placeholder="Nairobi, Mombasa"
                      value={form.preferredLocations}
                      onChange={e => setForm({ ...form, preferredLocations: e.target.value })}
                    />
                  </div>
                </>
              )}
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Telegram Link</label>
                  <input
                    type="url"
                    className="fluent-input"
                    placeholder="https://t.me/..."
                    value={form.telegramLink}
                    onChange={e => setForm({ ...form, telegramLink: e.target.value })}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">WhatsApp Link</label>
                  <input
                    type="url"
                    className="fluent-input"
                    placeholder="https://chat.whatsapp.com/..."
                    value={form.whatsAppLink}
                    onChange={e => setForm({ ...form, whatsAppLink: e.target.value })}
                  />
                </div>
              </div>
              <div className="flex gap-3 pt-4">
                <button type="submit" className="flex-1 fluent-btn fluent-btn-primary">
                  Create {modalType === 'unit' ? 'Unit' : 'Team'}
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowCreateModal(false);
                    resetForm();
                  }}
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
