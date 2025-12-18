# Events System Implementation

## Overview

The Events system provides full campaign event management with RSVP functionality. It includes both backend API and frontend UI with SEO-friendly slug-based routing.

## Database Schema

### Event Model
```csharp
public class Event
{
    public int Id { get; set; }
    public string Slug { get; set; }  // Auto-generated from Title
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string? ImageUrl { get; set; }
    public string Type { get; set; }  // Town Hall, Rally, Fundraiser, etc.
    public int? Capacity { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public ICollection<EventRSVP> RSVPs { get; set; }
}
```

### EventRSVP Model
```csharp
public class EventRSVP
{
    public int Id { get; set; }
    public int EventId { get; set; }  // Foreign key to Event
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public int NumberOfGuests { get; set; }  // 1-10
    public string? SpecialRequirements { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    [JsonIgnore]
    public Event Event { get; set; }
}
```

### Key Design Decisions

1. **Integer EventId**: Changed from string slugs to integers for database relationships
2. **Slug Field**: Added separate slug field for SEO-friendly URLs
3. **Unique Index**: Event.Slug has unique index for fast lookups
4. **Cascade Delete**: EventRSVPs are deleted when parent Event is deleted
5. **JsonIgnore**: Navigation properties marked to prevent circular reference errors
6. **DTO Pattern**: Controllers use DTOs for RSVP creation to avoid validation issues

## API Endpoints

### Events Controller (`/api/events`)

#### Public Endpoints
- `GET /api/events` - List all published events (or all with `?includeUnpublished=true`)
- `GET /api/events/{id}` - Get event by numeric ID
- `GET /api/events/slug/{slug}` - Get event by slug (for SEO routing)
- `GET /api/events/upcoming` - Get future events
- `GET /api/events/past` - Get past events

#### Admin-Only Endpoints (Requires JWT with Admin role)
- `POST /api/events` - Create new event (auto-generates slug)
- `PUT /api/events/{id}` - Update event (regenerates slug if title changed)
- `DELETE /api/events/{id}` - Delete event
- `GET /api/events/{id}/rsvps` - Get all RSVPs for an event
- `POST /api/events/seed` - Seed sample events

### EventRSVPs Controller (`/api/eventrsvps`)

#### Public Endpoints
- `POST /api/eventrsvps` - Submit RSVP (uses DTO to accept eventId as integer)
- `GET /api/eventrsvps/count/{eventId}` - Get RSVP count and total attendees

#### Admin-Only Endpoints
- `GET /api/eventrsvps` - Get all RSVPs (optional `?eventId=` filter)
- `GET /api/eventrsvps/{id}` - Get specific RSVP
- `PUT /api/eventrsvps/{id}` - Update RSVP
- `DELETE /api/eventrsvps/{id}` - Delete RSVP

## Frontend Implementation

### Route Structure
```
/events              → Events.jsx (list all events)
/events/:slug        → EventDetail.jsx (individual event with RSVP form)
```

### Service Layer (`src/services/eventService.js`)

**eventsService** - For event operations:
- `getAll(includeUnpublished)`
- `getById(id)`
- `getBySlug(slug)` - Used for detail page routing
- `getUpcoming(limit)`
- `getPast(limit)`
- `create(eventData)` - Admin only
- `update(id, eventData)` - Admin only
- `delete(id)` - Admin only

**eventRSVPService** - For RSVP operations:
- `getAll(eventId)`
- `getById(id)`
- `getCount(eventId)` - Returns `{ rsvpCount, totalAttendees }`
- `create(rsvpData)` - Expects `{ eventId: int, name, email, ... }`
- `update(id, rsvpData)`
- `delete(id)`

### Events List Page (`Events.jsx`)

**Features:**
- Fetches from `/api/events`
- Displays loading state
- Falls back to hardcoded data if API unavailable
- Links to event detail using slug: `/events/${event.slug}`

**Data Transformation:**
```javascript
const transformedEvent = {
  id: event.slug,        // For routing
  eventId: event.id,     // Numeric ID for RSVP submission
  slug: event.slug,
  title: event.title,
  date: formatted,
  time: formatted,
  // ... other fields
};
```

### Event Detail Page (`EventDetail.jsx`)

