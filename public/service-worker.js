// Service Worker
const CACHE_NAME = 'new-kenya-v1';
const OFFLINE_URL = '/offline.html';

// Handle push notifications
self.addEventListener('push', (event) => {
  if (!event.data) return;

  try {
    const data = event.data.json();
    const options = {
      body: data.body,
      icon: '/assets/icons/icon-192.svg',
      badge: '/assets/icons/icon-96.svg',
      vibrate: [100, 50, 100],
      data: {
        dateOfArrival: Date.now(),
        primaryKey: 1,
        url: data.url || '/'
      },
      actions: [
        {
          action: 'explore',
          title: 'View Details',
          icon: '/assets/icons/icon-48.svg'
        }
      ]
    };

    event.waitUntil(
      self.registration.showNotification(data.title, options)
    );
  } catch (error) {
    console.error('Error showing notification:', error);
  }
});

// Handle notification clicks
self.addEventListener('notificationclick', (event) => {
  event.notification.close();

  if (event.action === 'explore') {
    const urlToOpen = event.notification.data.url;
    event.waitUntil(
      clients.openWindow(urlToOpen)
    );
  }
});

const STATIC_ASSETS = [
  '/',
  '/index.html',
  '/manifest.json',
  '/assets/app-icon.svg',
  '/assets/og-image.svg',
  '/assets/icons/icon-16.svg',
  '/assets/icons/icon-512.svg'
];

// Install event - cache static assets
self.addEventListener('install', (event) => {
  event.waitUntil(
    caches.open(CACHE_NAME).then((cache) => {
      return cache.addAll(STATIC_ASSETS);
    })
  );
  self.skipWaiting();
});

// Activate event - clean up old caches
self.addEventListener('activate', (event) => {
  event.waitUntil(
    caches.keys().then((cacheNames) => {
      return Promise.all(
        cacheNames
          .filter((name) => name !== CACHE_NAME)
          .map((name) => caches.delete(name))
      );
    })
  );
  self.clients.claim();
});

// Fetch event - network first, fallback to cache
self.addEventListener('fetch', (event) => {
  // Skip cross-origin requests
  if (!event.request.url.startsWith(self.location.origin)) return;

  event.respondWith(
    fetch(event.request)
      .then((response) => {
        // Cache successful responses
        if (response.ok) {
          const responseClone = response.clone();
          caches.open(CACHE_NAME).then((cache) => {
            cache.put(event.request, responseClone);
          });
        }
        return response;
      })
      .catch(async () => {
        // Fallback to cache
        const cachedResponse = await caches.match(event.request);
        if (cachedResponse) return cachedResponse;

        // Show offline page if HTML is requested
        if (event.request.mode === 'navigate') {
          return caches.match(OFFLINE_URL);
        }
        return new Response('Network error', { status: 408 });
      })
  );
});