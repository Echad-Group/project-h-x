# New Kenya API Test Endpoints

## Base URL
Development: http://localhost:5065

## Volunteers

### Create Volunteer
```http
POST /api/volunteers
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "+254712345678",
  "interests": "Community outreach, Event organizing"
}
```

### Get All Volunteers
```http
GET /api/volunteers
```

### Get Volunteer by ID
```http
GET /api/volunteers/1
```

### Delete Volunteer
```http
DELETE /api/volunteers/1
```

## Contacts

### Create Contact Submission
```http
POST /api/contacts
Content-Type: application/json

{
  "name": "Jane Smith",
  "email": "jane@example.com",
  "subject": "Question about policies",
  "message": "I would like to know more about your education policy."
}
```

### Get All Contacts
```http
GET /api/contacts
```

### Get Contact by ID
```http
GET /api/contacts/1
```

### Delete Contact
```http
DELETE /api/contacts/1
```

## Donations

### Create Donation
```http
POST /api/donations
Content-Type: application/json

{
  "amount": 5000.00,
  "donorName": "Alice Johnson",
  "donorEmail": "alice@example.com",
  "paymentMethod": "M-Pesa",
  "transactionId": "MPESA123456",
  "message": "Keep up the good work!",
  "isAnonymous": false
}
```

### Get All Donations
```http
GET /api/donations
```

### Get Donation Statistics
```http
GET /api/donations/stats
```

### Get Donation by ID
```http
GET /api/donations/1
```

### Delete Donation
```http
DELETE /api/donations/1
```

## Events

### Get All Events
```http
GET /api/events
GET /api/events?includeUnpublished=true
```

### Get Event by ID
```http
GET /api/events/1
```

### Get Event by Slug
```http
GET /api/events/slug/town-hall-nairobi
```

### Get Upcoming Events
```http
GET /api/events/upcoming?limit=10
```

### Get Past Events
```http
GET /api/events/past?limit=10
```

### Create Event (Admin Only)
```http
POST /api/events
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "title": "Nairobi Town Hall",
  "description": "Join us for a community discussion",
  "date": "2024-12-25T14:00:00Z",
  "location": "Nairobi Community Center",
  "city": "Nairobi",
  "region": "Nairobi County",
  "imageUrl": "https://example.com/image.jpg",
  "type": "Town Hall",
  "capacity": 200,
  "isPublished": true
}
```

### Update Event (Admin Only)
```http
PUT /api/events/1
Authorization: Bearer {jwt-token}
Content-Type: application/json

{
  "id": 1,
  "title": "Nairobi Town Hall - Updated",
  "description": "Updated description",
  "date": "2024-12-25T14:00:00Z",
  "location": "New Location",
  "city": "Nairobi",
  "region": "Nairobi County",
  "imageUrl": "https://example.com/image.jpg",
  "type": "Town Hall",
  "capacity": 250,
  "isPublished": true
}
```

### Delete Event (Admin Only)
```http
DELETE /api/events/1
Authorization: Bearer {jwt-token}
```

### Get RSVPs for Event (Admin Only)
```http
GET /api/events/1/rsvps
Authorization: Bearer {jwt-token}
```

### Seed Sample Events (Admin Only)
```http
POST /api/events/seed
Authorization: Bearer {jwt-token}
```

## Event RSVPs

### Create RSVP
```http
POST /api/eventrsvps
Content-Type: application/json

{
  "eventId": 1,
  "name": "Bob Wilson",
  "email": "bob@example.com",
  "phone": "+254723456789",
  "numberOfGuests": 2,
  "specialRequirements": "Wheelchair access needed"
}
```

### Get All RSVPs (Optional: Filter by Event)
```http
GET /api/eventrsvps
GET /api/eventrsvps?eventId=1
```

### Get RSVP Count for Event
```http
GET /api/eventrsvps/count/1
```

Response:
```json
{
  "rsvpCount": 15,
  "totalAttendees": 32
}
```

### Get RSVP by ID
```http
GET /api/eventrsvps/1
```

### Update RSVP
```http
PUT /api/eventrsvps/1
Content-Type: application/json

{
  "id": 1,
  "eventId": 1,
  "name": "Bob Wilson",
  "email": "bob@example.com",
  "phone": "+254723456789",
  "numberOfGuests": 3,
  "specialRequirements": "Wheelchair access needed"
}
```

### Delete RSVP
```http
DELETE /api/eventrsvps/1
```

## Error Responses

### 404 Not Found
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404
}
```

### 409 Conflict (Duplicate)
```json
{
  "message": "A volunteer with this email already exists."
}
```

### 400 Bad Request (Validation Error)
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Email": ["The Email field is not a valid e-mail address."]
  }
}
```

## Swagger Documentation

When running in development mode, you can access interactive API documentation at:
http://localhost:5065/swagger

## Notes

- All endpoints accept and return JSON
- Timestamps (CreatedAt) are automatically set to UTC
- Email uniqueness is enforced for Volunteers
- Event ID (integer) + Email uniqueness is enforced for RSVPs
- Events use slug-based URLs for SEO-friendly routing (e.g., `/events/slug/town-hall-nairobi`)
- Event slugs are auto-generated from titles on create/update
- Admin-only endpoints require a valid JWT token with Admin role
- CORS is enabled for http://localhost:5173 (Vite dev server)
