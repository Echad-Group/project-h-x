import React, { useState, useEffect } from 'react';
import campaignTeamService from '../../services/campaignTeamService';

export default function AdminCampaignTeam() {
  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [teamMembers, setTeamMembers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [editingMember, setEditingMember] = useState(null);

  const [form, setForm] = useState({
    name: '',
    role: '',
    email: '',
    phone: '',
    bio: '',
    responsibilities: '',
    photoUrl: '',
    twitterHandle: '',
    linkedInUrl: '',
    facebookUrl: '',
    displayOrder: 0
  });

  useEffect(() => {
    loadMembers();
  }, []);

  async function loadMembers() {
    try {
      setLoading(true);
      const data = await campaignTeamService.getMembers();
      setTeamMembers(data);
      setError(null);
    } catch (err) {
      console.error('Error loading team members:', err);
      setError('Failed to load team members. Please try again.');
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e) {
    e.preventDefault();
    try {
      await campaignTeamService.createMember(form);
      await loadMembers();
      setShowAddModal(false);
      resetForm();
    } catch (err) {
      console.error('Error creating team member:', err);
      alert('Failed to create team member. Please try again.');
    }
  }

  async function handleUpdate(e) {
    e.preventDefault();
    try {
      await campaignTeamService.updateMember(editingMember.id, form);
      await loadMembers();
      setShowEditModal(false);
      setEditingMember(null);
      resetForm();
    } catch (err) {
      console.error('Error updating team member:', err);
      alert('Failed to update team member. Please try again.');
    }
  }

  async function deleteMember(id) {
    if (confirm('Are you sure you want to remove this team member?')) {
      try {
        await campaignTeamService.deleteMember(id);
        await loadMembers();
      } catch (err) {
        console.error('Error deleting team member:', err);
        alert('Failed to delete team member. Please try again.');
      }
    }
  }

  function openEditModal(member) {
    setEditingMember(member);
    setForm({
      name: member.name,
      role: member.role,
      email: member.email,
      phone: member.phone || '',
      bio: member.bio,
      responsibilities: member.responsibilities || '',
      photoUrl: member.photoUrl || '',
      twitterHandle: member.twitterHandle || '',
      linkedInUrl: member.linkedInUrl || '',
      facebookUrl: member.facebookUrl || '',
      displayOrder: member.displayOrder
    });
    setShowEditModal(true);
  }

  function resetForm() {
    setForm({
      name: '',
      role: '',
      email: '',
      phone: '',
      bio: '',
      responsibilities: '',
      photoUrl: '',
      twitterHandle: '',
      linkedInUrl: '',
      facebookUrl: '',
      displayOrder: 0
    });
  }

  if (loading) {
    return (
      <div className="p-8 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--kenya-green)] mx-auto mb-4"></div>
          <p className="text-gray-600">Loading team members...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-8">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Campaign Team Profile</h2>
          <p className="text-gray-600">Manage your core campaign team members</p>
        </div>
        <button
          onClick={() => setShowAddModal(true)}
          className="fluent-btn fluent-btn-primary"
        >
          + Add Team Member
        </button>
      </div>

      {error && (
        <div className="bg-red-50 border border-red-200 text-red-800 px-4 py-3 rounded mb-6">
          {error}
        </div>
      )}

      {/* Team Overview */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-8">
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Total Members</p>
          <p className="text-3xl font-bold text-gray-900">{teamMembers.length}</p>
        </div>
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Leadership</p>
          <p className="text-3xl font-bold text-gray-900">
            {teamMembers.filter(m => m.role.toLowerCase().includes('director') || m.role.toLowerCase().includes('manager')).length}
          </p>
        </div>
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Staff Members</p>
          <p className="text-3xl font-bold text-gray-900">
            {teamMembers.filter(m => !m.role.toLowerCase().includes('director') && !m.role.toLowerCase().includes('manager')).length}
          </p>
        </div>
      </div>

      {/* Team Members Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {teamMembers.map(member => (
          <div key={member.id} className="bg-white border rounded-lg p-6 hover:shadow-md transition-shadow">
            {/* Profile Header */}
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                {member.photoUrl ? (
                  <img 
                    src={member.photoUrl} 
                    alt={member.name}
                    className="w-16 h-16 rounded-full object-cover"
                  />
                ) : (
                  <div className="w-16 h-16 rounded-full bg-[var(--kenya-green)] text-white flex items-center justify-center text-2xl font-bold">
                    {member.name.split(' ').map(n => n[0]).join('')}
                  </div>
                )}
                <div>
                  <h3 className="font-bold text-gray-900">{member.name}</h3>
                  <p className="text-sm text-[var(--kenya-green)] font-medium">{member.role}</p>
                </div>
              </div>
              <button
                onClick={() => deleteMember(member.id)}
                className="text-red-600 hover:text-red-700"
                title="Delete member"
              >
                🗑️
              </button>
            </div>

            {/* Bio */}
            <p className="text-sm text-gray-700 mb-4">{member.bio}</p>

            {/* Contact Info */}
            <div className="space-y-2 mb-4">
              <div className="flex items-center gap-2 text-sm text-gray-600">
                <span>📧</span>
                <a href={`mailto:${member.email}`} className="hover:text-[var(--kenya-green)] truncate">
                  {member.email}
                </a>
              </div>
              {member.phone && (
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <span>📱</span>
                  <a href={`tel:${member.phone}`} className="hover:text-[var(--kenya-green)]">
                    {member.phone}
                  </a>
                </div>
              )}
            </div>

            {/* Social Links */}
            {(member.twitterHandle || member.linkedInUrl || member.facebookUrl) && (
              <div className="flex gap-3 mb-4 text-gray-600">
                {member.twitterHandle && (
                  <a href={`https://twitter.com/${member.twitterHandle}`} target="_blank" rel="noopener noreferrer" className="hover:text-[var(--kenya-green)]">
                    <span className="text-lg">🐦</span>
                  </a>
                )}
                {member.linkedInUrl && (
                  <a href={member.linkedInUrl} target="_blank" rel="noopener noreferrer" className="hover:text-[var(--kenya-green)]">
                    <span className="text-lg">💼</span>
                  </a>
                )}
                {member.facebookUrl && (
                  <a href={member.facebookUrl} target="_blank" rel="noopener noreferrer" className="hover:text-[var(--kenya-green)]">
                    <span className="text-lg">📘</span>
                  </a>
                )}
              </div>
            )}

            {/* Responsibilities */}
            {member.responsibilities && (
              <div className="border-t pt-4">
                <p className="text-xs font-medium text-gray-600 mb-1">Key Responsibilities:</p>
                <p className="text-sm text-gray-700">{member.responsibilities}</p>
              </div>
            )}

            {/* Footer */}
            <div className="mt-4 pt-4 border-t flex items-center justify-between">
              <p className="text-xs text-gray-500">
                Joined {new Date(member.joinedDate).toLocaleDateString()}
              </p>
              <button 
                onClick={() => openEditModal(member)}
                className="text-sm text-[var(--kenya-green)] hover:underline font-medium"
              >
                Edit Profile
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Add Team Member Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b sticky top-0 bg-white">
              <h3 className="text-xl font-bold">Add Team Member</h3>
            </div>
            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Full Name *</label>
                  <input
                    type="text"
                    className="fluent-input"
                    value={form.name}
                    onChange={e => setForm({ ...form, name: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Role/Position *</label>
                  <input
                    type="text"
                    className="fluent-input"
                    placeholder="e.g., Campaign Manager"
                    value={form.role}
                    onChange={e => setForm({ ...form, role: e.target.value })}
                    required
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Email *</label>
                  <input
                    type="email"
                    className="fluent-input"
                    value={form.email}
                    onChange={e => setForm({ ...form, email: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Phone</label>
                  <input
                    type="tel"
                    className="fluent-input"
                    placeholder="+254 712 345 678"
                    value={form.phone}
                    onChange={e => setForm({ ...form, phone: e.target.value })}
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Bio *</label>
                <textarea
                  className="fluent-input"
                  rows={3}
                  placeholder="Brief professional background..."
                  value={form.bio}
                  onChange={e => setForm({ ...form, bio: e.target.value })}
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Key Responsibilities</label>
                <textarea
                  className="fluent-input"
                  rows={2}
                  placeholder="Main areas of responsibility..."
                  value={form.responsibilities}
                  onChange={e => setForm({ ...form, responsibilities: e.target.value })}
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Photo URL</label>
                <input
                  type="url"
                  className="fluent-input"
                  placeholder="https://example.com/photo.jpg"
                  value={form.photoUrl}
                  onChange={e => setForm({ ...form, photoUrl: e.target.value })}
                />
              </div>

              <div className="border-t pt-4">
                <h4 className="font-medium mb-3">Social Media (Optional)</h4>
                <div className="grid grid-cols-3 gap-4">
                  <div>
                    <label className="block text-sm font-medium mb-1">Twitter Handle</label>
                    <input
                      type="text"
                      className="fluent-input"
                      placeholder="@username"
                      value={form.twitterHandle}
                      onChange={e => setForm({ ...form, twitterHandle: e.target.value })}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">LinkedIn URL</label>
                    <input
                      type="url"
                      className="fluent-input"
                      placeholder="https://linkedin.com/in/..."
                      value={form.linkedInUrl}
                      onChange={e => setForm({ ...form, linkedInUrl: e.target.value })}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">Facebook URL</label>
                    <input
                      type="url"
                      className="fluent-input"
                      placeholder="https://facebook.com/..."
                      value={form.facebookUrl}
                      onChange={e => setForm({ ...form, facebookUrl: e.target.value })}
                    />
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Display Order</label>
                <input
                  type="number"
                  className="fluent-input"
                  min="0"
                  value={form.displayOrder}
                  onChange={e => setForm({ ...form, displayOrder: parseInt(e.target.value) })}
                />
                <p className="text-xs text-gray-500 mt-1">Lower numbers appear first</p>
              </div>

              <div className="flex gap-3 pt-4">
                <button type="submit" className="flex-1 fluent-btn fluent-btn-primary">
                  Add Team Member
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowAddModal(false);
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

      {/* Edit Team Member Modal */}
      {showEditModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-3xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b sticky top-0 bg-white">
              <h3 className="text-xl font-bold">Edit Team Member</h3>
            </div>
            <form onSubmit={handleUpdate} className="p-6 space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Full Name *</label>
                  <input
                    type="text"
                    className="fluent-input"
                    value={form.name}
                    onChange={e => setForm({ ...form, name: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Role/Position *</label>
                  <input
                    type="text"
                    className="fluent-input"
                    placeholder="e.g., Campaign Manager"
                    value={form.role}
                    onChange={e => setForm({ ...form, role: e.target.value })}
                    required
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium mb-1">Email *</label>
                  <input
                    type="email"
                    className="fluent-input"
                    value={form.email}
                    onChange={e => setForm({ ...form, email: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Phone</label>
                  <input
                    type="tel"
                    className="fluent-input"
                    placeholder="+254 712 345 678"
                    value={form.phone}
                    onChange={e => setForm({ ...form, phone: e.target.value })}
                  />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Bio *</label>
                <textarea
                  className="fluent-input"
                  rows={3}
                  placeholder="Brief professional background..."
                  value={form.bio}
                  onChange={e => setForm({ ...form, bio: e.target.value })}
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Key Responsibilities</label>
                <textarea
                  className="fluent-input"
                  rows={2}
                  placeholder="Main areas of responsibility..."
                  value={form.responsibilities}
                  onChange={e => setForm({ ...form, responsibilities: e.target.value })}
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Photo URL</label>
                <input
                  type="url"
                  className="fluent-input"
                  placeholder="https://example.com/photo.jpg"
                  value={form.photoUrl}
                  onChange={e => setForm({ ...form, photoUrl: e.target.value })}
                />
              </div>

              <div className="border-t pt-4">
                <h4 className="font-medium mb-3">Social Media (Optional)</h4>
                <div className="grid grid-cols-3 gap-4">
                  <div>
                    <label className="block text-sm font-medium mb-1">Twitter Handle</label>
                    <input
                      type="text"
                      className="fluent-input"
                      placeholder="@username"
                      value={form.twitterHandle}
                      onChange={e => setForm({ ...form, twitterHandle: e.target.value })}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">LinkedIn URL</label>
                    <input
                      type="url"
                      className="fluent-input"
                      placeholder="https://linkedin.com/in/..."
                      value={form.linkedInUrl}
                      onChange={e => setForm({ ...form, linkedInUrl: e.target.value })}
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium mb-1">Facebook URL</label>
                    <input
                      type="url"
                      className="fluent-input"
                      placeholder="https://facebook.com/..."
                      value={form.facebookUrl}
                      onChange={e => setForm({ ...form, facebookUrl: e.target.value })}
                    />
                  </div>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Display Order</label>
                <input
                  type="number"
                  className="fluent-input"
                  min="0"
                  value={form.displayOrder}
                  onChange={e => setForm({ ...form, displayOrder: parseInt(e.target.value) })}
                />
                <p className="text-xs text-gray-500 mt-1">Lower numbers appear first</p>
              </div>

              <div className="flex gap-3 pt-4">
                <button type="submit" className="flex-1 fluent-btn fluent-btn-primary">
                  Update Team Member
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowEditModal(false);
                    setEditingMember(null);
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
