# MAUI Member Base Backlog

Last updated: March 27, 2026

## Purpose

This document defines a sprint-ready baseline backlog for the MAUI mobile app so members can do most non-admin workflows already available on the web platform.

## Product Goal

Deliver a production-usable MAUI app for party members and volunteers that supports:
- account access and security
- profile and volunteer tracking
- task execution and reporting
- inbox communication
- leaderboard visibility
- public campaign content consumption

## In Scope (Mobile Member App)

- Authentication and session lifecycle
- Member profile and deferred verification uploads
- Volunteer self-service status and updates
- Member task list and task completion flow
- Election result submission for field use
- Member inbox and read acknowledgments
- Leaderboard and member rank
- Public content: news, events, issues, campaign team
- Offline queue, sync, and resilience for field conditions
- Push notifications and deep links

## Out of Scope (Admin or Command Surfaces)

- Admin panel and admin content CRUD screens
- Verification queue review and adjudication
- Command dashboard and compliance admin controls
- War Room command operations
- Operations health and audit event admin tools

## Current State Snapshot

Implemented in MAUI:
- Login and OTP verification flow
- JWT token storage in secure storage
- Tab shell structure for main surfaces
- Placeholder outbox contracts

Not implemented in MAUI:
- Feature pages under tabs are scaffold placeholders
- No authenticated API client pipeline for all member endpoints
- No durable offline storage or retry engine
- No push/deep-link/device capability integration

## Backlog Structure

Priority levels:
- P0: required for usable member baseline
- P1: required for field reliability and parity
- P2: quality and optimization after core delivery

Effort labels:
- S: 1-2 days
- M: 3-5 days
- L: 1-2 weeks

## Epic 1: App Foundation and Auth Hardening (P0)

### Story 1.1: Environment-aware API configuration (S)
- Replace hardcoded base URL with environment configuration per platform
- Support local emulator and production endpoints

Acceptance criteria:
- Android emulator, iOS simulator, and device builds resolve API base URL without code edits
- No hardcoded ngrok URL in startup code

### Story 1.2: Authenticated HTTP pipeline (M)
- Add token-attaching HTTP handler
- Add standardized error mapping and transient retry policy

Acceptance criteria:
- Authenticated endpoints receive bearer token automatically
- Expired/invalid token causes controlled sign-out and redirect to login

### Story 1.3: Session restore and guarded navigation (M)
- Restore existing session on app launch
- Gate member tabs behind authenticated state

Acceptance criteria:
- App opens to login if no valid token
- App opens to main shell if valid token exists

### Story 1.4: Account lifecycle actions (M)
- Add register, forgot-password, reset-password, logout

Acceptance criteria:
- Member can create account, recover password, and logout from mobile
- Validation and API error messages are visible and actionable

## Epic 2: Member Tracking Core (P0)

### Story 2.1: Profile read/update screen (L)
- Implement profile API integration for details and edits
- Include profile photo upload/delete

Acceptance criteria:
- Profile loads from API and saves successfully
- Profile photo update and delete work on device

### Story 2.2: Completeness and deferred verification (L)
- Implement completeness percentage and missing-items checklist
- Upload NIDA, voter card, and selfie documents

Acceptance criteria:
- Checklist reflects backend profile data
- Verification document uploads persist and refresh status

### Story 2.3: Volunteer self-service status (M)
- Show volunteer record status
- Support update and leave actions

Acceptance criteria:
- Volunteer status loads from API
- Member can update own volunteer details and leave via mobile

## Epic 3: Member Operations Workflows (P0)

### Story 3.1: My tasks list and detail (L)
- Consume my-task endpoints
- Show assignee context, due dates, and status

Acceptance criteria:
- Member sees assigned tasks and task details
- Empty/error states are handled clearly

### Story 3.2: Task execution and completion (L)
- Add task action flow and completion submission
- Queue completion actions if offline

Acceptance criteria:
- Online completion updates backend state
- Offline completion is queued and syncs when connectivity returns

### Story 3.3: Result submission workflow (L)
- Build polling-station result form
- Integrate location and optional evidence capture
- Submit directly or via outbox if offline

Acceptance criteria:
- Valid result submissions reach backend
- Offline submissions are retained and eventually synced

### Story 3.4: Inbox and read acknowledgment (M)
- Build inbox list and message detail/read action

Acceptance criteria:
- Member can load inbox messages
- Read acknowledgment updates message state server-side

### Story 3.5: Leaderboard member view (M)
- Implement my-rank and surrounding context view

Acceptance criteria:
- Member rank is visible and refreshable
- Screen handles no-rank/no-data gracefully

## Epic 4: Public Content Parity (P1)

### Story 4.1: News feed and article detail (M)
- News listing, featured, and detail

