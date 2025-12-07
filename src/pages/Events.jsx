import React, { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useMeta } from '../components/MetaTags'
import { useTranslation } from 'react-i18next'

// Image Carousel Component for Event Cards
function EventImageCarousel({ images }) {
  const [currentIndex, setCurrentIndex] = useState(0);

  if (!images || images.length === 0) {
    return (
      <div className="w-full h-48 bg-gradient-to-br from-green-100 to-red-100 flex items-center justify-center">
        <svg className="w-16 h-16 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
        </svg>
      </div>
    );
  }

  const goToNext = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIndex((prev) => (prev + 1) % images.length);
  };

  const goToPrevious = (e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIndex((prev) => (prev - 1 + images.length) % images.length);
  };

  const goToSlide = (index, e) => {
    e.preventDefault();
    e.stopPropagation();
    setCurrentIndex(index);
  };

  return (
    <div className="relative w-full h-48 overflow-hidden bg-gray-100 group">
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

      {/* Navigation Arrows - only show if multiple images */}
      {images.length > 1 && (
        <>
          <button
            onClick={goToPrevious}
            className="absolute left-2 top-1/2 -translate-y-1/2 bg-black/50 text-white p-2 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Previous image"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button
            onClick={goToNext}
            className="absolute right-2 top-1/2 -translate-y-1/2 bg-black/50 text-white p-2 rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-black/70"
            aria-label="Next image"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>

          {/* Dots Indicator */}
          <div className="absolute bottom-2 left-1/2 -translate-x-1/2 flex gap-1.5">
            {images.map((_, index) => (
              <button
                key={index}
                onClick={(e) => goToSlide(index, e)}
                className={`w-2 h-2 rounded-full transition-all ${
                  index === currentIndex 
                    ? 'bg-white w-6' 
                    : 'bg-white/60 hover:bg-white/80'
                }`}
                aria-label={`Go to slide ${index + 1}`}
              />
            ))}
          </div>
        </>
      )}

      {/* Image counter */}
      {images.length > 1 && (
        <div className="absolute top-2 right-2 bg-black/60 text-white text-xs px-2 py-1 rounded-full">
          {currentIndex + 1} / {images.length}
        </div>
      )}
    </div>
  );
}

const EventCard = ({id, title, date, location, time, description, images, attendees})=> {
  const { t } = useTranslation();
  
  return (
    <div className="bg-white rounded-lg overflow-hidden card-shadow hover:shadow-xl transition-shadow flex flex-col">
      {/* Image Carousel */}
      <Link to={`/events/${id}`}>
        <EventImageCarousel images={images} />
      </Link>
      
      <div className="p-6 flex-1 flex flex-col">
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-2">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
          <span>{date}</span>
          {time && <><span>•</span><span>{time}</span></>}
        </div>
        
        <div className="flex items-center gap-2 text-sm text-gray-500 mb-3">
          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          <span>{location}</span>
        </div>
        
        <h4 className="font-bold text-lg text-gray-900 mb-2">{title}</h4>
        
        {description && (
          <p className="text-sm text-gray-600 mb-4 line-clamp-2 flex-1">{description}</p>
        )}
        
        {attendees && (
          <div className="flex items-center gap-2 text-xs text-gray-500 mb-4">
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
            <span>{attendees} attending</span>
          </div>
        )}
        
        <Link 
          to={`/events/${id}`}
          className="px-4 py-2 bg-[var(--kenya-green)] text-white rounded-md text-center font-medium hover:bg-[var(--kenya-red)] transition-colors"
        >
          {t('events.rsvp')}
        </Link>
      </div>
    </div>
  );
};

export default function Events(){
  const { updateMeta } = useMeta();
  const { t } = useTranslation();
  
  const events = [
    {
      id: 'kisumu-townhall',
      title: 'Townhall - Kisumu',
      date: 'Nov 12, 2025',
      time: '2:00 PM',
      location: 'Kisumu Stadium',
      startDate: '2025-11-12T14:00',
      description: 'Join us for a community townhall to discuss jobs, youth engagement, and local development priorities.',
      attendees: 3200,
      images: [
        'https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?w=800',
        'https://images.unsplash.com/photo-1591115765373-5207764f72e7?w=800'
      ]
    },
    {
      id: 'youth-summit',
      title: 'Youth Summit - Nairobi',
      date: 'Nov 28, 2025',
      time: '9:00 AM',
      location: 'KICC, Nairobi',
      startDate: '2025-11-28T09:00',
      description: 'A full-day summit focused on youth empowerment, entrepreneurship, and skills development.',
      attendees: 5000,
      images: [
        'https://images.unsplash.com/photo-1523240795612-9a054b0db644?w=800',
        'https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=800',
        'https://images.unsplash.com/photo-1524178232363-1fb2b075b655?w=800'
      ]
    },
    {
      id: 'mombasa-roadshow',
      title: 'Coastal Roadshow - Mombasa',
      date: 'Dec 5, 2025',
      time: '10:00 AM',
      location: 'Mombasa Grounds',
      startDate: '2025-12-05T10:00',
      description: 'Campaign roadshow highlighting coastal development, port expansion, and tourism initiatives.',
      attendees: 4500,
      images: [
        'https://images.unsplash.com/photo-1578575437130-527eed3abbec?w=800',
        'https://images.unsplash.com/photo-1559827260-dc66d52bef19?w=800'
      ]
    },
    {
      id: 'nakuru-farmers-forum',
      title: "Farmers' Forum - Nakuru",
      date: 'Dec 15, 2025',
      time: '11:00 AM',
      location: 'Nakuru Agricultural Center',
      startDate: '2025-12-15T11:00',
      description: 'Agricultural policy dialogue with farmers focusing on subsidies, market access, and modern farming.',
      attendees: 2800,
      images: [
        'https://images.unsplash.com/photo-1625246333195-78d9c38ad449?w=800'
      ]
    },
    {
      id: 'eldoret-rally',
      title: 'Unity Rally - Eldoret',
      date: 'Dec 22, 2025',
      time: '3:00 PM',
      location: 'Eldoret Sports Club',
      startDate: '2025-12-22T15:00',
      description: 'Major campaign rally bringing together communities to celebrate unity and discuss regional development.',
      attendees: 6000,
      images: [
        'https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?w=800',
        'https://images.unsplash.com/photo-1557804506-669a67965ba0?w=800'
      ]
    },
    {
      id: 'women-empowerment',
      title: 'Women Empowerment Forum - Nairobi',
      date: 'Jan 10, 2026',
      time: '1:00 PM',
      location: 'Serena Hotel, Nairobi',
      startDate: '2026-01-10T13:00',
      description: 'Forum celebrating women leadership and announcing the Women Empowerment Fund initiatives.',
      attendees: 1500,
      images: [
        'https://images.unsplash.com/photo-1573496359142-b8d87734a5a2?w=800',
        'https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?w=800'
      ]
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
    <section className="max-w-7xl mx-auto px-4 py-12">
      <div className="mb-8">
        <h1 className="text-4xl font-bold text-gray-900">{t('events.title')}</h1>
        <p className="mt-2 text-lg text-gray-600">Join us at upcoming campaign events across Kenya</p>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {events.map(e=> <EventCard key={e.id} {...e} />)}
      </div>
    </section>
  )
}
