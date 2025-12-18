# Progressive Web App (PWA) Implementation

## Overview

The New Kenya Platform is a fully functional Progressive Web App with comprehensive offline capabilities, push notifications, installability, and advanced debugging tools. The PWA implementation follows modern best practices and provides a native app-like experience.

## Core PWA Features

### 1. **Service Worker**
- Location: `public/service-worker.js`
- Registration: `src/serviceWorkerRegistration.js`
- Features:
  - Offline page support
  - Cache-first strategy for static assets
  - Network-first for API calls
  - Push notification handling
  - Background sync capabilities

### 2. **Web App Manifest**
- Location: `public/manifest.json`
- Configuration:
  - App name: "New Kenya"
  - Theme color: `#10b981` (green)
  - Background color: `#ffffff`
  - Display mode: `standalone`
  - Icons: Multiple sizes (192x192, 512x512)
  - Scope and start URL configured

### 3. **Installability**
- **Install Prompt Component** (`src/components/InstallPrompt.jsx`)
  - Captures `beforeinstallprompt` event
  - Beautiful modal UI for install prompts
  - Responsive design for mobile and desktop
  - Analytics tracking for install events
  - Dismissible with "Don't show again" option

### 4. **Offline Support**
- Offline page: `public/offline.html`
- Graceful degradation when network unavailable
- Cached resources accessible offline
- User-friendly offline messaging

## PWA Debug Panel

### Location
`src/components/PWADebugPanel.jsx`

### Purpose
Comprehensive debugging and monitoring tool for PWA development and testing.

### Features

#### **Tab 1: Status**
Displays real-time PWA and system status:

**PWA Features Section**
- ✅ Standalone Mode - Whether app is running in standalone mode
- ✅ Service Worker - Registration status with state badge (activated, installing, etc.)
- ✅ Push Notifications - Browser support for Push API
- ✅ Push Subscription - Active subscription status
- ✅ Installable - Whether install prompt is available

**Network Information Section**
- 🟢 Online/Offline Status - Live indicator with pulsing animation
- 📶 Connection Type - Effective connection type (4g, 3g, 2g, wifi)
- ⚡ Downlink Speed - Connection speed in Mbps
- Real-time updates on network changes

**Battery Information Section** (when available)
- 🔋 Battery Level - Percentage with visual progress bar
- ⚡ Charging Status - Whether device is charging
- Color-coded indicator (green >20%, red ≤20%)
- Live updates on battery changes

**System Information Section**
- Last Service Worker Update timestamp
- User Agent string (truncated for display)

**Actions**
- **Install App** button (when installable)
- **Update Service Worker** - Force SW update
- **Refresh All Data** - Reload all debug information

#### **Tab 2: Storage**
Comprehensive storage and cache management:

**Storage Usage Section**
- Used storage in KB/MB/GB
- Total quota available
- Visual progress bar with percentage
- Color-coded indicator (purple gradient)

**Cache Storage Section**
- Total number of cached items
- Number of active caches
- List of all cache names with details
- Cache contents breakdown

**App Manifest Section**
- App name and short name
- Theme color with visual swatch
- Background color
- All manifest properties

**Storage Actions**
- 🗑️ **Clear All Caches** - Remove all cached data
- ⚠️ **Unregister Service Worker** - Complete SW removal

#### **Tab 3: Analytics**
PWA usage analytics and insights:

- **Analytics Visualizations** component integration
- Session tracking (start time, duration, standalone mode)
- Event tracking (installs, notifications, offline usage)
- Installation date and history
- Notification statistics (received, clicked, by category)
- Offline usage metrics
- **Reset Analytics** button

#### **Tab 4: Notifications**
Push notification testing and management:

**Notification Permission**
- Current permission status
- Request permission button
- Real-time status updates

**Test Notification**
- Send test notification with custom content
- Test action buttons
- Verify notification delivery

**Notification Categories**
- Enable/disable categories with toggle switches
- Category descriptions
- Export preferences to JSON
- Import preferences from file

**Quiet Hours**
- Enable/disable quiet hours
- Start time picker (default: 10 PM)
- End time picker (default: 8 AM)
- Visual toggle switches

### UI Design

