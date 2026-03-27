# Project H X

Project H X is a full-stack campaign operations platform that combines a public-facing campaign website with authenticated volunteer tooling, an admin command portal, delivery and messaging workflows, election reporting, verification review, and a Mongo-backed war-room module.

Last updated: March 27, 2026

## Current Stack

### Frontend
- React 18 + Vite
- React Router 6
- Tailwind CSS
- i18next for English and Swahili localization
- PWA manifest + service worker

### Backend
- ASP.NET Core 8 Web API
- ASP.NET Identity + JWT authentication
- Entity Framework Core + SQLite for transactional application data
- MongoDB for War Room state persistence
- WebPush for browser push delivery
- Background hosted services for delivery, reminders, and weekly reporting

## Current Repository Layout

```text
project-h-x/
├── docs/
│   ├── API_TESTING.md
│   └── *.md
├── backend/
│   ├── seed-*.ps1
│   └── NewKenyaAPI/
│       ├── Controllers/
│       ├── Data/
│       ├── Migrations/
│       ├── Models/
│       ├── Services/
│       └── wwwroot/
├── public/
├── scripts/
├── src/
│   ├── components/
│   ├── contexts/
│   ├── locales/
│   ├── pages/
│   ├── sections/
│   ├── services/
│   └── utils/
├── mobile-app/
├── mobile-maui/
└── README.md
```

## Product Surface

### Public web experience
- Home, About, Issues, Events, News, Team, Gallery, FAQ, Contact, Get Involved
- Progressive Web App install prompt and offline fallback
- Push notification subscription and settings
- Donation modal and campaign merchandising placeholders

### Authenticated member experience
- Minimal signup with first name, last name, email, password, and confirm password
- Login with OTP challenge flow support
- Profile and settings management
- Profile completeness progress indicator with missing-items checklist
- Post-signup reminder banner for incomplete profiles
- Volunteer status check, volunteer editing, and leave flow
- Task execution hub and leaderboard access

### Admin experience
- Admin Panel with tabs for overview, war room, verification queue, hierarchy manager, coverage map, command center, message composer, election dashboard, campaign projects, teams and units, volunteer management, campaign team profile, news management, emails, and engagement analytics
- Protected by Admin role through frontend route guard and backend authorization policies

## Frontend Routes

### Public routes
- `/`
- `/about`
- `/issues`
- `/events`
- `/events/:id`
- `/get-involved`
- `/contact`
- `/faq`
- `/news`
- `/news/:slug`
- `/team`
- `/gallery`
- `/login`
- `/register`
- `/reset-password`
- `/leaderboard`
- `/notification-settings`

### Protected routes
- `/profile`
- `/tasks`
- `/volunteer/dashboard`
- `/organization`

### Admin-only routes
- `/admin`
- `/admin/news/create`
- `/admin/news/edit/:id`

## Backend Domains

### Identity and access
- `AuthController`
  - register, login, send-otp, verify, logout, reset-password, forgot-password
- JWT bearer authentication
- ASP.NET Identity roles seeded at startup

### Profile and verification
- `UserProfileController`
  - profile read/update
  - password change
  - email change
  - account deletion
  - profile photo upload/delete
  - verification document upload for NIDA, voter card, and selfie
- `VerificationController`
  - queue listing
  - queue item detail
  - approve/reject decisions
  - document retrieval

### Volunteer and organization operations
- `VolunteersController`
  - public volunteer creation
  - admin volunteer listing
  - volunteer status check
  - self-update and self-delete
- `UnitsController`, `TeamsController`, `AssignmentsController`
- `TasksController`

### Public content
- `NewsController`
- `EventsController`
- `EventRSVPsController`
- `IssuesController`
- `CampaignTeamController`
- `ContactsController`
- `DonationsController`

### Command and operations
- `CommandDashboardController`
- `ComplianceController`
- `MessagesController`
- `LeaderboardController`
- `GeolocationController`
- `ResultsController`
- `OperationsHealthController`
- `AuditController`
- `AdminEmailArchiveController`
- `WarRoomController`

## Persistence Model

### SQLite via EF Core
Primary application records live in SQLite, including:
- users and roles
- volunteers
- contacts
- donations
- events and RSVPs
- push subscriptions
- units, teams, assignments, tasks
- issues, initiatives, questions
- campaign team members
- news articles
- campaign messages
- compliance reminders
- leaderboard scores
- OTP verification codes
- election results
- geolocation pings
- audit events

### MongoDB
War Room state is persisted separately via `WarRoomMongoStore` and restored into `WarRoomCommandService` at startup. The persisted state covers command pods, incidents, battle rhythm, legal cases, red-zone decisions, coalition modules, mobilization roles, command grid, and campaign phases.

