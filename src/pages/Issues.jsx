import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'
import { FAQSchemaGenerator } from '../utils/seoHelpers'

const IssueCard = ({title, desc, details = [], faqs = []}) => (
  <div className="p-6 bg-white rounded-lg card-shadow">
    <h4 className="text-xl font-semibold text-[var(--kenya-green)]">{title}</h4>
    <p className="mt-2 text-gray-600">{desc}</p>
    
    {details.length > 0 && (
      <div className="mt-4">
        <h5 className="font-medium text-gray-700 mb-2">Key Initiatives:</h5>
        <ul className="list-disc list-inside space-y-1">
          {details.map((detail, index) => (
            <li key={index} className="text-sm text-gray-600">{detail}</li>
          ))}
        </ul>
      </div>
    )}

    {faqs.length > 0 && (
      <div className="mt-4 pt-4 border-t">
        <details>
          <summary className="font-medium text-[var(--kenya-red)] cursor-pointer">
            Common Questions
          </summary>
          <div className="mt-2 space-y-3">
            {faqs.map((faq, index) => (
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
)

export default function Issues(){
  const { updateMeta } = useMeta();
  
  const issues = [
    {
      title: 'Economic Growth & Jobs',
      desc: 'Create pathways for youth employment and support SMEs.',
      details: [
        'Digital economy development and innovation hubs',
        'Youth entrepreneurship funding and mentorship',
        'SME tax incentives and simplified registration',
        'Technical skills training and apprenticeships'
      ],
      faqs: [
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
      desc: 'Universal primary healthcare and stronger clinics.',
      details: [
        'Universal primary healthcare coverage',
        'Modern county health facilities',
        'Preventive care programs',
        'Digital health records system'
      ],
      faqs: [
        {
          question: 'What does universal primary healthcare mean?',
          answer: 'It means ensuring every Kenyan has access to essential health services without financial hardship, supported by modern facilities and digital health systems.'
        }
      ]
    },
    {
      title: 'Education & Skills',
      desc: 'Invest in technical training, scholarships and digital learning.',
      details: [
        'Digital learning infrastructure',
        'STEM education focus',
        'Technical training partnerships',
        'Higher education scholarships'
      ],
      faqs: [
        {
          question: 'How will you improve digital learning?',
          answer: 'By providing tablets, internet connectivity, digital curriculum resources, and teacher training in modern educational technology.'
        }
      ]
    },
    {
      title: 'Security & Justice',
      desc: 'Community safety and accountable institutions.',
      details: [
        'Community policing programs',
        'Modern police equipment and training',
        'Judicial system reforms',
        'Anti-corruption measures'
      ],
      faqs: [
        {
          question: 'How will you enhance community safety?',
          answer: 'Through community policing programs, better street lighting, rapid response systems, and modern police training.'
        }
      ]
    },
    {
      title: 'Infrastructure & Technology',
      desc: 'Modern infrastructure and digital connectivity.',
      details: [
        'Rural internet connectivity',
        'Smart city initiatives',
        'Green energy projects',
        'Transport modernization'
      ],
      faqs: [
        {
          question: 'What is the plan for digital connectivity?',
          answer: 'We will implement a comprehensive rural internet program, smart city initiatives, and support for tech innovation hubs.'
        }
      ]
    },
    {
      title: 'Agriculture & Environment',
      desc: 'Sustainable farming and environmental protection.',
      details: [
        'Modern farming techniques',
        'Climate change adaptation',
        'Water conservation',
        'Forest protection'
      ],
      faqs: [
        {
          question: 'How will you support farmers?',
          answer: 'Through modern farming technology, irrigation systems, market access, and climate-smart agriculture training.'
        }
      ]
    }
  ]

  useEffect(() => {
    // Collect all FAQs for structured data
    const allFaqs = issues.reduce((acc, issue) => [
      ...acc,
      ...issue.faqs
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
  }, []);

  return (
    <section className="max-w-7xl mx-auto px-4 py-12 grid grid-cols-1 md:grid-cols-2 gap-6">
      {issues.map(i=> <IssueCard key={i.title} {...i} />)}
    </section>
  )
}
