import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useTranslation } from 'react-i18next';
import AdminProjects from '../components/admin/AdminProjects';
import AdminTeams from '../components/admin/AdminTeams';
import AdminVolunteers from '../components/admin/AdminVolunteers';
import AdminEmails from '../components/admin/AdminEmails';
import AdminEngagement from '../components/admin/AdminEngagement';
import AdminCampaignTeam from '../components/admin/AdminCampaignTeam';
import AdminOverview from '../components/admin/AdminOverview';
import AdminNewsManagement from '../components/admin/AdminNewsManagement';

export default function AdminPanel() {
  const { user } = useAuth();
  const { t } = useTranslation();
  const [activeTab, setActiveTab] = useState('overview');
  const [sidebarOpen, setSidebarOpen] = useState(true);

  useEffect(() => {
    // Role-based access control is now handled by ProtectedRoute wrapper
    // This component will only be rendered if user has Admin role
  }, []);

  const tabs = [
    { id: 'overview', name: 'Overview', icon: '📊' },
    { id: 'projects', name: 'Campaign Projects', icon: '🎯' },
    { id: 'teams', name: 'Teams and Units', icon: '👥' },
    { id: 'volunteers', name: 'Volunteer Management', icon: '🤝' },
    { id: 'campaign-team', name: 'Campaign Team Profile', icon: '👔' },
    { id: 'news', name: 'News Management', icon: '📰' },
    { id: 'emails', name: 'Email Updates', icon: '✉️' },
    { id: 'engagement', name: 'Engagement Analytics', icon: '📈' },
  ];

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="flex">
        {/* Sidebar */}
        <aside className={`${sidebarOpen ? 'w-64' : 'w-20'} bg-white shadow-lg transition-all duration-300 min-h-screen sticky top-0`}>
          <div className="p-4 border-b">
            <div className="flex items-center justify-between">
              {sidebarOpen && (
                <div>
                  <h2 className="text-xl font-bold text-gray-900">Admin Panel</h2>
                  <p className="text-xs text-gray-500">New Kenya Movement</p>
                </div>
              )}
              <button
                onClick={() => setSidebarOpen(!sidebarOpen)}
                className="p-2 hover:bg-gray-100 rounded-lg"
              >
                {sidebarOpen ? '←' : '→'}
              </button>
            </div>
          </div>

          <nav className="p-4 space-y-2">
            {tabs.map(tab => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                  activeTab === tab.id
                    ? 'bg-[var(--kenya-green)] text-white'
                    : 'text-gray-700 hover:bg-gray-100'
                }`}
              >
                <span className="text-xl">{tab.icon}</span>
                {sidebarOpen && <span className="font-medium">{tab.name}</span>}
              </button>
            ))}
          </nav>

          {sidebarOpen && (
            <div className="p-4 border-t mt-auto">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-full bg-[var(--kenya-green)] text-white flex items-center justify-center font-bold">
                  {user?.firstName?.[0] || user?.email?.[0] || 'A'}
                </div>
                <div>
                  <p className="font-medium text-sm">{user?.firstName || 'Admin'}</p>
                  <p className="text-xs text-gray-500">{user?.email}</p>
                </div>
              </div>
            </div>
          )}
        </aside>

        {/* Main Content */}
        <main className="flex-1 p-8">
          <div className="max-w-7xl mx-auto">
            {/* Header */}
            <div className="mb-8">
              <h1 className="text-3xl font-bold text-gray-900 mb-2">
                {tabs.find(t => t.id === activeTab)?.name}
              </h1>
              <p className="text-gray-600">
                Manage and monitor your campaign activities
              </p>
            </div>

            {/* Content Area */}
            <div className="bg-white rounded-lg shadow-sm">
              {activeTab === 'overview' && <AdminOverview />}
              {activeTab === 'projects' && <AdminProjects />}
              {activeTab === 'teams' && <AdminTeams />}
              {activeTab === 'volunteers' && <AdminVolunteers />}
              {activeTab === 'campaign-team' && <AdminCampaignTeam />}
              {activeTab === 'news' && <AdminNewsManagement />}
              {activeTab === 'emails' && <AdminEmails />}
              {activeTab === 'engagement' && <AdminEngagement />}
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}
