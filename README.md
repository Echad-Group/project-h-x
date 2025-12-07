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
- ✅ CORS configured for frontend
- ✅ Swagger/OpenAPI documentation
- ✅ Data validation and error handling
- ✅ Four main entities:
  - Volunteers (name, email, phone, interests)
  - Contacts (name, email, subject, message)
  - Donations (amount, donor info, payment method)
  - EventRSVPs (event ID, attendee info, guest count)

## API Endpoints

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
- `GET /api/eventrsvps/count/{eventId}` — Get RSVP count for event
- `POST /api/eventrsvps` — Create new RSVP
- `PUT /api/eventrsvps/{id}` — Update RSVP
- `DELETE /api/eventrsvps/{id}` — Delete RSVP

## Development Notes

- The donation flow is currently a client-side placeholder. Replace with secure payment gateway integration (e.g., M-Pesa, Stripe) for production.
- API is configured to allow CORS from localhost:5173 for development. Update CORS settings in `Program.cs` for production deployment.
- Database file (`newkenya.db`) is created in the API project directory and excluded from git.
- The backend uses SQLite for simplicity. For production, consider migrating to PostgreSQL or SQL Server.

## Next Steps

- [ ] Integrate payment gateway (M-Pesa for Kenyan market)
- [ ] Add authentication and authorization (admin panel)
- [ ] Implement email notifications for form submissions
- [ ] Add file upload for profile images
- [ ] Set up CI/CD pipeline
- [ ] Deploy frontend to Vercel/Netlify
- [ ] Deploy backend to Azure/AWS
- [ ] Add analytics tracking
- [ ] Implement A/B testing for messaging
- [ ] Add comprehensive unit and integration tests

## License

MIT