**Modern Gradient Interface**
- Header: Blue-to-purple gradient
- Color-coded sections:
  - Blue/Purple: PWA features
  - Green/Teal: Network information
  - Yellow/Orange: Battery status
  - Gray: System information
  - Purple/Pink: Storage usage

**Interactive Elements**
- SVG icons throughout (no emoji dependencies)
- Smooth transitions and animations
- Hover effects on buttons
- Loading states (spinning refresh icon)
- Pulsing indicators for live data
- Progress bars for percentages

**Responsive Design**
- Fixed bottom-right positioning
- 420px width panel
- 85vh max height with scrolling
- Mobile-friendly touch targets
- Compact information display

### Technical Implementation

**State Management**
```javascript
const [debug, setDebug] = useState({
  isInstallable: false,
  isStandalone: false,
  serviceWorker: null,
  serviceWorkerState: 'unknown',
  pushSupport: false,
  subscription: null,
  cacheSize: 0,
  cacheNames: [],
  manifest: null,
  networkType: 'unknown',
  downlink: 'unknown',
  isOnline: true,
  lastUpdate: null,
  installPromptEvent: null,
  storageEstimate: null,
  battery: null
});
```

**Event Listeners**
- `online` / `offline` - Network status changes
- `connection.change` - Connection type changes
- `controllerchange` - Service worker updates
- `beforeinstallprompt` - Install prompt availability
- `battery.levelchange` - Battery level changes
- `battery.chargingchange` - Charging status changes

**APIs Used**
- Service Worker API
- Cache Storage API
- Push API
- Battery Status API
- Network Information API
- Storage API (quota estimation)
- Web App Manifest API

**Helper Functions**
```javascript
formatBytes(bytes)        // Convert bytes to KB/MB/GB
getNetworkIcon(type)      // Get emoji for connection type
StatusRow({ label, value, badge })  // Consistent status display
```

### Development vs Production

**Development Mode**
- Debug panel fully visible and functional
- All debugging features enabled
- Performance monitoring active

**Production Mode**
- Debug panel completely hidden (`process.env.NODE_ENV === 'production'`)
- No performance overhead
- Analytics still collected

### Usage

**Opening the Debug Panel**
1. Look for the settings icon (⚙️) in bottom-right corner
2. Click to open the panel
3. Navigate between tabs using the tab bar

**Common Debug Tasks**

*Check Service Worker Status:*
- Open Status tab
- Look at "Service Worker" row
- Badge shows current state (activated, installing, etc.)

*Clear Cache:*
- Open Storage tab
- Click "Clear All Caches" button
- Confirm action

*Test Notifications:*
- Open Notifications tab
- Ensure permission is granted
- Click "Send Test" button

*Check Storage Usage:*
- Open Storage tab
- View usage percentage and details
- Review active cache names

*Force Service Worker Update:*
- Open Status tab
- Click "Update Service Worker" button

*Install PWA:*
- Open Status tab
- If "Install App" button appears, click it
- Follow browser prompts

### Integration with Other Services

**PWA Analytics Service**
- Location: `src/services/pwaAnalytics.js`
- Tracks installations, sessions, events
- LocalStorage-based persistence
- Integration with Google Analytics (optional)

**Notification Preferences Service**
- Location: `src/services/notificationPreferences.js`
- Category management
- Quiet hours configuration
- Import/export functionality

**Push Notification Service**
- Location: `src/services/pushNotification.js`
- Subscription management
- Test notification sending
- VAPID key configuration

## Service Worker Features

### Cache Strategies

**Static Assets (Cache-First)**
- HTML pages
- CSS stylesheets
- JavaScript bundles
- Images and fonts
- Icons

**API Calls (Network-First with Fallback)**
- User data endpoints
- Events and news
- Campaign team information
- Volunteers and organizations

**Offline Fallback**
- Displays `offline.html` when network unavailable
- Shows friendly message to users
- Preserves app context

### Push Event Handling

```javascript
self.addEventListener('push', (event) => {
  const data = event.data.json();
  const options = {
    body: data.message,
    icon: '/icon-192.png',
    badge: '/icon-192.png',
    data: data.url,
    actions: data.actions
  };
  event.waitUntil(
    self.registration.showNotification(data.title, options)
  );
});
```

