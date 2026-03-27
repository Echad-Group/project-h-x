# Events Module

Last updated: March 27, 2026

## Purpose

The Events module powers public campaign event discovery, event detail pages, RSVP submission, admin event publishing, and event-triggered push notifications.

## Backend

### Core entities
- `Event`
- `EventRSVP`

### Event API
Base route: `/api/events`

Public endpoints:
- `GET /api/events`
- `GET /api/events/{id}`
- `GET /api/events/slug/{slug}`
- `GET /api/events/upcoming`
- `GET /api/events/past`

Admin endpoints:
- `POST /api/events`
- `PUT /api/events/{id}`
- `DELETE /api/events/{id}`
- `GET /api/events/{id}/rsvps`
- `POST /api/events/seed`

### RSVP API
Base route: `/api/eventrsvps`

Endpoints:
- `GET /api/eventrsvps`
- `GET /api/eventrsvps/{id}`
- `GET /api/eventrsvps/count/{eventId}`
- `POST /api/eventrsvps`
- `PUT /api/eventrsvps/{id}`
- `DELETE /api/eventrsvps/{id}`

### Current implementation details
- Slugs are used for public routing and SEO-friendly links
- Numeric event IDs are used for persistence and RSVP relationships
- Duplicate RSVP prevention is handled server-side
- Event publishing and updates can dispatch category-tagged push notifications via `PushNotificationDispatcher`

## Frontend

### Public pages
- `src/pages/Events.jsx`
- `src/pages/EventDetail.jsx`

Current behavior:
- list page fetches current event inventory
- detail page loads a single event and RSVP counts
- users can submit RSVP data with guest count and special requirements
- upcoming and past segmentation is supported by the API

### Supporting services
- `src/services/eventService.js`

### Related UI areas
- Home page preview components can surface event content
- Notification flows can drive users back to event pages

## Admin and publishing behavior

- Event creation and updates are backend-authorized
- Admins can retrieve RSVP rosters for a specific event
- Sample event data can be seeded with PowerShell

## Data and operational notes

- Event records live in SQLite
- Slug uniqueness is indexed for efficient lookup
- RSVP counts are available through a dedicated endpoint for lightweight UI refreshes

## Current Constraints

- No waitlist or capacity-enforcement workflow exists yet
- No calendar export or reminder scheduling feature exists yet
- Event image management is URL-driven rather than upload-first
