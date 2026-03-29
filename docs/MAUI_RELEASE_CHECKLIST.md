# MAUI Member Release Checklist

Last updated: March 29, 2026

## Scope

This checklist is for Project HX member app release hardening under Sprint 6 (stories 6.2 and 6.3).

## Security Hardening

- Token handling
- Verify JWT is stored only via secure storage service.
- Confirm logout and account-delete paths clear token/session state.
- Confirm unauthenticated navigation cannot access member tabs.

- Upload handling
- Enforce image/document content-type checks before upload submission.
- Verify file picker restrictions on Android, iOS, and Windows.
- Verify evidence file staging path is app-private and not world-readable.

- API transport
- Verify production base URL is HTTPS.
- Verify auth endpoints and member workflows send bearer token where required.
- Verify non-2xx responses surface clear, non-sensitive user messages.

## Observability and Diagnostics

- Confirm diagnostics file is created in app data path under diagnostics/events.jsonl.
- Confirm outbox retry/dead-letter events are recorded with item type/id context.
- Confirm deep-link mapping failures are logged as warning diagnostics.
- Confirm startup/resume outbox failures are logged and persisted.

## Build Flavors and Config

- Android
- Validate package id and versionCode/versionName are correct.
- Validate release config uses signing keystore.
- Validate no debug-only API URLs remain in release settings.

- iOS
- Validate bundle id and build number/version are correct.
- Validate release provisioning profile and signing certificate.
- Validate ATS/network policy for production endpoints.

- Windows (test target)
- Validate net9.0-windows target builds and runs tests.

## Validation Gates

- Run MAUI unit test baseline and API smoke tests.
- Run mobile build with all configured target frameworks for the release branch.
- Perform manual smoke on key P0 workflows:
- Login and OTP path
- Profile load/update
- Tasks load/complete (online and queued offline)
- Inbox load/read
- Result submit and outbox sync recovery

## Sign-off Artifacts

- Build logs for Android and iOS release builds.
- Test run results attached to CI artifact.
- Security checklist review signed by mobile owner.
- Known-risk register updated with any deferred items.
