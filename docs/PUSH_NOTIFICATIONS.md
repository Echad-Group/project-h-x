# Push Notifications

Last updated: March 27, 2026

## Purpose

Push notifications are used for browser-delivered campaign updates, including admin-triggered sends and category-aware dispatch from content publishing flows.

## Current Backend Implementation

### Controller
`PushController`

Endpoints:
- `POST /api/push/subscribe`
- `POST /api/push/unsubscribe`
- `POST /api/push/send`
- `GET /api/push/status`

### Persistence
Push subscriptions are stored in SQLite through the `PushSubscriptions` table.

Tracked fields include:
- endpoint
- P256dh key
- auth key
- active flag
- user association when available
- last-used timestamp

### Dispatch flow
- `PushNotificationDispatcher` sends category-tagged push payloads to active subscriptions
- Invalid subscriptions are marked inactive when downstream push providers return gone or not-found status codes
- News and Events publishing flows can trigger category-specific push dispatches

## Current Frontend Implementation

### Services and components
- `src/services/pushNotification.js`
- `src/services/notificationPreferences.js`
- `src/components/NotificationButton.jsx`
- `src/components/NotificationSettings.jsx`
- `src/components/NotificationTester.jsx`

### Current UX behavior
- Notification button is available in the app shell
- Settings page allows per-category toggles
- Quiet hours are configurable and synced to the service worker
- Test notifications can be triggered for diagnostics
- Logged-in and anonymous states are handled differently in the notification button behavior

## Service Worker Behavior

The service worker currently:
- receives push payloads
- reads cached notification preferences
- suppresses notifications during quiet hours
- suppresses categories that the user disabled
- routes users to the provided URL on click
- attempts to focus an existing client window before opening a new one

## Current Notification Categories

The implementation includes category filtering for:
- events
- news
- volunteer
- local

Additional category strings can still flow through payloads, but the current preference UI and worker logic are centered on those user-facing categories.

## Operational Notes

- VAPID keys must exist in backend configuration for actual dispatch
- Browser permission status determines whether subscription can succeed client-side
- Push subscription state is queryable from the backend
- Admin-originated send paths exist, but category dispatch from content publishing is the more integrated workflow currently present in the codebase

## Current Risks and Constraints

- Production secrets must not remain in plain configuration files
- Push is browser- and platform-dependent; parity across every mobile browser is not guaranteed
- Notification delivery analytics are present in limited form compared with a full notification observability platform
