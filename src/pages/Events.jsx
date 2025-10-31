import React from 'react'

const EventCard = ({title, date, location})=> (
  <div className="p-4 bg-white rounded-lg card-shadow">
    <div className="text-sm text-gray-500">{date} — {location}</div>
    <h4 className="font-semibold mt-1">{title}</h4>
    <button className="mt-3 px-3 py-2 bg-[var(--kenya-green)] text-white rounded-md">RSVP</button>
  </div>
)

export default function Events(){
  const events = [
    {title: 'Townhall - Kisumu', date: 'Nov 12, 2025', location: 'Kisumu Stadium'},
    {title: 'Youth Summit - Nairobi', date: 'Nov 28, 2025', location: 'KICC'},
    {title: 'Roadshow - Mombasa', date: 'Dec 5, 2025', location: 'Mombasa Grounds'}
  ]

  return (
    <section className="max-w-5xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Events</h1>
      <div className="mt-6 grid grid-cols-1 md:grid-cols-3 gap-6">
        {events.map(e=> <EventCard key={e.title} {...e} />)}
      </div>
    </section>
  )
}