Acceptance criteria:
- Member can browse current published news and open article detail

### Story 4.2: Events and RSVP (M)
- Event list/detail with RSVP submission

Acceptance criteria:
- Member can RSVP from app and see RSVP success state

### Story 4.3: Issues and campaign team (M)
- Issues listing/detail and campaign team view

Acceptance criteria:
- Member can browse policy issues and team profiles from mobile

## Epic 5: Offline, Sync, and Device Capabilities (P1)

### Story 5.1: Durable local storage (L)
- Replace in-memory queue with SQLite-backed outbox
- Add queue schema, statuses, retry metadata

Acceptance criteria:
- App restart does not lose pending outbox items
- Failed items are retried with backoff

### Story 5.2: Connectivity-aware sync engine (L)
- Trigger sync on reconnect and app resume
- Add conflict and dead-letter handling

Acceptance criteria:
- Pending outbox drains automatically when network returns
- Non-recoverable failures are visible in a sync status view

### Story 5.3: Push notifications and deep links (L)
- Register device and update push preference state
- Handle notification taps and deep links into app routes

Acceptance criteria:
- Push token registration succeeds
- Notification tap opens expected screen context

### Story 5.4: Geolocation and camera integration (M)
- Capture geolocation for required workflows
- Capture/select images for evidence uploads

Acceptance criteria:
- Permission flow is handled per platform
- Captured data is attached correctly to submission payloads

## Epic 6: Reliability, Security, and Release Readiness (P2)

### Story 6.1: Test baseline (L)
- ViewModel unit tests for auth/profile/tasks/inbox/outbox
- API integration smoke tests for critical member paths

Acceptance criteria:
- CI test suite covers P0 workflows and passes reliably

### Story 6.2: Observability and failure analytics (M)
- Add structured logging, crash reporting, sync diagnostics

Acceptance criteria:
- Critical runtime errors and sync failures are traceable

### Story 6.3: Hardening and release checklist (M)
- Secure upload handling, token handling review, build flavor checks
- Android and iOS release checklist completion

Acceptance criteria:
- Signed builds generated and validated for both platforms
- Security checklist items completed for member baseline

## Sprint Sequence (Recommended)

### Sprint 1
- Epic 1 stories 1.1, 1.2, 1.3
- Epic 2 story 2.1 start

Outcome:
- Stable app shell and session pipeline

### Sprint 2
- Epic 1 story 1.4
- Epic 2 stories 2.1, 2.2, 2.3

Outcome:
- Full profile and volunteer self-service baseline

### Sprint 3
- Epic 3 stories 3.1, 3.2, 3.4, 3.5

Outcome:
- Core member daily workflows operational

### Sprint 4
- Epic 3 story 3.3
- Epic 5 stories 5.1, 5.2

Outcome:
- Field submission and offline-first reliability baseline

### Sprint 5
- Epic 4 stories 4.1, 4.2, 4.3
- Epic 5 stories 5.3, 5.4

Outcome:
- Public-content parity and native capability integration

### Sprint 6
- Epic 6 stories 6.1, 6.2, 6.3

Outcome:
- Production readiness and quality hardening

## Endpoint Mapping for Mobile Member Scope

Authentication:
- POST /api/auth/register
- POST /api/auth/login
- POST /api/auth/send-otp
- POST /api/auth/verify
- POST /api/auth/forgot-password
- POST /api/auth/reset-password
- POST /api/auth/logout

Profile and volunteer:
- GET /api/userprofile
- PUT /api/userprofile
- POST /api/userprofile/upload-photo
- DELETE /api/userprofile/photo
- POST /api/userprofile/upload-verification-document
- PUT /api/userprofile/password
- PUT /api/userprofile/email
- DELETE /api/userprofile
- GET /api/volunteers/check-status
- PUT /api/volunteers/me
- DELETE /api/volunteers/me

Operations:
- GET /api/tasks/my-tasks
- GET /api/messages/inbox
- POST /api/messages/{messageId}/read
- GET /api/leaderboard/my-rank
- POST /api/results/submit

Public content:
- GET /api/news
- GET /api/news/featured
- GET /api/news/{slug}
- GET /api/events
- GET /api/events/{id}
- GET /api/events/slug/{slug}
- POST /api/eventrsvps
- GET /api/issues
- GET /api/issues/{id}
- GET /api/issues/slug/{slug}
- GET /api/campaignteam

## Definition of Done for Member Baseline

The MAUI member baseline is complete when:
- all P0 stories are delivered and tested on Android and iOS
- offline queue survives app restarts and syncs reliably after reconnect
- member can authenticate, manage profile, manage volunteer status, execute tasks, read inbox, view leaderboard, and submit results
- no admin-only workflows are exposed in the mobile navigation
- release checklist is complete for a pilot deployment
