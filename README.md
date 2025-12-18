# New Kenya — Campaign Platform

This repository contains a modern, full-stack application for a presidential campaign website themed "New Kenya". It uses Kenyan national colors and a clean, modern aesthetic with bilingual support (English/Swahili).

## Project Structure (Monorepo)

```
project-h-x/
├── frontend/               # React + Vite application
│   ├── src/
│   │   ├── components/    # Reusable UI components
│   │   ├── pages/         # Page components
│   │   ├── locales/       # i18n translations (en, sw)
│   │   └── services/      # PWA and notification services
│   └── public/            # Static assets
└── backend/               # .NET 8 Web API
    └── NewKenyaAPI/
        ├── Controllers/   # API endpoints
        ├── Models/        # Data models
        ├── Data/          # DbContext
        └── Migrations/    # EF Core migrations
```

## Requirements

### Frontend
- Node.js 16+ and npm (or yarn)

### Backend
- .NET 8 SDK
- SQLite (included with EF Core)

## Installation & Setup

### Frontend Setup

```powershell
# From repository root
npm install

# Start dev server
npm run dev
```

The frontend will be available at http://localhost:5173

### Backend Setup

```powershell
# Navigate to backend directory
cd backend/NewKenyaAPI

# Restore packages
dotnet restore

# Run migrations (creates database)
dotnet ef database update

# Start API server
dotnet run
```

The API will be available at https://localhost:7000 (or http://localhost:5000)

## Available Scripts

### Frontend
- `npm run dev` — Start Vite dev server
- `npm run build` — Build production static assets
- `npm run preview` — Preview production build

### Backend
- `dotnet run` — Start API server
- `dotnet ef migrations add <MigrationName>` — Create new migration
- `dotnet ef database update` — Apply migrations to database
- `dotnet test` — Run unit tests

## Features

### Frontend
- ✅ Bilingual support (English/Swahili) with i18next
- ✅ Responsive design with Tailwind CSS
- ✅ Progressive Web App (PWA) capabilities
- ✅ News section with filtering, search, and pagination
- ✅ Events management with RSVP functionality
- ✅ Volunteer signup forms
- ✅ Contact forms
- ✅ Donation system (placeholder for payment integration)
- ✅ Social media integration
- ✅ Image carousels for news and events

### Backend
- ✅ RESTful API with .NET 8
- ✅ Entity Framework Core with SQLite
- ✅ JWT Authentication with role-based authorization
- ✅ CORS configured for frontend
- ✅ Swagger/OpenAPI documentation
- ✅ Data validation and error handling
- ✅ Seed scripts for sample data
- ✅ Main entities:
  - **Authentication**: Users with roles (Admin, Volunteer, User, Moderator, TeamLead)
  - **Issues**: Policy platform with initiatives and questions
  - **Events**: Campaign events with slug-based routing
  - **EventRSVPs**: Event registrations with attendee info
  - **Volunteers**: Volunteer signups with interests
  - **Contacts**: Contact form submissions
  - **Donations**: Donation records
  - **Organizations**: Volunteer units and teams with assignments

## API Endpoints

### Authentication
- `POST /api/auth/register` — Register new user
- `POST /api/auth/login` — Login and receive JWT token
- `POST /api/auth/refresh` — Refresh expired token

### Issues (Policy Platform)
- `GET /api/issues` — Get all published issues
- `GET /api/issues/{id}` — Get issue by ID
- `GET /api/issues/slug/{slug}` — Get issue by slug
- `POST /api/issues` — Create issue (Admin only)
- `PUT /api/issues/{id}` — Update issue (Admin only)
- `DELETE /api/issues/{id}` — Delete issue (Admin only)
- `POST /api/issues/seed` — Seed sample issues (Admin only)

### Events
- `GET /api/events` — Get all events
- `GET /api/events/{id}` — Get event by ID
- `GET /api/events/slug/{slug}` — Get event by slug
- `GET /api/events/upcoming` — Get upcoming events
- `GET /api/events/past` — Get past events
- `POST /api/events` — Create event (Admin only)
- `PUT /api/events/{id}` — Update event (Admin only)
- `DELETE /api/events/{id}` — Delete event (Admin only)
- `GET /api/events/{id}/rsvps` — Get event RSVPs (Admin only)
- `POST /api/events/seed` — Seed sample events (Admin only)

