import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../contexts/AuthContext';
import { volunteerService } from '../services/volunteerService';
import { assignmentsService } from '../services/organizationService';
import { Link } from 'react-router-dom';

export default function VolunteerDashboard() {
  const { t } = useTranslation();
  const { user, isAuthenticated } = useAuth();
  const [loading, setLoading] = useState(true);
  const [volunteerData, setVolunteerData] = useState(null);
  const [assignments, setAssignments] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (isAuthenticated) {
      loadDashboard();
    }
  }, [isAuthenticated]);

  async function loadDashboard() {
    try {
      setLoading(true);
      
      // Get volunteer status
      const status = await volunteerService.checkStatus();
      
      if (status.isVolunteer) {
        setVolunteerData(status.volunteer);
        
        // Get assignments
        const assignmentsData = await assignmentsService.getByVolunteer(status.volunteer.id);
        setAssignments(assignmentsData);
      }
    } catch (err) {
      console.error('Error loading dashboard:', err);
      setError('Failed to load dashboard');
    } finally {
      setLoading(false);
    }
  }

  if (!isAuthenticated) {
    return (
      <div className="bg-blue-50 border border-blue-200 rounded-lg p-6 text-center">
        <p className="text-blue-800 mb-4">Please log in to view your volunteer dashboard.</p>
        <Link to="/login" className="fluent-btn fluent-btn-primary">
          Log In
        </Link>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center p-12">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading your dashboard...</p>
        </div>
      </div>
    );
  }

  if (!volunteerData) {
    return (
      <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 text-center">
        <h3 className="text-xl font-bold text-yellow-900 mb-2">You're Not Registered as a Volunteer Yet</h3>
        <p className="text-yellow-800 mb-4">
          Sign up to become a volunteer and join our movement!
        </p>
        <Link to="/get-involved" className="fluent-btn fluent-btn-primary">
          Sign Up as Volunteer
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Welcome Header */}
      <div className="bg-gradient-to-r from-[var(--kenya-green)] to-green-600 text-white rounded-lg p-6">
        <h1 className="text-3xl font-bold mb-2">Welcome back, {volunteerData.name}!</h1>
        <p className="text-green-50">Thank you for being part of the New Kenya movement</p>
      </div>

      {/* Assignments Section */}
      <div className="bg-white rounded-lg card-shadow p-6">
        <h2 className="text-2xl font-bold mb-4 flex items-center gap-2">
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
          </svg>
          Your Assignments
        </h2>

        {assignments.length === 0 ? (
          <div className="text-center py-8">
            <svg className="w-16 h-16 mx-auto text-gray-300 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
            </svg>
            <p className="text-gray-600 mb-2">You haven't been assigned to any units or teams yet.</p>
            <p className="text-sm text-gray-500">Check back soon or contact the campaign coordinator.</p>
          </div>
        ) : (
          <div className="space-y-4">
            {assignments.map(assignment => (
              <div
                key={assignment.id}
                className="border rounded-lg p-4 hover:shadow-md transition-shadow"
                style={{ borderLeft: `4px solid ${assignment.unit.color || '#16A34A'}` }}
              >
                <div className="flex items-start justify-between">
                  <div className="flex items-start gap-3">
                    <span className="text-3xl">{assignment.unit.icon || '📋'}</span>
                    <div>
                      <h3 className="font-bold text-lg">{assignment.unit.name}</h3>
                      {assignment.team && (
                        <p className="text-gray-600 flex items-center gap-1">
                          <span className="text-xl">{assignment.team.icon || '👥'}</span>
                          {assignment.team.name}
                        </p>
                      )}
                      <p className="text-sm text-gray-500 mt-1">
                        Joined: {new Date(assignment.assignedAt).toLocaleDateString()}
                      </p>
                    </div>
                  </div>
                </div>

                <p className="text-gray-700 mt-3 text-sm">
                  {assignment.team ? assignment.team.description : assignment.unit.description}
                </p>

                {/* Communication Channels */}
                <div className="mt-4 flex flex-wrap gap-2">
                  {(assignment.team?.telegramLink || assignment.unit.telegramLink) && (
                    <a
                      href={assignment.team?.telegramLink || assignment.unit.telegramLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-1 px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm hover:bg-blue-200"
                    >
                      ✈️ Join Telegram Group
                    </a>
                  )}
                  {(assignment.team?.whatsAppLink || assignment.unit.whatsAppLink) && (
                    <a
                      href={assignment.team?.whatsAppLink || assignment.unit.whatsAppLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-1 px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm hover:bg-green-200"
                    >
                      💬 Join WhatsApp Group
                    </a>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Profile Information */}
      <div className="bg-white rounded-lg card-shadow p-6">
        <h2 className="text-2xl font-bold mb-4 flex items-center gap-2">
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
          </svg>
          Your Profile
        </h2>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-600">Email</label>
            <p className="mt-1 text-gray-900">{volunteerData.email}</p>
          </div>
          {volunteerData.phone && (
            <div>
              <label className="block text-sm font-medium text-gray-600">Phone</label>
              <p className="mt-1 text-gray-900">{volunteerData.phone}</p>
            </div>
          )}
          {volunteerData.city && (
            <div>
              <label className="block text-sm font-medium text-gray-600">City</label>
              <p className="mt-1 text-gray-900">{volunteerData.city}</p>
            </div>
          )}
          {volunteerData.region && (
            <div>
              <label className="block text-sm font-medium text-gray-600">Region</label>
              <p className="mt-1 text-gray-900">{volunteerData.region}</p>
            </div>
          )}
          {volunteerData.skills && (
            <div className="md:col-span-2">
              <label className="block text-sm font-medium text-gray-600 mb-2">Skills</label>
              <div className="flex flex-wrap gap-2">
                {volunteerData.skills.split(',').map((skill, idx) => (
                  <span key={idx} className="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm">
                    {skill.trim()}
                  </span>
                ))}
              </div>
            </div>
          )}
          {volunteerData.hoursPerWeek && (
            <div>
              <label className="block text-sm font-medium text-gray-600">Availability</label>
              <p className="mt-1 text-gray-900">{volunteerData.hoursPerWeek} hours/week</p>
              {volunteerData.availableWeekends && (
                <span className="text-sm text-gray-600">• Weekends available</span>
              )}
              {volunteerData.availableEvenings && (
                <span className="text-sm text-gray-600 ml-2">• Evenings available</span>
              )}
            </div>
          )}
        </div>
      </div>

      {/* Quick Actions */}
      <div className="bg-white rounded-lg card-shadow p-6">
        <h2 className="text-2xl font-bold mb-4">Quick Actions</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Link
            to="/events"
            className="flex items-center gap-3 p-4 border rounded-lg hover:shadow-md transition-shadow"
          >
            <svg className="w-8 h-8 text-[var(--kenya-green)]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            <div>
              <h3 className="font-semibold">Upcoming Events</h3>
              <p className="text-sm text-gray-600">RSVP to events</p>
            </div>
          </Link>

          <Link
            to="/get-involved"
            className="flex items-center gap-3 p-4 border rounded-lg hover:shadow-md transition-shadow"
          >
            <svg className="w-8 h-8 text-[var(--kenya-green)]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <div>
              <h3 className="font-semibold">Explore Teams</h3>
              <p className="text-sm text-gray-600">View all units</p>
            </div>
          </Link>

          <Link
            to="/contact"
            className="flex items-center gap-3 p-4 border rounded-lg hover:shadow-md transition-shadow"
          >
            <svg className="w-8 h-8 text-[var(--kenya-green)]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
            </svg>
            <div>
              <h3 className="font-semibold">Contact Us</h3>
              <p className="text-sm text-gray-600">Get support</p>
            </div>
          </Link>
        </div>
      </div>
    </div>
  );
}
