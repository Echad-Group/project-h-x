# Project H X MAUI Native Skeleton

This folder contains a .NET MAUI Native scaffold aligned to the campaign field operations architecture.

## Included

- Shell navigation with mobile-first tabs
- Login + OTP challenge pages and viewmodels
- API auth service contracts and baseline implementation
- SecureStorage session service
- Offline outbox contracts and in-memory placeholder processor
- Placeholder pages for tasks, submission, inbox, leaderboard, profile

## Next Build Steps

1. Add real API DTOs for tasks, messages, and election submissions.
2. Add SQLite persistence for outbox and cached read models.
3. Implement push registration and deep-link routing.
4. Add geolocation and camera upload capabilities.
5. Connect SubmitResultPage to outbox-backed submit workflow.

## Notes

- Base API in MauiProgram is set to https://localhost:5065/api/
- Update platform-specific localhost mapping for Android emulator (10.0.2.2)
