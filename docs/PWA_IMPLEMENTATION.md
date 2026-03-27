# PWA Implementation

Last updated: March 27, 2026

## Purpose

The web application is implemented as a Progressive Web App to support installability, offline fallback, push notification handling, and developer diagnostics.

## Current Assets

- Manifest: `public/manifest.json`
- Service worker: `public/service-worker.js`
- Offline page: `public/offline.html`
- Registration helper: `src/serviceWorkerRegistration.js`
- Install prompt UI: `src/components/InstallPrompt.jsx`
- Debug panel: `src/components/PWADebugPanel.jsx`

## Current Capabilities

### Installability
- The app can be installed where browser support allows it
- Install UX is surfaced through `InstallPrompt.jsx`
- The install prompt is integrated into the main app shell

### Offline behavior
- Service worker caches core resources and serves `offline.html` when navigation cannot complete online
- The offline experience is present but not a fully offline-first application model for all authenticated workflows

### Notification-aware service worker
- Handles push payloads
- Filters notifications using user category preferences
- Respects quiet hours stored in cached preference state
- Handles notification click routing and window focus behavior
- Processes subscription-change events and preference sync messages

### Analytics and diagnostics
- `PWADebugPanel.jsx` exposes developer-oriented diagnostics for service worker state, installability, storage, analytics, and notification test flows
- The debug panel is intended for development and troubleshooting rather than a user-facing production feature

## Frontend Integration

### App shell
The following app-shell components participate in the PWA experience:
- `InstallPrompt`
- `PWADebugPanel`
- `NotificationButton`
- `NotificationSettings`

### Related services
- `src/services/pwaAnalytics.js`
- `src/services/pushNotification.js`
- `src/services/notificationPreferences.js`

## Current Limitations

- Offline support does not make all authenticated task, admin, or command workflows fully available without a network connection
- The PWA includes useful diagnostics and notification tooling, but the broader app remains primarily network-driven
- Service worker caching and chunking strategy should continue to be reviewed as bundle size evolves
