import React, { useState, useEffect } from 'react';
import api from '../../services/api';

export default function AdminOverview() {
  const [stats, setStats] = useState({
    volunteers: 0,
    events: 0,
    donations: 0,
    activeProjects: 0,
    teams: 0,
    units: 0,
    totalDonations: 0,
    upcomingEvents: 0
  });
  const [loading, setLoading] = useState(true);
  const [backendAvailable, setBackendAvailable] = useState(true);

  useEffect(() => {
    loadStats();
  }, []);

  async function loadStats() {
    try {
      setLoading(true);
      // Fetch various stats from different endpoints with silent failure
      const [volunteers, events, donations, units, teams] = await Promise.all([
        api.get('/volunteers').catch(() => {
          setBackendAvailable(false);
          return { data: [] };
        }),
        api.get('/events').catch(() => {
          setBackendAvailable(false);
          return { data: [] };
        }),
        api.get('/donations').catch(() => {
          setBackendAvailable(false);
          return { data: [] };
        }),
        api.get('/units').catch(() => {
          setBackendAvailable(false);
          return { data: [] };
        }),
        api.get('/teams').catch(() => {
          setBackendAvailable(false);
          return { data: [] };
        })
      ]);

      const totalDonations = donations.data.reduce((sum, d) => sum + (d.amount || 0), 0);
      const upcomingEvents = events.data.filter(e => new Date(e.date) > new Date()).length;

      setStats({
        volunteers: volunteers.data.length || 0,
        events: events.data.length || 0,
        donations: donations.data.length || 0,
        activeProjects: 0, // TODO: Fetch from projects endpoint
        teams: teams.data.length || 0,
        units: units.data.length || 0,
        totalDonations,
        upcomingEvents
      });
    } catch (error) {
      // Silent failure - component continues to render with empty data
      console.log('Backend unavailable, showing offline mode');
      setBackendAvailable(false);
    } finally {
      setLoading(false);
    }
  }

  const statCards = [
    { label: 'Total Volunteers', value: stats.volunteers, icon: '🤝', color: 'bg-blue-500' },
    { label: 'Active Teams', value: stats.teams, icon: '👥', color: 'bg-green-500' },
    { label: 'Organization Units', value: stats.units, icon: '🏢', color: 'bg-purple-500' },
    { label: 'Total Events', value: stats.events, icon: '📅', color: 'bg-orange-500' },
    { label: 'Upcoming Events', value: stats.upcomingEvents, icon: '🎯', color: 'bg-red-500' },
    { label: 'Total Donations', value: `KSh ${stats.totalDonations.toLocaleString()}`, icon: '💰', color: 'bg-yellow-500' },
    { label: 'Donation Count', value: stats.donations, icon: '🎁', color: 'bg-pink-500' },
    { label: 'Active Projects', value: stats.activeProjects, icon: '📊', color: 'bg-indigo-500' }
  ];

  if (loading) {
    return (
      <div className="p-8 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-8">
      {/* Backend status indicator */}
      {!backendAvailable && (
        <div className="mb-6 bg-yellow-50 border border-yellow-200 rounded-lg p-4 flex items-center gap-3">
          <span className="text-2xl">⚠️</span>
          <div>
            <p className="font-medium text-yellow-800">Offline Mode</p>
            <p className="text-sm text-yellow-700">Backend is unavailable. Showing cached/empty data.</p>
          </div>
        </div>
      )}
      
      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {statCards.map((stat, idx) => (
          <div key={idx} className="bg-white border rounded-lg p-6 hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-4">
              <div className={`w-12 h-12 ${stat.color} rounded-lg flex items-center justify-center text-2xl`}>
                {stat.icon}
              </div>
            </div>
            <p className="text-gray-600 text-sm mb-1">{stat.label}</p>
            <p className="text-2xl font-bold text-gray-900">{stat.value}</p>
          </div>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="bg-white border rounded-lg p-6 mb-8">
        <h3 className="text-lg font-bold text-gray-900 mb-4">Quick Actions</h3>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <button className="p-4 border rounded-lg hover:bg-gray-50 transition-colors text-left">
            <span className="text-2xl block mb-2">➕</span>
            <span className="font-medium">Create Event</span>
          </button>
          <button className="p-4 border rounded-lg hover:bg-gray-50 transition-colors text-left">
            <span className="text-2xl block mb-2">📧</span>
            <span className="font-medium">Send Email</span>
          </button>
          <button className="p-4 border rounded-lg hover:bg-gray-50 transition-colors text-left">
            <span className="text-2xl block mb-2">👥</span>
            <span className="font-medium">Add Team</span>
          </button>
          <button className="p-4 border rounded-lg hover:bg-gray-50 transition-colors text-left">
            <span className="text-2xl block mb-2">📊</span>
            <span className="font-medium">View Reports</span>
          </button>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="bg-white border rounded-lg p-6">
        <h3 className="text-lg font-bold text-gray-900 mb-4">Recent Activity</h3>
        {backendAvailable ? (
          <div className="space-y-4">
            <div className="flex items-start gap-3 pb-4 border-b">
              <div className="w-2 h-2 bg-green-500 rounded-full mt-2"></div>
              <div>
                <p className="font-medium text-gray-900">New volunteer registration</p>
                <p className="text-sm text-gray-600">John Doe joined the Communications Unit</p>
                <p className="text-xs text-gray-500 mt-1">2 hours ago</p>
              </div>
            </div>
            <div className="flex items-start gap-3 pb-4 border-b">
              <div className="w-2 h-2 bg-blue-500 rounded-full mt-2"></div>
              <div>
                <p className="font-medium text-gray-900">Event created</p>
              <p className="text-sm text-gray-600">Town Hall Meeting scheduled for Dec 20</p>
              <p className="text-xs text-gray-500 mt-1">5 hours ago</p>
            </div>
          </div>
          <div className="flex items-start gap-3">
            <div className="w-2 h-2 bg-yellow-500 rounded-full mt-2"></div>
            <div>
              <p className="font-medium text-gray-900">Donation received</p>
              <p className="text-sm text-gray-600">Anonymous donation of KSh 5,000</p>
              <p className="text-xs text-gray-500 mt-1">1 day ago</p>
            </div>
          </div>
        </div>
        ) : (
          <div className="text-center text-gray-400 py-8">
            <p className="text-sm">Activity feed unavailable in offline mode</p>
          </div>
        )}
      </div>
    </div>
  );
}
