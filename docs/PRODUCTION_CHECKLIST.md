# Production Checklist

Last updated: March 27, 2026

## Current Readiness Summary

The codebase contains a large amount of functional application logic, but it should still be treated as pre-production until infrastructure, secrets handling, delivery observability, and automated testing are hardened.

## Backend Hardening

### Secrets and configuration
- Move JWT secret, default admin credentials, SMTP credentials, and VAPID private key out of checked configuration files and into environment-based secret storage
- Review all appsettings variants for local-only values
- Ensure MongoDB connection details are environment-specific

### Database and persistence
- Decide whether SQLite remains acceptable for the intended deployment profile
- Back up both SQLite data and MongoDB War Room state
- Validate migration history on a clean environment before release

### Logging and observability
- Keep HTTP logging disabled or reduced for sensitive production paths if request-body logging is not acceptable
- Route structured logs to a central sink
- Add monitoring for background services, especially delivery workers and reminder schedulers
- Monitor war-room persistence success and restore health

### API protection
- Review role assignments and seeded default admin behavior
- Confirm rate limiting rules are appropriate for public and admin endpoints
- Review authorization on push, verification, election, and command endpoints

## Frontend and PWA Readiness

- Verify manifest metadata, icons, and production URLs
- Confirm service worker caching strategy after each release
- Validate notification permission and push flows on supported target browsers
- Review bundle-size warnings and introduce more code splitting if needed

## Feature-Specific Readiness

### Authentication and profile
- Verify signup, login, OTP, forgot-password, and reset-password flows end to end
- Verify profile completeness, deferred identity capture, and verification uploads end to end

### Verification
- Test admin queue review flow and document retrieval
- Confirm upload storage, document replacement, and reviewer decisions behave correctly

### Messaging and push
- Test push subscribe, unsubscribe, status, admin send, and content-triggered category dispatch
- Validate message worker behavior for in-app, push, and WhatsApp-style delivery channels

### Events and news
- Test content creation, update, public display, and notification side effects

### Volunteer and organization
- Test volunteer create, self-edit, leave, team assignment, task assignment, and volunteer dashboard flows

### Election and command modules
- Test result submission, review, conflict adjudication, aggregation, and status reporting
- Test command dashboard, compliance, leaderboard, geolocation, and war-room APIs with realistic data

## Operational Actions Before Release

- Configure email provider credentials and validate actual delivery
- Review uploaded-file storage strategy for profile photos and verification documents
- Decide on reverse proxy, TLS termination, backup policy, and deployment rollback procedure
- Create a clean staging environment that mirrors production topology as closely as possible

## Known Current Warnings

- Backend builds currently warn about the MongoDB.Driver version in use
- Frontend builds currently warn about large bundle chunks and a mixed dynamic/static import path in push-notification code

## Recommended Release Gate

Do not treat the platform as production-ready until all of the following are true:
- secrets are externalized
- default admin bootstrap is locked down
- full smoke tests pass on staging
- backup and restore procedure is verified
- email and push are tested with production-like configuration
- role and authorization review is complete
