import React, { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'
import { eventsService, eventRSVPService } from '../services/eventService'

// Event Detail Image Carousel
function EventDetailCarousel({ images }) {
  const [currentIndex, setCurrentIndex] = useState(0);

  if (!images || images.length === 0) return null;

  const goToNext = () => {
    setCurrentIndex((prev) => (prev + 1) % images.length);
  };

  const goToPrevious = () => {
    setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
  };

  const goToSlide = (index) => {
    setCurrentIndex(index);
  };

  return (
    <div className="relative w-full h-96 overflow-hidden bg-gray-100 rounded-lg mb-8 group">
      {/* Images */}
      <div 
        className="flex transition-transform duration-500 ease-out h-full"
        style={{ transform: `translateX(-${currentIndex * 100}%)` }}
      >
        {images.map((image, index) => (
          <img
            key={index}
            src={image}
            alt={`Event image ${index + 1}`}
            className="w-full h-full object-cover flex-shrink-0"
            loading="lazy"
          />
        ))}
      </div>

      {/* Navigation - only show if multiple images */}
      {images.length > 1 && (
        <>
          <button
            onClick={goToPrevious}
            className="absolute left-4 top-1/2 -translate-y-1/2 bg-black/50 text-white p-3 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Previous image"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={goToNext}
            className="absolute right-4 top-1/2 -translate-y-1/2 bg-black/50 text-white p-3 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Next image"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

          {/* Thumbnail Navigation */}
          <div className="absolute bottom-4 left-1/2 -translate-x-1/2 flex gap-2">
            {images.map((image, index) => (
              <button
                key={index}
                onClick={() => goToSlide(index)}
                className={`w-16 h-16 rounded overflow-hidden border-2 transition-all ${
                  index === currentIndex 
                    ? 'border-white scale-110' 
                    : 'border-white/50 hover:border-white/80 opacity-70 hover:opacity-100'
                }`}
                aria-label={`Go to image ${index + 1}`}
              >
                <img src={image} alt={`Thumbnail ${index + 1}`} className="w-full h-full object-cover" />
              </button>
            ))}
          </div>

          {/* Image counter */}
          <div className="absolute top-4 right-4 bg-black/60 text-white text-sm px-3 py-1 rounded-full">
            {currentIndex + 1} / {images.length}
          </div>
        </>
      )}
    </div>
  );
}

const EVENTS = {
  'kisumu-townhall': {
    title: 'Townhall - Kisumu',
    date: 'Nov 12, 2025',
    time: '2:00 PM',
    location: 'Kisumu Stadium',
    startDate: '2025-11-12T14:00',
    description: 'Join us for a community townhall to discuss jobs, youth engagement, and local development priorities. This is an opportunity to engage directly with the campaign team and share your ideas for Kisumu\'s future.',
    attendees: 3200,
    images: [
      'https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=1200',
      'https://images.unsplash.com/photo-1591115765373-5207764f72e7?w=1200'
    ]
  },
  'youth-summit': {
    title: 'Youth Summit - Nairobi',
    date: 'Nov 28, 2025',
    time: '9:00 AM',
    location: 'KICC, Nairobi',
    startDate: '2025-11-28T09:00',
    description: 'A full-day summit focused on youth empowerment, entrepreneurship, and skills development. Features panel discussions, workshops, and networking opportunities with industry leaders.',
    attendees: 5000,
    images: [
      'https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=1200',
      'https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=1200',
      'https://images.unsplash.com/photo-1524178232363-1fb2b075b655?w=1200'
    ]
  },
  'mombasa-roadshow': {
    title: 'Coastal Roadshow - Mombasa',
    date: 'Dec 5, 2025',
    time: '10:00 AM',
    location: 'Mombasa Grounds',
    startDate: '2025-12-05T10:00',
    description: 'Campaign roadshow highlighting coastal development, port expansion, and tourism initiatives. Join us to learn about our plans for the coastal region.',
    attendees: 4500,
    images: [
      'https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=1200',
      'https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=1200'
    ]
  },
  'nakuru-farmers-forum': {
    title: 'Farmers\' Forum - Nakuru',
    date: 'Dec 15, 2025',
    time: '11:00 AM',
    location: 'Nakuru Agricultural Center',
    startDate: '2025-12-15T11:00',
    description: 'Agricultural policy dialogue with farmers focusing on subsidies, market access, and modern farming techniques. Bring your questions and concerns.',
    attendees: 2800,
    images: [
      'https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=1200'
    ]
  },
  'eldoret-rally': {
    title: 'Unity Rally - Eldoret',
    date: 'Dec 22, 2025',
    time: '3:00 PM',
    location: 'Eldoret Sports Club',
    startDate: '2025-12-22T15:00',
    description: 'Major campaign rally bringing together communities to celebrate unity and discuss regional development. Special guest speakers and entertainment.',
    attendees: 6000,
    images: [
      'https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=1200',
      'https://images.unsplash.com/photo-1557804506-669a67965ba0?w=1200'
    ]
  },
  'women-empowerment': {
    title: 'Women Empowerment Forum - Nairobi',
    date: 'Jan 10, 2026',
    time: '1:00 PM',
    location: 'Serena Hotel, Nairobi',
    startDate: '2026-01-10T13:00',
    description: 'Forum celebrating women leadership and announcing the Women Empowerment Fund initiatives. Connect with women leaders and entrepreneurs.',
    attendees: 1500,
    images: [
      'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=1200',
      'https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?w=1200'
    ]
  }
}

export default function EventDetail(){
  const { id } = useParams(); // This is the slug from the URL
  const [event, setEvent] = useState(null);
  const { updateMeta } = useMeta();
  const { t } = useTranslation();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [rsvpCount, setRsvpCount] = useState(0);
  const [backendAvailable, setBackendAvailable] = useState(true);

  useEffect(() => {
    loadEvent();
  }, [id]);

  async function loadEvent() {
    try {
      // Try to fetch from API using slug
      const data = await eventsService.getBySlug(id);
      
      // Transform the data
      const transformedEvent = {
        id: data.slug || data.id,
        eventId: data.id, // Numeric ID for RSVP
        title: data.title,
        slug: data.slug,
        date: new Date(data.date).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' }),
        time: new Date(data.date).toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' }),
        location: data.location,
        city: data.city,
        region: data.region,
        startDate: data.date,
        description: data.description,
        images: data.imageUrl ? [data.imageUrl] : [],
        type: data.type,
        capacity: data.capacity,
        attendees: 0
      };
      
      setEvent(transformedEvent);
      setBackendAvailable(true);
      
      // Update meta tags
      updateMeta({ 
        title: `${transformedEvent.title} - Events`, 
        description: transformedEvent.description, 
        url: `/events/${id}` 
      });
      
      // Fetch RSVP count
      await fetchRsvpCount(transformedEvent.eventId);
    } catch (err) {
      console.error('Error loading event:', err);
      setBackendAvailable(false);
      // Try to load from fallback EVENTS
      const fallbackEvent = EVENTS[id];
      if (fallbackEvent) {
        setEvent(fallbackEvent);
        updateMeta({ 
          title: `${fallbackEvent.title} - Events`, 
          description: fallbackEvent.description, 
          url: `/events/${id}` 
        });
      }
    } finally {
      setLoading(false);
    }
  }

  async function fetchRsvpCount(eventId) {
    if (!eventId) return;
    try {
      const data = await eventRSVPService.getCount(eventId);
      setRsvpCount(data.totalAttendees || 0);
    } catch (err) {
      console.error('Error fetching RSVP count:', err);
    }
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading event...</p>
        </div>
      </div>
    );
  }

  if(!event) {
    return (
      <div className="max-w-4xl mx-auto px-4 py-12">
        <div className="text-center">
          <h1 className="text-2xl font-bold text-gray-900 mb-4">Event not found</h1>
          <Link to="/events" className="text-[var(--kenya-green)] hover:underline">
            ← Back to Events
          </Link>
        </div>
      </div>
    );
  }

  async function handleRSVP(e){
    e.preventDefault();
    setError(null);
    setSuccess(false);
    setLoading(true);

    const formData = new FormData(e.target);
    const data = {
      eventId: event.eventId || event.id, // Use numeric ID
      name: formData.get('name'),
      email: formData.get('email'),
      phone: formData.get('phone') || null,
      numberOfGuests: parseInt(formData.get('guests') || '1'),
      specialRequirements: formData.get('requirements') || null
    };

    console.log('RSVP submitted for', event.eventId, data);

    try {
      await eventRSVPService.create(data);
      setSuccess(true);
      e.target.reset();
      
      // Refresh RSVP count
      await fetchRsvpCount(event.eventId);
    } catch (err) {
      console.error('RSVP error:', err);
      
      if (err.response?.status === 409) {
        setError(t('events.errors.duplicate') || 'You have already RSVPed for this event.');
      } else if (err.response?.data?.errors) {
        const validationErrors = Object.values(err.response.data.errors).flat();
        setError(validationErrors.join(', '));
      } else if (err.code === 'ERR_NETWORK') {
        setError(t('events.errors.network') || 'Network error. Please check if the backend is running.');
      } else {
        setError(t('events.errors.general') || 'Failed to submit RSVP. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="bg-gray-50 min-h-screen">
      <section className="max-w-4xl mx-auto px-4 py-12">
        {/* Back Link */}
        <Link 
          to="/events" 
          className="inline-flex items-center text-[var(--kenya-green)] hover:text-[var(--kenya-red)] mb-6 transition-colors"
        >
          ← Back to Events
        </Link>

        <div className="bg-white rounded-lg card-shadow overflow-hidden">
          <div className="p-8">
            {/* Event Header */}
            <div className="mb-6">
              <div className="flex items-center gap-4 text-sm text-gray-600 mb-3">
                <div className="flex items-center gap-2">
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
                  </svg>
                  <span>{event.date} at {event.time}</span>
                </div>
                <span>•</span>
                <div className="flex items-center gap-2">
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                  </svg>
                  <span>{event.location}</span>
                </div>
              </div>
              
              <h1 className="text-4xl font-bold text-gray-900 mb-3">{event.title}</h1>
              
              {rsvpCount > 0 && (
                <div className="flex items-center gap-2 text-sm text-gray-600">
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                  <span><strong>{rsvpCount}</strong> people attending</span>
                </div>
              )}
            </div>
          </div>

          {/* Event Images */}
          {event.images && event.images.length > 0 && (
            <div className="px-8">
              <EventDetailCarousel images={event.images} />
            </div>
          )}

          {/* Event Description and RSVP */}
          <div className="px-8 pb-8">
            <div className="prose prose-lg max-w-none mb-8">
              <h2 className="text-2xl font-bold text-gray-900 mb-3">About This Event</h2>
              <p className="text-gray-700 leading-relaxed">{event.description}</p>
            </div>

            {/* RSVP Form */}
            <div className="bg-gray-50 rounded-lg p-6 border border-gray-200">
              <h3 className="text-xl font-bold text-gray-900 mb-4">Register to Attend</h3>
              
              {error && (
                <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded mb-4">
                  {error}
                </div>
              )}
              
              {success && (
                <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded mb-4">
                  {t('events.success') || 'Thank you! Your RSVP has been confirmed.'}
                </div>
              )}
              
              <form onSubmit={handleRSVP} className="grid grid-cols-1 gap-4">
                <input 
                  name="name" 
                  placeholder={t('events.form.name')} 
                  required 
                  className="fluent-input" 
                />
                <input 
                  name="email" 
                  type="email" 
                  placeholder={t('events.form.email')} 
                  required 
                  className="fluent-input" 
                />
                <input 
                  name="phone" 
                  type="tel"
                  placeholder={t('events.form.phone')} 
                  className="fluent-input" 
                />
                <div>
                  <label className="block text-sm font-medium mb-1">Number of Guests</label>
                  <input 
                    name="guests" 
                    type="number"
                    min="1"
                    max="10"
                    defaultValue="1"
                    className="fluent-input" 
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Special Requirements (Optional)</label>
                  <textarea 
                    name="requirements"
                    rows="2"
                    placeholder="e.g., Wheelchair access, dietary requirements"
                    className="fluent-input"
                  />
                </div>
                <button 
                  type="submit"
                  disabled={loading}
                  className="fluent-btn fluent-btn-primary disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {loading ? t('events.form.submitting') || 'Submitting...' : t('events.rsvp')}
                </button>
              </form>
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}
