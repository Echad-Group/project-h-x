import React, { useEffect } from 'react'
import { useParams } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

const EVENTS = {
  'kisumu-townhall': {
    title: 'Townhall - Kisumu',
    date: 'Nov 12, 2025',
    location: 'Kisumu Stadium',
    startDate: '2025-11-12T14:00',
    description: 'A community townhall to discuss jobs, youth engagement and local priorities.'
  }
}

export default function EventDetail(){
  const { id } = useParams();
  const event = EVENTS[id];
  const { updateMeta } = useMeta();
  const { t } = useTranslation();

  useEffect(() => {
    if(event) updateMeta({ title: `${event.title} - Events`, description: event.description, url: `/events/${id}` });
  }, [id]);

  if(!event) return <div className="p-6">Event not found</div>

  function handleRSVP(e){
    e.preventDefault();
    const data = Object.fromEntries(new FormData(e.target).entries());
    console.log('RSVP submitted for', id, data);
    alert('Thanks — your RSVP has been recorded (demo).');
    e.target.reset();
  }

  return (
    <section className="max-w-3xl mx-auto px-4 py-12 bg-white rounded card-shadow">
      <div className="text-sm text-gray-500">{event.date} — {event.location}</div>
      <h1 className="text-2xl font-bold mt-2">{event.title}</h1>
      <p className="mt-4 text-gray-700">{event.description}</p>

      <form onSubmit={handleRSVP} className="mt-6 grid grid-cols-1 gap-3">
        <input name="name" placeholder={t('events.form.name')} required className="fluent-input" />
        <input name="email" type="email" placeholder={t('events.form.email')} required className="fluent-input" />
        <input name="phone" placeholder={t('events.form.phone')} className="fluent-input" />
        <button className="fluent-btn fluent-btn-primary">{t('events.rsvp')}</button>
      </form>
    </section>
  )
}