## Background Services

- `MessageDeliveryWorker`
  - processes queued in-app, push, and WhatsApp-style message deliveries
- `ComplianceReminderScheduler`
  - drives compliance reminder generation and escalation workflows
- `WeeklyCommandReportingService`
  - scheduled reporting support for operations summaries
- `CampaignBootstrapService`
  - seeds hierarchy and startup defaults

## PWA and Push

- Service worker in `public/service-worker.js`
- Offline fallback page in `public/offline.html`
- Install prompt component
- Notification settings with category preferences and quiet hours
- Push subscription status, subscribe, unsubscribe, and admin send/test flow
- Category-aware dispatch for News and Events publishing flows

## Signup, Profile, and Verification Flow

### Current signup flow
- Registration is intentionally minimal at point of entry
- Required fields: first name, last name, email, password, confirm password
- Optional identity fields remain supported at API level

### Current completion flow
- Profile page exposes deferred fields previously removed from signup
- Users can complete:
  - phone number
  - location
  - bio and socials
  - national ID number
  - voter card number
  - NIDA document upload
  - voter card upload
  - selfie upload
- Profile completeness indicator shows exact missing items

### Verification flow
- Admin verification queue surfaces pending users
- Documents are retrievable through verification endpoints
- Decision timeline is recorded by verification review service
- Face-match checks only run when the relevant document combination is provided

## Commands and Scripts

### Frontend
```powershell
npm install
npm run dev
npm run build
npm run preview
npm run test:unit
```

### Backend
```powershell
cd backend/NewKenyaAPI
dotnet restore
dotnet ef database update
dotnet run
dotnet build
```

### Seed scripts
```powershell
.\backend\seed-issues.ps1 -Token "<jwt>"
.\backend\seed-events.ps1 -Token "<jwt>"
.\backend\clear-events.ps1 -Token "<jwt>"
.\backend\seed-campaign-team.ps1 -Token "<jwt>"
.\backend\seed-news.ps1 -Token "<jwt>"
```

## Environment and Local Development Notes

- Frontend dev server runs on Vite, typically `http://localhost:5173`
- Backend API commonly runs on `http://localhost:5065` in local development
- CORS currently allows local Vite origins configured in `Program.cs`
- HTTP logging is enabled in development to log endpoint hits with request and response detail
- Default admin bootstrap still exists and should be locked down before production

## Known Current Constraints

- MongoDB driver version in the backend currently raises a vulnerability warning during build
- Frontend production bundle is large enough to trigger Vite chunk-size warnings
- Some folders, such as `mobile-app` and `mobile-maui`, are scaffolds rather than production-complete clients
- Payment processing remains a placeholder, not a live integrated gateway

## Documentation Map

- [docs/CAMPAIGN_TEAM_IMPLEMENTATION.md](docs/CAMPAIGN_TEAM_IMPLEMENTATION.md)
- [docs/EVENTS_IMPLEMENTATION.md](docs/EVENTS_IMPLEMENTATION.md)
- [docs/ISSUES_IMPLEMENTATION.md](docs/ISSUES_IMPLEMENTATION.md)
- [docs/NEWS_IMPLEMENTATION.md](docs/NEWS_IMPLEMENTATION.md)
- [docs/PWA_IMPLEMENTATION.md](docs/PWA_IMPLEMENTATION.md)
- [docs/PUSH_NOTIFICATIONS.md](docs/PUSH_NOTIFICATIONS.md)
- [docs/USER_PROFILE_IMPLEMENTATION.md](docs/USER_PROFILE_IMPLEMENTATION.md)
- [docs/VOLUNTEER_ACCOUNT_IMPLEMENTATION.md](docs/VOLUNTEER_ACCOUNT_IMPLEMENTATION.md)
- [docs/VOLUNTEER_ORGANIZATION_SYSTEM.md](docs/VOLUNTEER_ORGANIZATION_SYSTEM.md)
- [docs/PRODUCTION_CHECKLIST.md](docs/PRODUCTION_CHECKLIST.md)
- [docs/API_TESTING.md](docs/API_TESTING.md)
- [docs/CHAT_EXPORT_GAP_MATRIX.md](docs/CHAT_EXPORT_GAP_MATRIX.md)
- [docs/SRS_CONSOLIDATED.md](docs/SRS_CONSOLIDATED.md)

## Current Testing State

- Frontend build succeeds
- Backend build succeeds when the API executable is not locked by a running process
- Lightweight frontend unit test runner exists for `seoHelpers`
- There is no comprehensive automated test suite covering the full platform yet