### Volunteers
- `GET /api/volunteers` — Get all volunteers
- `GET /api/volunteers/{id}` — Get volunteer by ID
- `POST /api/volunteers` — Create new volunteer
- `DELETE /api/volunteers/{id}` — Delete volunteer

### Contacts
- `GET /api/contacts` — Get all contact submissions
- `GET /api/contacts/{id}` — Get contact by ID
- `POST /api/contacts` — Create new contact submission
- `DELETE /api/contacts/{id}` — Delete contact

### Donations
- `GET /api/donations` — Get all donations
- `GET /api/donations/{id}` — Get donation by ID
- `GET /api/donations/stats` — Get donation statistics
- `POST /api/donations` — Create new donation
- `DELETE /api/donations/{id}` — Delete donation

### Event RSVPs
- `GET /api/eventrsvps?eventId={id}` — Get RSVPs (optionally filtered by event)
- `GET /api/eventrsvps/{id}` — Get RSVP by ID
- `GET /api/eventrsvps/count/{eventId}` — Get RSVP count and total attendees
- `POST /api/eventrsvps` — Create new RSVP
- `PUT /api/eventrsvps/{id}` — Update RSVP
- `DELETE /api/eventrsvps/{id}` — Delete RSVP

### Organizations (Admin only)
- `GET /api/units` — Get all volunteer units
- `POST /api/units` — Create volunteer unit
- `GET /api/teams` — Get all teams
- `POST /api/teams` — Create team
- `GET /api/assignments` — Get volunteer assignments
- `POST /api/assignments` — Create assignment

## Database Seeding

The project includes PowerShell scripts to seed sample data:

```powershell
# Seed Issues (requires admin JWT token)
.\backend\seed-issues.ps1 -Token "your-jwt-token"

# Seed Events (requires admin JWT token)
.\backend\seed-events.ps1 -Token "your-jwt-token"

# Clear Events before re-seeding
.\backend\clear-events.ps1 -Token "your-jwt-token"
```

**Getting Admin Token:**
1. Register a user via `/api/auth/register`
2. Manually promote user to Admin role in database
3. Login via `/api/auth/login` to get JWT token

## Development Notes

- **Authentication**: Uses JWT tokens with 7-day expiration. Tokens include role claims for authorization.
- **Slug-based Routing**: Events and Issues use SEO-friendly slug URLs (auto-generated from titles)
- **Admin Panel**: Located at `/admin` - requires Admin role for access
- **Data Seeding**: Use provided PowerShell scripts to populate Issues and Events tables
- **Circular References**: Controllers return DTOs to prevent JSON serialization errors
- **Donation Flow**: Currently a client-side placeholder. Replace with secure payment gateway integration (e.g., M-Pesa, Stripe) for production.
- **CORS**: Configured to allow localhost:5173 for development. Update in `Program.cs` for production.
- **Database**: SQLite file (`newkenya.db`) created in API project directory and excluded from git.
- **Production Database**: For production, consider migrating to PostgreSQL or SQL Server.

## Next Steps

- [x] Add authentication and authorization with JWT
- [x] Implement Issues (policy platform) system
- [x] Implement Events system with RSVP functionality
- [x] Create admin panel for content management
- [x] Add seed scripts for sample data
- [ ] Integrate payment gateway (M-Pesa for Kenyan market)
- [ ] Add admin UI for managing Events and Issues
- [ ] Implement email notifications for form submissions
- [ ] Add file upload for event/issue images
- [ ] Set up CI/CD pipeline
- [ ] Deploy frontend to Vercel/Netlify
- [ ] Deploy backend to Azure/AWS
- [ ] Add analytics tracking
- [ ] Implement A/B testing for messaging
- [ ] Add comprehensive unit and integration tests

## License

MIT