### Notification Click Handling

```javascript
self.addEventListener('notificationclick', (event) => {
  event.notification.close();
  
  // Focus existing window or open new one
  event.waitUntil(
    clients.matchAll({ type: 'window', includeUncontrolled: true })
      .then(windowClients => {
        // Smart window management logic
      })
  );
});
```

## Installation Process

### User Flow

1. **User visits website**
   - Service worker registers automatically
   - Assets begin caching

2. **Install prompt appears** (after criteria met)
   - Beautiful modal with app icon
   - "Install" and "Maybe Later" options
   - Analytics tracking

3. **User clicks "Install"**
   - Browser shows native install prompt
   - App installs to device/desktop
   - Icon added to home screen/app drawer

4. **App launches in standalone mode**
   - Full-screen experience
   - No browser UI
   - Native app feel

### Install Criteria

- HTTPS served (or localhost for development)
- Valid manifest.json with required fields
- Registered service worker
- User engagement (visitedAt least once before)

## Analytics Tracking

### Tracked Events

**Installation Events**
- `install` - `prompt_ready` - Install prompt shown
- `install` - `manual_trigger` - User clicked install
- `install` - `success` - Installation completed

**Session Events**
- Session start/end times
- Standalone vs browser mode
- Total session duration

**Notification Events**
- `notification` - `grant` - Permission granted
- `notification` - `subscribe` - User subscribed
- `notification` - `received` - Notification received
- `notification` - `clicked` - Notification clicked

**Offline Events**
- `offline` - `usage` - App used while offline

### Data Structure

```javascript
{
  installDate: "2024-01-15T10:30:00Z",
  sessions: [
    {
      startTime: "2024-01-15T10:30:00Z",
      endTime: "2024-01-15T10:45:00Z",
      events: [...],
      isStandalone: true
    }
  ],
  events: [...],
  notifications: {
    granted: true,
    subscribed: true,
    received: 25,
    clicked: 18,
    categories: {
      events: 10,
      news: 8,
      volunteer: 7
    }
  },
  offline: {
    usage: 5,
    lastUsed: "2024-01-15T10:35:00Z"
  }
}
```

## Browser Support

### Fully Supported
- ✅ Chrome/Edge (Desktop & Mobile)
- ✅ Firefox (Desktop & Mobile)
- ✅ Safari (iOS 11.3+, macOS)
- ✅ Samsung Internet
- ✅ Opera

### Partial Support
- ⚠️ Safari (limited push notification support on macOS)
- ⚠️ iOS Safari (install requires "Add to Home Screen" manual action)

### Not Supported
- ❌ Internet Explorer
- ❌ Legacy browsers (< 2 years old)

## Testing Checklist

### Development Testing

- [ ] Service worker registers successfully
- [ ] Offline page displays when network disabled
- [ ] Install prompt appears after criteria met
- [ ] App installs correctly on device
- [ ] Push notifications work (permission, subscribe, receive)
- [ ] Debug panel opens and displays correct data
- [ ] All debug panel tabs function correctly
- [ ] Cache management works (clear, update)
- [ ] Storage quota displays accurately
- [ ] Battery API shows correct data (if available)
- [ ] Network information updates in real-time

### Cross-Browser Testing

- [ ] Test in Chrome (desktop)
- [ ] Test in Firefox (desktop)
- [ ] Test in Safari (macOS)
- [ ] Test in Chrome (Android)
- [ ] Test in Safari (iOS)
- [ ] Test in Edge (desktop)
- [ ] Test in Samsung Internet (Android)

### Offline Testing

- [ ] Disable network and verify offline page
- [ ] Check cached pages load correctly
- [ ] Verify API fallback behavior
- [ ] Test offline analytics tracking
- [ ] Confirm service worker updates when online

### Performance Testing

- [ ] Lighthouse PWA score (target: 90+)
- [ ] Service worker install time
- [ ] Cache size optimization
- [ ] Time to interactive (TTI)
- [ ] First contentful paint (FCP)

## Production Deployment

### Pre-Deployment Checklist

