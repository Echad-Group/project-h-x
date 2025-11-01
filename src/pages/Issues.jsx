import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'
import { FAQSchemaGenerator } from '../utils/seoHelpers'

const IssueCard = ({title, desc, faqs = []}) => (
  <div className="p-6 bg-white rounded-lg card-shadow">
    <h4 className="font-semibold text-[var(--kenya-green)]">{title}</h4>
    <p className="mt-2 text-sm text-gray-600">{desc}</p>
  </div>
)

export default function Issues(){
  const { updateMeta } = useMeta();
  
  const issues = [
    {
      title: 'Jobs & Enterprise',
      desc: 'Create pathways for youth employment and support SMEs.',
      faqs: [
        {
          question: 'What is the plan for youth employment?',
          answer: 'Our comprehensive plan includes apprenticeships, startup funding, and skills training programs.'
        },
        {
          question: 'How will you support small businesses?',
          answer: 'Through tax incentives, simplified registration, and access to low-interest loans.'
        }
      ]
    },
    {
      title: 'Healthcare',
      desc: 'Universal primary healthcare and stronger clinics.',
      faqs: [
        {
          question: 'What does universal primary healthcare mean?',
          answer: 'It means ensuring every Kenyan has access to essential health services without financial hardship.'
        }
      ]
    },
    {
      title: 'Education',
      desc: 'Invest in technical training, scholarships and digital learning.',
      faqs: [
        {
          question: 'How will you improve digital learning?',
          answer: 'By providing tablets, internet connectivity, and digital curriculum resources to schools.'
        }
      ]
    },
    {
      title: 'Security & Justice',
      desc: 'Community safety and accountable institutions.',
      faqs: [
        {
          question: 'How will you enhance community safety?',
          answer: 'Through community policing programs, better street lighting, and rapid response systems.'
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