**Features:**
- Uses slug from URL params to fetch event via `eventsService.getBySlug(slug)`
- Displays full event information
- RSVP form with validation
- Duplicate prevention (backend checks email + eventId)
- Live RSVP count display
- Falls back to hardcoded EVENTS object if API unavailable

**RSVP Submission:**
```javascript
const data = {
  eventId: event.eventId,  // Uses numeric ID, not slug
  name: formData.get('name'),
  email: formData.get('email'),
  phone: formData.get('phone') || null,
  numberOfGuests: parseInt(formData.get('guests') || '1'),
  specialRequirements: formData.get('requirements') || null
};

await eventRSVPService.create(data);
```

## Issues Encountered & Solutions

### Issue 1: Type Mismatch (String vs Integer EventId)
**Problem:** EventRSVP.EventId was string, Event.Id was integer  
**Solution:** Changed EventRSVP.EventId to int, updated all comparisons in controllers  
**Files Modified:** EventRSVP.cs, EventRSVPsController.cs, EventsController.cs

### Issue 2: "The Event field is required" Validation Error
**Problem:** Navigation property `Event` was being validated during POST  
**Solution 1:** Added `[JsonIgnore]` to navigation property  
**Solution 2:** Created `CreateEventRSVPDto` to accept clean data without navigation properties  
**Files Modified:** EventRSVP.cs, EventRSVPsController.cs, EventRSVPDto.cs

### Issue 3: Circular Reference in JSON Serialization
**Problem:** Event → RSVPs → Event caused infinite loop  
**Solution:** Added `[JsonIgnore]` to EventRSVP.Event navigation property  
**Files Modified:** EventRSVP.cs

### Issue 4: Slug-based Routing with Integer Foreign Keys
**Problem:** Frontend needs slugs for URLs, backend needs integers for relationships  
**Solution:** Store both in frontend state: `slug` for routing, `eventId` for API calls  
**Files Modified:** EventDetail.jsx, Events.jsx

## Seed Script Usage

### Seed Events
```powershell
# Requires admin JWT token
.\backend\seed-events.ps1 -Token "eyJhbGciOiJIUzI1NiIsInR5cC..."
```

Creates 6 sample events:
1. Nairobi Town Hall Meeting
2. Mombasa Community Rally
3. Kisumu Youth Summit
4. Nakuru Farmers' Forum
5. Eldoret Healthcare Discussion
6. Virtual Town Hall

### Clear Events (Before Re-seeding)
```powershell
.\backend\clear-events.ps1 -Token "eyJhbGciOiJIUzI1NiIsInR5cC..."
```

Deletes all events and associated RSVPs (cascade delete).

## Testing Checklist

- [ ] Event listing loads from API
- [ ] Slug-based URLs work (e.g., `/events/nairobi-town-hall`)
- [ ] Event detail page loads via slug
- [ ] RSVP form validation works
- [ ] RSVP submission succeeds with valid data
- [ ] Duplicate RSVP prevented (same email for same event)
- [ ] RSVP count updates after submission
- [ ] Guest count calculated correctly
- [ ] Admin can view all RSVPs via API
- [ ] Unpublished events hidden from public
- [ ] Upcoming/past event filtering works
- [ ] Falls back to hardcoded data when API unavailable

## Migration Commands

```bash
# Create migration for Events schema
cd backend/NewKenyaAPI
dotnet ef migrations add AddEventSlugAndFixRSVP

# Apply migration
dotnet ef database update

# Rollback if needed
dotnet ef database update PreviousMigrationName
```

## Future Enhancements

- [ ] Add event capacity tracking (prevent RSVPs when full)
- [ ] Email confirmation for RSVPs
- [ ] Calendar export (.ics files)
- [ ] Event categories/tags
- [ ] Image upload for events
- [ ] Event search and filtering
- [ ] Admin UI for managing events (currently API-only)
- [ ] RSVP cancellation flow
- [ ] Waitlist functionality for full events
- [ ] Event reminders (push notifications)

## References

- API Testing Documentation: `backend/API_TESTING.md`
- Database Models: `backend/NewKenyaAPI/Models/`
- Controllers: `backend/NewKenyaAPI/Controllers/`
- Frontend Pages: `src/pages/Events.jsx`, `src/pages/EventDetail.jsx`
- Service Layer: `src/services/eventService.js`
