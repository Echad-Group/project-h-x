import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { unitsService } from '../services/organizationService';

export default function UnitsTeamsVisualization() {
  const { t } = useTranslation();
  const [units, setUnits] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [expandedUnit, setExpandedUnit] = useState(null);
  const [selectedTeam, setSelectedTeam] = useState(null);

  useEffect(() => {
    loadUnits();
  }, []);

  async function loadUnits() {
    try {
      setLoading(true);
      const data = await unitsService.getAll();
      setUnits(data);
    } catch (err) {
      console.error('Error loading units:', err);
      setError('Failed to load organization structure');
    } finally {
      setLoading(false);
    }
  }

  function toggleUnit(unitId) {
    setExpandedUnit(expandedUnit === unitId ? null : unitId);
    setSelectedTeam(null);
  }

  function selectTeam(team) {
    setSelectedTeam(team);
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center p-12">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading organization structure...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-6 text-center">
        <p className="text-red-700">{error}</p>
        <button onClick={loadUnits} className="mt-4 fluent-btn fluent-btn-primary">
          Try Again
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="text-center mb-8">
        <h2 className="text-3xl font-bold mb-2">Organization Structure</h2>
        <p className="text-gray-600">
          Explore our units and teams to find where you can contribute
        </p>
      </div>

      {/* Units Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {units.map(unit => (
          <div
            key={unit.id}
            className="bg-white rounded-lg card-shadow overflow-hidden transition-all hover:shadow-xl"
          >
            <div
              className="p-6 cursor-pointer"
              style={{ borderTop: `4px solid ${unit.color || '#16A34A'}` }}
              onClick={() => toggleUnit(unit.id)}
            >
              <div className="flex items-center justify-between mb-3">
                <div className="flex items-center gap-3">
                  <span className="text-4xl">{unit.icon || '📋'}</span>
                  <h3 className="text-xl font-bold">{unit.name}</h3>
                </div>
                <svg
                  className={`w-6 h-6 transform transition-transform ${expandedUnit === unit.id ? 'rotate-180' : ''}`}
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                >
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
              </div>

              <p className="text-gray-600 text-sm mb-4">{unit.description}</p>

              <div className="flex items-center gap-4 text-sm text-gray-500">
                <span className="flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                  {unit.volunteerCount} volunteers
                </span>
                <span className="flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                  {unit.teamCount} teams
                </span>
              </div>

              {/* Communication Links */}
              {(unit.telegramLink || unit.whatsAppLink) && (
                <div className="mt-4 flex gap-2">
                  {unit.telegramLink && (
                    <a
                      href={unit.telegramLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-1 px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-xs hover:bg-blue-200"
                      onClick={(e) => e.stopPropagation()}
                    >
                      ✈️ Telegram
                    </a>
                  )}
                  {unit.whatsAppLink && (
                    <a
                      href={unit.whatsAppLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-1 px-3 py-1 bg-green-100 text-green-700 rounded-full text-xs hover:bg-green-200"
                      onClick={(e) => e.stopPropagation()}
                    >
                      💬 WhatsApp
                    </a>
                  )}
                </div>
              )}
            </div>

            {/* Teams List */}
            {expandedUnit === unit.id && unit.teams && unit.teams.length > 0 && (
              <div className="border-t bg-gray-50 p-4 space-y-2">
                <h4 className="font-semibold text-sm text-gray-700 mb-3">Teams</h4>
                {unit.teams.map(team => (
                  <div
                    key={team.id}
                    className="bg-white rounded-lg p-3 hover:shadow-md transition-shadow cursor-pointer border border-gray-200"
                    onClick={() => selectTeam(team)}
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <span className="text-2xl">{team.icon || '👥'}</span>
                        <div>
                          <h5 className="font-medium text-sm">{team.name}</h5>
                          <p className="text-xs text-gray-500">{team.volunteerCount} members</p>
                        </div>
                      </div>
                      <svg className="w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                      </svg>
                    </div>
                    {team.requiredSkills && (
                      <div className="mt-2 flex flex-wrap gap-1">
                        {team.requiredSkills.split(',').slice(0, 3).map((skill, idx) => (
                          <span key={idx} className="px-2 py-0.5 bg-blue-50 text-blue-700 rounded text-xs">
                            {skill.trim()}
                          </span>
                        ))}
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        ))}
      </div>

      {/* Team Detail Modal */}
      {selectedTeam && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" onClick={() => setSelectedTeam(null)}>
          <div className="bg-white rounded-lg max-w-2xl w-full p-6 max-h-[90vh] overflow-y-auto" onClick={(e) => e.stopPropagation()}>
            <div className="flex items-start justify-between mb-4">
              <div className="flex items-center gap-3">
                <span className="text-4xl">{selectedTeam.icon || '👥'}</span>
                <div>
                  <h3 className="text-2xl font-bold">{selectedTeam.name}</h3>
                  <p className="text-sm text-gray-500">{selectedTeam.volunteerCount} volunteers</p>
                </div>
              </div>
              <button
                onClick={() => setSelectedTeam(null)}
                className="text-gray-400 hover:text-gray-600 text-2xl"
              >
                ×
              </button>
            </div>

            <p className="text-gray-700 mb-6">{selectedTeam.description}</p>

            {selectedTeam.requiredSkills && (
              <div className="mb-4">
                <h4 className="font-semibold mb-2">Required Skills:</h4>
                <div className="flex flex-wrap gap-2">
                  {selectedTeam.requiredSkills.split(',').map((skill, idx) => (
                    <span key={idx} className="px-3 py-1 bg-blue-100 text-blue-700 rounded-full text-sm">
                      {skill.trim()}
                    </span>
                  ))}
                </div>
              </div>
            )}

            {selectedTeam.preferredLocations && (
              <div className="mb-4">
                <h4 className="font-semibold mb-2">Preferred Locations:</h4>
                <div className="flex flex-wrap gap-2">
                  {selectedTeam.preferredLocations.split(',').map((location, idx) => (
                    <span key={idx} className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm">
                      {location.trim()}
                    </span>
                  ))}
                </div>
              </div>
            )}

            {(selectedTeam.telegramLink || selectedTeam.whatsAppLink) && (
              <div className="mb-4">
                <h4 className="font-semibold mb-2">Join the Team:</h4>
                <div className="flex gap-3">
                  {selectedTeam.telegramLink && (
                    <a
                      href={selectedTeam.telegramLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-2 px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600"
                    >
                      ✈️ Join Telegram
                    </a>
                  )}
                  {selectedTeam.whatsAppLink && (
                    <a
                      href={selectedTeam.whatsAppLink}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="flex items-center gap-2 px-4 py-2 bg-green-500 text-white rounded-lg hover:bg-green-600"
                    >
                      💬 Join WhatsApp
                    </a>
                  )}
                </div>
              </div>
            )}

            <div className="mt-6 pt-4 border-t">
              <button
                onClick={() => {
                  // TODO: Implement volunteer interest/signup for this team
                  alert('Volunteer signup coming soon!');
                }}
                className="fluent-btn fluent-btn-primary w-full"
              >
                Express Interest in This Team
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
