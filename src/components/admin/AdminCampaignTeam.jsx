import React, { useState } from 'react';

export default function AdminCampaignTeam() {
  const [showAddModal, setShowAddModal] = useState(false);
  const [teamMembers, setTeamMembers] = useState([
    {
      id: 1,
      name: 'Sarah Kimani',
      role: 'Campaign Manager',
      email: 'sarah.kimani@newkenya.org',
      phone: '+254 712 345 678',
      bio: 'Experienced political strategist with 10+ years in grassroots organizing',
      photo: null,
      joined: '2025-01-15',
      responsibilities: 'Overall campaign strategy, team coordination'
    },
    {
      id: 2,
      name: 'John Omondi',
      role: 'Communications Director',
      email: 'john.omondi@newkenya.org',
      phone: '+254 723 456 789',
      bio: 'Former journalist with expertise in political communications',
      photo: null,
      joined: '2025-02-01',
      responsibilities: 'Media relations, press releases, social media strategy'
    },
    {
      id: 3,
      name: 'Mary Wanjiru',
      role: 'Field Operations Manager',
      email: 'mary.wanjiru@newkenya.org',
      phone: '+254 734 567 890',
      bio: 'Community organizer specializing in rural mobilization',
      photo: null,
      joined: '2025-02-10',
      responsibilities: 'Ground operations, volunteer coordination, regional campaigns'
    }
  ]);

  const [form, setForm] = useState({
    name: '',
    role: '',
    email: '',
    phone: '',
    bio: '',
    responsibilities: ''
  });

  function handleSubmit(e) {
    e.preventDefault();
    const newMember = {
      id: teamMembers.length + 1,
      ...form,
      photo: null,
      joined: new Date().toISOString().split('T')[0]
    };
    setTeamMembers([...teamMembers, newMember]);
    setShowAddModal(false);
    setForm({
      name: '',
      role: '',
      email: '',
      phone: '',
      bio: '',
      responsibilities: ''
    });
  }

  function deleteMember(id) {
    if (confirm('Are you sure you want to remove this team member?')) {
      setTeamMembers(teamMembers.filter(m => m.id !== id));
    }
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

      {/* Team Overview */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Total Members</p>
          <p className="text-3xl font-bold text-gray-900">{teamMembers.length}</p>
        </div>
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Leadership</p>
          <p className="text-3xl font-bold text-gray-900">5</p>
        </div>
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Field Staff</p>
          <p className="text-3xl font-bold text-gray-900">12</p>
        </div>
        <div className="bg-white border rounded-lg p-6">
          <p className="text-gray-600 text-sm mb-1">Support Staff</p>
          <p className="text-3xl font-bold text-gray-900">8</p>
        </div>
      </div>

      {/* Team Members Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {teamMembers.map(member => (
          <div key={member.id} className="bg-white border rounded-lg p-6 hover:shadow-md transition-shadow">
            {/* Profile Header */}
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                <div className="w-16 h-16 rounded-full bg-[var(--kenya-green)] text-white flex items-center justify-center text-2xl font-bold">
                  {member.name.split(' ').map(n => n[0]).join('')}
                </div>
                <div>
                  <h3 className="font-bold text-gray-900">{member.name}</h3>
                  <p className="text-sm text-[var(--kenya-green)] font-medium">{member.role}</p>
                </div>
              </div>
              <button
                onClick={() => deleteMember(member.id)}
                className="text-red-600 hover:text-red-700"
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
                <a href={`mailto:${member.email}`} className="hover:text-[var(--kenya-green)]">
                  {member.email}
                </a>
              </div>
              <div className="flex items-center gap-2 text-sm text-gray-600">
                <span>📱</span>
                <a href={`tel:${member.phone}`} className="hover:text-[var(--kenya-green)]">
                  {member.phone}
                </a>
              </div>
            </div>

            {/* Responsibilities */}
            <div className="border-t pt-4">
              <p className="text-xs font-medium text-gray-600 mb-1">Key Responsibilities:</p>
              <p className="text-sm text-gray-700">{member.responsibilities}</p>
            </div>

            {/* Footer */}
            <div className="mt-4 pt-4 border-t flex items-center justify-between">
              <p className="text-xs text-gray-500">
                Joined {new Date(member.joined).toLocaleDateString()}
              </p>
              <button className="text-sm text-[var(--kenya-green)] hover:underline font-medium">
                Edit Profile
              </button>
            </div>
          </div>
        ))}
      </div>

      {/* Add Team Member Modal */}
      {showAddModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto">
            <div className="p-6 border-b">
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

              <div className="flex gap-3 pt-4">
                <button type="submit" className="flex-1 fluent-btn fluent-btn-primary">
                  Add Team Member
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowAddModal(false);
                    setForm({
                      name: '',
                      role: '',
                      email: '',
                      phone: '',
                      bio: '',
                      responsibilities: ''
                    });
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
