import React, { useState, useEffect } from 'react';
import NotificationButton from './NotificationButton';

export default function PWADebugPanel() {
  const [isOpen, setIsOpen] = useState(false);
  const [debug, setDebug] = useState({
    isInstallable: false,
    serviceWorker: null,
    pushSupport: false,
    subscription: null,
    cacheSize: 0,
    manifest: null
  });

  useEffect(() => {
    checkPWAFeatures();
  }, []);

  async function checkPWAFeatures() {
    const features = {
      isInstallable: window.matchMedia('(display-mode: standalone)').matches,
      serviceWorker: 'serviceWorker' in navigator,
      pushSupport: 'PushManager' in window,
      subscription: null,
      cacheSize: 0,
      manifest: null
    };

    try {
      // Check service worker status
      if (features.serviceWorker) {
        const registration = await navigator.serviceWorker.ready;
        features.subscription = await registration.pushManager.getSubscription();
      }

      // Check cache size
      if ('caches' in window) {
        const cache = await caches.open('new-kenya-v1');
        const keys = await cache.keys();
        features.cacheSize = keys.length;
      }

      // Get manifest
      const manifestLink = document.querySelector('link[rel="manifest"]');
      if (manifestLink) {
        const manifestResponse = await fetch(manifestLink.href);
        features.manifest = await manifestResponse.json();
      }

      setDebug(features);
    } catch (error) {
      console.error('Error checking PWA features:', error);
    }
  }

  async function clearCache() {
    try {
      await caches.delete('new-kenya-v1');
      await checkPWAFeatures();
    } catch (error) {
      console.error('Error clearing cache:', error);
    }
  }

  if (process.env.NODE_ENV === 'production') {
    return null;
  }

  return (
    <div className="fixed bottom-4 right-4 z-50">
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="bg-gray-900 text-white p-2 rounded-full shadow-lg"
      >
        {isOpen ? '×' : '🔧'}
      </button>

      {isOpen && (
        <div className="absolute bottom-12 right-0 w-80 bg-white rounded-lg shadow-xl p-4 space-y-4">
          <h3 className="font-bold text-lg">PWA Debug Panel</h3>
          
          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span>Installable:</span>
              <span>{debug.isInstallable ? '✅' : '❌'}</span>
            </div>
            
            <div className="flex justify-between">
              <span>Service Worker:</span>
              <span>{debug.serviceWorker ? '✅' : '❌'}</span>
            </div>
            
            <div className="flex justify-between">
              <span>Push Support:</span>
              <span>{debug.pushSupport ? '✅' : '❌'}</span>
            </div>
            
            <div className="flex justify-between">
              <span>Push Subscription:</span>
              <span>{debug.subscription ? '✅' : '❌'}</span>
            </div>
            
            <div className="flex justify-between">
              <span>Cached Resources:</span>
              <span>{debug.cacheSize}</span>
            </div>
          </div>

          <div className="space-y-2">
            <NotificationButton />
            
            <button
              onClick={clearCache}
              className="w-full px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
            >
              Clear Cache
            </button>
            
            <button
              onClick={checkPWAFeatures}
              className="w-full px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
            >
              Refresh Status
            </button>
          </div>

          {debug.manifest && (
            <div className="text-xs mt-2 p-2 bg-gray-100 rounded">
              <pre>{JSON.stringify(debug.manifest, null, 2)}</pre>
            </div>
          )}
        </div>
      )}
    </div>
  );
}