1. **Manifest Configuration**
   - [ ] Update `name` and `short_name`
   - [ ] Set correct `start_url`
   - [ ] Configure `theme_color` and `background_color`
   - [ ] Add all required icon sizes
   - [ ] Set correct `scope`

2. **Service Worker**
   - [ ] Update cache version numbers
   - [ ] Configure cache strategies
   - [ ] Set correct API URLs
   - [ ] Test offline functionality

3. **HTTPS**
   - [ ] Ensure HTTPS enabled on production domain
   - [ ] Valid SSL certificate installed
   - [ ] HTTP redirects to HTTPS

4. **Push Notifications**
   - [ ] VAPID keys generated and configured
   - [ ] Backend push endpoints secured
   - [ ] Rate limiting enabled
   - [ ] Error handling implemented

5. **Debug Panel**
   - [ ] Verify hidden in production build
   - [ ] Test NODE_ENV=production setting
   - [ ] Confirm no console errors

### Post-Deployment Verification

1. Visit site and open DevTools
2. Check Application tab → Service Worker
3. Verify manifest loads correctly
4. Test install prompt appears
5. Install app and test standalone mode
6. Send test push notification
7. Verify offline functionality
8. Check Lighthouse PWA audit score

## Troubleshooting

### Service Worker Not Registering

**Symptoms:** SW doesn't appear in DevTools
**Solutions:**
- Verify HTTPS (or localhost)
- Check console for registration errors
- Clear browser cache and reload
- Check service worker file path

### Install Prompt Not Appearing

**Symptoms:** No install button/modal shown
**Solutions:**
- Verify manifest.json is valid
- Check service worker is registered
- Ensure HTTPS enabled
- Meet install criteria (multiple visits)
- Check console for manifest errors

### Push Notifications Not Working

**Symptoms:** Notifications don't appear
**Solutions:**
- Check browser notification permissions
- Verify VAPID keys configured
- Test with Debug Panel → Notifications tab
- Check service worker push event handler
- Verify backend push endpoint

### Cache Not Updating

**Symptoms:** Old content still showing
**Solutions:**
- Increment cache version in service worker
- Use Debug Panel to clear caches
- Force service worker update
- Check cache strategy configuration

### Offline Page Not Showing

**Symptoms:** Error page instead of offline.html
**Solutions:**
- Verify offline.html cached in SW install
- Check fetch event handler logic
- Test with DevTools → Network → Offline
- Clear caches and re-register SW

## Future Enhancements

### Planned Features
- [ ] Background sync for form submissions
- [ ] Periodic background sync for updates
- [ ] Web Share API integration
- [ ] Badge API for unread notifications
- [ ] File System Access API
- [ ] WebRTC for peer-to-peer features
- [ ] Advanced caching strategies per route
- [ ] Precaching critical resources
- [ ] Dynamic manifest updates

### Experimental Features
- [ ] Install promotion in onboarding flow
- [ ] A/B testing for install prompts
- [ ] PWA-specific analytics dashboard
- [ ] Custom install UI for iOS
- [ ] Shortcuts API for quick actions

## Resources

### Documentation
- [MDN: Progressive Web Apps](https://developer.mozilla.org/en-US/docs/Web/Progressive_web_apps)
- [Web.dev: PWA](https://web.dev/progressive-web-apps/)
- [Google Developers: PWA](https://developers.google.com/web/progressive-web-apps)

### Tools
- [Lighthouse](https://developers.google.com/web/tools/lighthouse) - PWA auditing
- [Workbox](https://developers.google.com/web/tools/workbox) - Service worker library
- [PWA Builder](https://www.pwabuilder.com/) - PWA testing and generation

### Related Documentation
- [PUSH_NOTIFICATIONS.md](./PUSH_NOTIFICATIONS.md) - Push notification implementation
- [PRODUCTION_CHECKLIST.md](./PRODUCTION_CHECKLIST.md) - Deployment checklist
- [README.md](./README.md) - Project overview

## Conclusion

The New Kenya Platform PWA implementation provides a comprehensive, production-ready progressive web app experience with advanced debugging capabilities, offline support, push notifications, and installability. The PWA Debug Panel offers developers unprecedented insight into the app's PWA features during development while remaining invisible in production.
