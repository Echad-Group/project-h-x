import React, { useState, useEffect } from 'react'
import api from '../services/api'

const fallbackItems = [
  {title: 'Jobs & Growth', summary: 'Programs to unlock private sector hiring and support SMEs.', icon: '💼'},
  {title: 'Healthcare', summary: 'Invest in clinics, preventive care and health workers.', icon: '🏥'},
  {title: 'Education', summary: 'Skills, technical training and digital classrooms.', icon: '📚'}
]

export default function IssuesPreview(){
  const [issues, setIssues] = useState(fallbackItems);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadIssues();
  }, []);

  async function loadIssues() {
    try {
      const response = await api.get('/issues');
      if (response.data && response.data.length > 0) {
        // Take first 3 issues
        setIssues(response.data.slice(0, 3));
      }
    } catch (error) {
      console.log('Using fallback issues data');
    } finally {
      setLoading(false);
    }
  }

  return (
    <section aria-label="Key issues">
      <h2 className="text-2xl font-bold">Key Issues</h2>
      <div className="mt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
        {issues.map(issue => (
          <div key={issue.title} className="p-5 bg-white rounded-lg card-shadow hover:shadow-lg transition-shadow">
            <div className="flex items-center gap-2 mb-2">
              {issue.icon && <span className="text-2xl">{issue.icon}</span>}
              <h3 className="text-lg font-semibold text-[var(--kenya-red)]">{issue.title}</h3>
            </div>
            <p className="mt-2 text-sm text-gray-600">{issue.summary || issue.desc}</p>
            <a className="mt-3 inline-block text-[var(--kenya-green)] font-medium hover:underline" href="/issues">Learn more →</a>
          </div>
        ))}
      </div>
    </section>
  )
}
