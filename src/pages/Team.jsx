import React, { useEffect, useState } from 'react'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'
import campaignTeamService from '../services/campaignTeamService'

export default function Team(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();
  const [teamMembers, setTeamMembers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    updateMeta({
      title: `${t('team.title')} - New Kenya Campaign`,
      description: t('team.description'),
      url: '/team'
    });

    loadTeamMembers();
  }, [t]);

  async function loadTeamMembers() {
    try {
      setLoading(true);
      const data = await campaignTeamService.getMembers();
      setTeamMembers(data);
    } catch (err) {
      console.error('Error loading team members:', err);
      setError('Failed to load team members. Please try again later.');
    } finally {
      setLoading(false);
    }
  }

  if (loading) {
    return (
      <section className="max-w-6xl mx-auto px-4 py-12">
        <div className="flex items-center justify-center py-20">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[var(--kenya-green)] mx-auto mb-4"></div>
            <p className="text-gray-600">Loading team members...</p>
          </div>
        </div>
      </section>
    );
  }

  if (error) {
    return (
      <section className="max-w-6xl mx-auto px-4 py-12">
        <div className="bg-red-50 border border-red-200 text-red-800 px-6 py-4 rounded-lg">
          {error}
        </div>
      </section>
    );
  }

  return (
    <section className="max-w-7xl mx-auto px-4 py-12">
      {/* Header */}
      <div className="text-center mb-12">
        <h1 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
          {t('team.title')}
        </h1>
        <p className="text-xl text-gray-600 max-w-3xl mx-auto">
          Meet the dedicated professionals driving the New Kenya Campaign forward
        </p>
      </div>

      {teamMembers.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">No team members to display yet.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          {teamMembers.map((member) => (
            <div 
              key={member.id} 
              className="bg-white rounded-xl shadow-lg hover:shadow-2xl transition-all duration-300 overflow-hidden group"
            >
              {/* Profile Image */}
              <div className="relative h-64 bg-gradient-to-br from-[var(--kenya-green)] to-[var(--kenya-red)] overflow-hidden">
                {member.photoUrl ? (
                  <img 
                    src={member.photoUrl} 
                    alt={member.name}
                    className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-300"
                  />
                ) : (
                  <div className="w-full h-full flex items-center justify-center">
                    <div className="text-white text-6xl font-bold">
                      {member.name.split(' ').map(n => n[0]).join('')}
                    </div>
                  </div>
                )}
                
                {/* Social Links Overlay */}
                {(member.twitterHandle || member.linkedInUrl || member.facebookUrl) && (
                  <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black/70 to-transparent p-4 opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                    <div className="flex justify-center gap-4 text-white">
                      {member.twitterHandle && (
                        <a 
                          href={`https://twitter.com/${member.twitterHandle.replace('@', '')}`} 
                          target="_blank" 
                          rel="noopener noreferrer"
                          className="hover:scale-125 transition-transform"
                          title="Twitter"
                        >
                          <span className="text-2xl">🐦</span>
                        </a>
                      )}
                      {member.linkedInUrl && (
                        <a 
                          href={member.linkedInUrl} 
                          target="_blank" 
                          rel="noopener noreferrer"
                          className="hover:scale-125 transition-transform"
                          title="LinkedIn"
                        >
                          <span className="text-2xl">💼</span>
                        </a>
                      )}
                      {member.facebookUrl && (
                        <a 
                          href={member.facebookUrl} 
                          target="_blank" 
                          rel="noopener noreferrer"
                          className="hover:scale-125 transition-transform"
                          title="Facebook"
                        >
                          <span className="text-2xl">📘</span>
                        </a>
                      )}
                    </div>
                  </div>
                )}
              </div>

              {/* Member Info */}
              <div className="p-6">
                <h3 className="text-2xl font-bold text-gray-900 mb-1">{member.name}</h3>
                <p className="text-[var(--kenya-green)] font-semibold text-lg mb-3">{member.role}</p>
                
                <p className="text-gray-600 text-sm leading-relaxed mb-4 line-clamp-3">
                  {member.bio}
                </p>

                {/* Contact Info */}
                <div className="space-y-2 pt-4 border-t border-gray-100">
                  <a 
                    href={`mailto:${member.email}`} 
                    className="flex items-center gap-2 text-sm text-gray-600 hover:text-[var(--kenya-green)] transition-colors"
                  >
                    <span>📧</span>
                    <span className="truncate">{member.email}</span>
                  </a>
                  
                  {member.phone && (
                    <a 
                      href={`tel:${member.phone}`} 
                      className="flex items-center gap-2 text-sm text-gray-600 hover:text-[var(--kenya-green)] transition-colors"
                    >
                      <span>📱</span>
                      <span>{member.phone}</span>
                    </a>
                  )}
                </div>

                {/* Responsibilities */}
                {member.responsibilities && (
                  <div className="mt-4 pt-4 border-t border-gray-100">
                    <p className="text-xs font-semibold text-gray-500 uppercase mb-2">
                      Key Responsibilities
                    </p>
                    <p className="text-sm text-gray-700 leading-relaxed">
                      {member.responsibilities}
                    </p>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* Call to Action */}
      {teamMembers.length > 0 && (
        <div className="mt-16 text-center">
          <div className="bg-gradient-to-r from-[var(--kenya-green)] to-[var(--kenya-red)] text-white rounded-2xl p-8 shadow-xl">
            <h2 className="text-3xl font-bold mb-4">Join Our Team</h2>
            <p className="text-lg mb-6 max-w-2xl mx-auto">
              We're always looking for passionate individuals to help build a better Kenya
            </p>
            <a 
              href="/get-involved" 
              className="inline-block bg-white text-[var(--kenya-green)] px-8 py-3 rounded-full font-semibold hover:bg-gray-100 transition-colors shadow-lg"
            >
              Get Involved
            </a>
          </div>
        </div>
      )}
    </section>
  )
}
