import React, { useEffect } from 'react'
import { useMeta } from '../components/MetaTags'

const EventCard = ({title, date, location, startDate})=> (
  <div className="p-4 bg-white rounded-lg card-shadow">
    <div className="text-sm text-gray-500">{date} — {location}</div>
    <h4 className="font-semibold mt-1">{title}</h4>
    <button className="mt-3 px-3 py-2 bg-[var(--kenya-green)] text-white rounded-md">RSVP</button>
  </div>
)

export default function Events(){
  const { updateMeta } = useMeta();
  
  const events = [
    {
      title: 'Townhall - Kisumu',
      date: 'Nov 12, 2025',
      location: 'Kisumu Stadium',
      startDate: '2025-11-12T14:00'
    },
    {
      title: 'Youth Summit - Nairobi',
      date: 'Nov 28, 2025',
      location: 'KICC',
      startDate: '2025-11-28T09:00'
    },
    {
      title: 'Roadshow - Mombasa',
      date: 'Dec 5, 2025',
      location: 'Mombasa Grounds',
      startDate: '2025-12-05T10:00'
    }
  ];

  useEffect(() => {
    updateMeta({
      title: 'Events - New Kenya Campaign',
      description: 'Join us at our upcoming events across Kenya. Town halls, youth summits, and community meetings.',
      image: '/assets/og-image.svg',
      structuredData: {
        '@context': 'https://schema.org',
        '@type': 'ItemList',
        itemListElement: events.map((event, index) => ({
          '@type': 'ListItem',
          position: index + 1,
          item: {
            '@type': 'Event',
            name: event.title,
            startDate: event.startDate,
            location: {
              '@type': 'Place',
              name: event.location,
              address: {
                '@type': 'PostalAddress',
                addressLocality: event.location.split(' ')[0],
                addressCountry: 'KE'
              }
            },
            organizer: {
              '@type': 'Organization',
              name: 'New Kenya Campaign'
            }
          }
        }))
      }
    });
  }, []);

  return (
    <section className="max-w-5xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Events</h1>
      <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-6">
        {events.map(e=> <EventCard key={e.title} {...e} />)}
      </div>
    </section>
  )
}
