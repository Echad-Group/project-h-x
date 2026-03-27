# API Testing Guide

Last updated: March 27, 2026

## Base URL

Local development commonly uses:
- `http://localhost:5065/api`

## Authentication Setup

### Register
```http
POST /api/auth/register
Content-Type: multipart/form-data
```

Required form fields:
- `firstName`
- `lastName`
- `email`
- `password`

Optional form fields still accepted by the backend include:
- `phoneNumber`
- `campaignRole`
- `nationalIdNumber`
- `voterCardNumber`
- `nidaDocument`
- `voterCardDocument`
- `selfieDocument`

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123"
}
```

### OTP
```http
POST /api/auth/send-otp
POST /api/auth/verify
```

### Password reset
```http
POST /api/auth/forgot-password
POST /api/auth/reset-password
```

## Public Content Endpoints

### News
- `GET /api/news`
- `GET /api/news/featured`
- `GET /api/news/{slug}`
- `GET /api/news/by-id/{id}`
- `POST /api/news/{id}/increment-view`
- `GET /api/news/categories`
- `GET /api/news/tags`

### Events
- `GET /api/events`
- `GET /api/events/{id}`
- `GET /api/events/slug/{slug}`
- `GET /api/events/upcoming`
- `GET /api/events/past`

### Event RSVPs
- `GET /api/eventrsvps/count/{eventId}`
- `POST /api/eventrsvps`

### Issues
- `GET /api/issues`
- `GET /api/issues/{id}`
- `GET /api/issues/slug/{slug}`

### Campaign team
- `GET /api/campaignteam`
- `GET /api/campaignteam/{id}`

### Contact and donation
- `POST /api/contacts`
- `POST /api/donations`

## Authenticated Member Endpoints

### Profile
- `GET /api/userprofile`
- `PUT /api/userprofile`
- `POST /api/userprofile/upload-photo`
- `DELETE /api/userprofile/photo`
- `POST /api/userprofile/upload-verification-document`
- `PUT /api/userprofile/password`
- `PUT /api/userprofile/email`
- `DELETE /api/userprofile`

### Volunteer self-service
- `GET /api/volunteers/check-status`
- `PUT /api/volunteers/me`
- `DELETE /api/volunteers/me`

### Tasks, inbox, leaderboard
- `GET /api/tasks/my-tasks`
- `GET /api/messages/inbox`
- `POST /api/messages/{messageId}/read`
- `GET /api/leaderboard/my-rank`

## Admin and Operations Endpoints

### Volunteers and organization
- `GET /api/volunteers/admin`
- `GET /api/units`
- `GET /api/teams`
- `POST /api/assignments`
- `POST /api/assignments/bulk`
- `GET /api/tasks/manage`
- `POST /api/tasks/create`
- `POST /api/tasks/assign`

### Content management
- `POST /api/news`
- `PUT /api/news/{id}`
- `DELETE /api/news/{id}`
- `POST /api/events`
- `PUT /api/events/{id}`
- `DELETE /api/events/{id}`
- `POST /api/issues`
- `PUT /api/issues/{id}`
- `DELETE /api/issues/{id}`
- `POST /api/campaignteam`
- `PUT /api/campaignteam/{id}`
- `DELETE /api/campaignteam/{id}`

### Verification and command
- `GET /api/verification/queue`
- `GET /api/verification/queue/{userId}`
- `POST /api/verification/queue/{userId}/decision`
- `GET /api/command-dashboard/summary`
- `GET /api/compliance/summary`
- `POST /api/compliance/reminder`
- `GET /api/messages/analytics`
- `POST /api/messages/broadcast`
- `POST /api/messages/target`
- `POST /api/leaderboard/recalculate`
- `GET /api/geolocation/coverage`
- `POST /api/geolocation/ingest`
- `GET /api/ops-health`
- `GET /api/audit/events`
- `GET /api/adminemailarchive`

### Election results
- `POST /api/results/submit`
- `GET /api/results/pending-review`
- `POST /api/results/{resultId}/review`
- `GET /api/results/conflicts`
- `POST /api/results/conflicts/{conflictGroupKey}/adjudicate`
- `GET /api/results/aggregate`
- `GET /api/results/status`

### War Room
- `GET /api/warroom/state`
- `GET /api/warroom/pods`
- `POST /api/warroom/pods`
- `PUT /api/warroom/pods/{podId}`
- `POST /api/warroom/incidents`
- `GET /api/warroom/incidents`
- `PUT /api/warroom/incidents/{incidentId}`
- `POST /api/warroom/incidents/{incidentId}/escalate`
- `GET /api/warroom/battle-rhythm`
- `POST /api/warroom/battle-rhythm/{itemId}/complete`
- `GET /api/warroom/red-zone`
- `GET /api/warroom/red-zone/decisions`
- `POST /api/warroom/red-zone/toggle`
- `POST /api/warroom/red-zone/decisions`
- `GET /api/warroom/command-grid`
- `PUT /api/warroom/command-grid/{nodeId}`
- `GET /api/warroom/coalitions`
- `POST /api/warroom/coalitions`
- `PUT /api/warroom/coalitions/{coalitionId}/modules`
- `GET /api/warroom/mobilization-roles`
- `PUT /api/warroom/mobilization-roles/{roleCode}`
- `GET /api/warroom/campaign-phases`
- `POST /api/warroom/campaign-phases/switch`
- `GET /api/warroom/legal-cases`
- `GET /api/warroom/legal-cases/query`
- `POST /api/warroom/legal-cases`
- `PUT /api/warroom/legal-cases/{caseId}`

## Suggested Smoke Test Sequence

1. Register a user
2. Login and capture JWT
3. Load current profile
4. Update profile and upload profile photo
5. Create volunteer record or check volunteer status
6. Subscribe to push notifications
7. Fetch public news, events, and issues
8. As admin, create a news article or event
9. Review verification queue and command dashboard
10. Exercise one War Room endpoint group and one results endpoint group

## Notes

- Many admin and command endpoints require Admin or leadership roles
- Multipart endpoints should be tested with an actual client that supports file upload semantics
- A running backend process can lock build artifacts, but it does not affect HTTP endpoint testing itself
