import React, { useEffect, useState } from 'react'
import { useMeta } from '../components/MetaTags'
import { FAQSchemaGenerator } from '../utils/seoHelpers'
import api from '../services/api'

const IssueCard = ({title, desc, summary, icon, color, details = [], initiatives = [], faqs = [], questions = []}) => {
  // Support both API response format (summary, initiatives, questions) and fallback format (desc, details, faqs)
  const description = summary || desc;
  const displayInitiatives = initiatives.length > 0 ? initiatives : details;
  const displayQuestions = questions.length > 0 ? questions : faqs;
  
  return (
    <div className="p-6 bg-white rounded-lg card-shadow">
      <div className="flex items-start gap-3 mb-3">
        {icon && <span className="text-3xl">{icon}</span>}
        <h4 className="text-xl font-semibold text-[var(--kenya-green)]">{title}</h4>
      </div>
      <p className="mt-2 text-gray-600">{description}</p>
      
      {displayInitiatives.length > 0 && (
        <div className="mt-4">
          <h5 className="font-medium text-gray-700 mb-2">Key Initiatives:</h5>
          <ul className="list-disc list-inside space-y-1">
            {displayInitiatives.map((item, index) => (
              <li key={index} className="text-sm text-gray-600">{item.title || item}</li>
            ))}
          </ul>
        </div>
      )}

      {displayQuestions.length > 0 && (
        <div className="mt-4 pt-4 border-t">
          <details>
            <summary className="font-medium text-[var(--kenya-red)] cursor-pointer">
              Common Questions
            </summary>
            <div className="mt-2 space-y-3">
              {displayQuestions.map((faq, index) => (
                <div key={index} className="pl-4">
                  <p className="text-sm font-medium text-gray-700">{faq.question}</p>
                  <p className="mt-1 text-sm text-gray-600">{faq.answer}</p>
                </div>
              ))}
            </div>
          </details>
        </div>
      )}
    </div>
  );
};

const fallbackIssues = [
  {
    title: 'Economic Growth & Jobs',
    summary: 'Create pathways for youth employment and support SMEs.',
    icon: '💼',
    initiatives: [
      { title: 'Digital economy development and innovation hubs' },
      { title: 'Youth entrepreneurship funding and mentorship' },
      { title: 'SME tax incentives and simplified registration' },
      { title: 'Technical skills training and apprenticeships' }
    ],
    questions: [
      {
        question: 'What is the plan for youth employment?',
        answer: 'Our comprehensive plan includes apprenticeships, startup funding, and skills training programs focused on digital economy and future industries.'
      },
      {
        question: 'How will you support small businesses?',
        answer: 'Through tax incentives, simplified registration, access to low-interest loans, and digital transformation support.'
      }
    ]
  },
  {
    title: 'Healthcare Access',
    summary: 'Universal primary healthcare and stronger clinics.',
    icon: '🏥',
    initiatives: [
      { title: 'Universal primary healthcare coverage' },
      { title: 'Modern county health facilities' },
      { title: 'Preventive care programs' },
      { title: 'Digital health records system' }
    ],
    questions: [
      {
        question: 'What does universal primary healthcare mean?',
        answer: 'It means ensuring every Kenyan has access to essential health services without financial hardship, supported by modern facilities and digital health systems.'
      }
    ]
  },
  {
    title: 'Education & Skills',
    summary: 'Invest in technical training, scholarships and digital learning.',
    icon: '📚',
    initiatives: [
      { title: 'Digital learning infrastructure' },
      { title: 'STEM education focus' },
      { title: 'Technical training partnerships' },
      { title: 'Higher education scholarships' }
    ],
    questions: [
      {
        question: 'How will you improve digital learning?',
        answer: 'By providing tablets, internet connectivity, digital curriculum resources, and teacher training in modern educational technology.'
      }
    ]
  },
  {
    title: 'Security & Justice',
    summary: 'Community safety and accountable institutions.',
    icon: '🛡️',
    initiatives: [
      { title: 'Community policing programs' },
      { title: 'Modern police equipment and training' },
      { title: 'Judicial system reforms' },
      { title: 'Anti-corruption measures' }
    ],
    questions: [
      {
        question: 'How will you enhance community safety?',
        answer: 'Through community policing programs, better street lighting, rapid response systems, and modern police training.'
      }
    ]
  },
  {
    title: 'Infrastructure & Technology',
    summary: 'Modern infrastructure and digital connectivity.',
    icon: '🌐',
    initiatives: [
      { title: 'Rural internet connectivity' },
      { title: 'Smart city initiatives' },
      { title: 'Green energy projects' },
      { title: 'Transport modernization' }
    ],
    questions: [
      {
        question: 'What is the plan for digital connectivity?',
        answer: 'We will implement a comprehensive rural internet program, smart city initiatives, and support for tech innovation hubs.'
      }
    ]
  },
  {
    title: 'Agriculture & Environment',
    summary: 'Sustainable farming and environmental protection.',
    icon: '🌾',
    initiatives: [
      { title: 'Modern farming techniques' },
      { title: 'Climate change adaptation' },
      { title: 'Water conservation' },
      { title: 'Forest protection' }
    ],
    questions: [
      {
        question: 'How will you support farmers?',
        answer: 'Through modern farming technology, irrigation systems, market access, and climate-smart agriculture training.'
      }
    ]
  }
];

export default function Issues(){
  const { updateMeta } = useMeta();
  const [issues, setIssues] = useState([]);
  const [loading, setLoading] = useState(true);
  const [backendAvailable, setBackendAvailable] = useState(true);

  useEffect(() => {
    loadIssues();
  }, []);

  useEffect(() => {
    if (issues.length === 0) return;
    
    // Collect all FAQs for structured data
    const allFaqs = issues.reduce((acc, issue) => [
      ...acc,
      ...(issue.questions || issue.faqs || [])
    ], []);

    // Generate breadcrumbs
    const breadcrumbs = [
      { name: 'Home', url: '/' },
      { name: 'Issues', url: '/issues' }
    ];

    updateMeta({
      title: 'Key Issues - New Kenya Campaign',
      description: 'Our stance on jobs, healthcare, education, and security. Learn about our detailed plans for Kenya\'s future.',
      image: '/assets/og-image.svg',
      url: '/issues',
      breadcrumbs,
      structuredData: FAQSchemaGenerator(allFaqs)
    });
  }, [issues, updateMeta]);

  async function loadIssues() {
    try {
      const response = await api.get('/issues');
      setIssues(response.data);
      setBackendAvailable(true);
    } catch (error) {
      console.log('Backend unavailable, using fallback data');
      setBackendAvailable(false);
      setIssues(fallbackIssues);
    } finally {
      setLoading(false);
    }
  }
  
  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading issues...</p>
        </div>
      </div>
    );
  }

  return (
    <section className="max-w-7xl mx-auto px-4 py-12 grid grid-cols-1 md:grid-cols-2 gap-6">
      {issues.map(i=> <IssueCard key={i.title} {...i} />)}
    </section>
  )
}
