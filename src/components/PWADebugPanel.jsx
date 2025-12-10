import React, { useState, useEffect } from 'react';
import NotificationButton from './NotificationButton';
import pwaAnalytics from '../services/pwaAnalytics';
import notificationPrefs from '../services/notificationPreferences';
import AnalyticsVisualizations from './AnalyticsVisualizations';
import { useTranslation } from 'react-i18next';

export default function PWADebugPanel() {
  const [isOpen, setIsOpen] = useState(false);
  const [activeTab, setActiveTab] = useState('status');
  const [debug, setDebug] = useState({
    isInstallable: false,
    serviceWorker: null,
     importError: null,
    pushSupport: false,
    subscription: null,
    cacheSize: 0,
    manifest: null,
    networkType: navigator.connection?.type || 'unknown',
    isOnline: navigator.onLine,
    lastUpdate: null,
    installPromptEvent: null
  });
  
  const [stats, setStats] = useState(null);
  const [notificationCategories, setNotificationCategories] = useState([]);
  const { t } = useTranslation();

  useEffect(() => {
    checkPWAFeatures();
    updateStats();
    loadNotificationPrefs();

    // Set up network status listeners
    window.addEventListener('online', handleNetworkChange);
    window.addEventListener('offline', handleNetworkChange);
    
    // Listen for service worker updates
    if ('serviceWorker' in navigator) {
      navigator.serviceWorker.addEventListener('controllerchange', handleSWUpdate);
    }

    // Listen for beforeinstallprompt
    window.addEventListener('beforeinstallprompt', handleInstallPrompt);

    return () => {
      window.removeEventListener('online', handleNetworkChange);
      window.removeEventListener('offline', handleNetworkChange);
      window.removeEventListener('beforeinstallprompt', handleInstallPrompt);
    };
  }, []);

  function handleNetworkChange() {
    setDebug(prev => ({
      ...prev,
      isOnline: navigator.onLine,
      networkType: navigator.connection?.type || 'unknown'
    }));
    if (!navigator.onLine) {
      pwaAnalytics.trackOfflineUsage();
    }
  }

  function handleSWUpdate() {
    setDebug(prev => ({
      ...prev,
      lastUpdate: new Date().toISOString()
    }));
    checkPWAFeatures();
  }

  function handleInstallPrompt(event) {
    event.preventDefault();
    setDebug(prev => ({
      ...prev,
      installPromptEvent: event
    }));
  }

  function updateStats() {
    setStats(pwaAnalytics.getStats());
  }

  function loadNotificationPrefs() {
    setNotificationCategories(notificationPrefs.getCategories());
  }

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
        <div className="absolute bottom-12 right-0 w-96 bg-white rounded-lg shadow-xl p-4 max-h-[80vh] overflow-y-auto">
          <div className="flex justify-between items-center mb-4">
            <h3 className="font-bold text-lg">{t('pwa.title')}</h3>
            <div className="flex gap-2">
              {['status', 'analytics', 'notifications'].map(tab => (
                <button
                  key={tab}
                  onClick={() => setActiveTab(tab)}
                  className={`px-3 py-1 text-sm rounded ${
                    activeTab === tab 
                      ? 'bg-blue-500 text-white' 
                      : 'bg-gray-100 hover:bg-gray-200'
                  }`}
                >
                  {t(`pwa.tabs.${tab}`)}
                </button>
              ))}
            </div>
          </div>

          {activeTab === 'status' && (
            <div className="space-y-4">
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span>{t('pwa.status.installable')}</span>
                  <span>{debug.isInstallable ? '✅' : '❌'}</span>
                </div>
                <div className="flex justify-between">
                  <span>{t('pwa.status.serviceWorker')}</span>
                  <span>{debug.serviceWorker ? '✅' : '❌'}</span>
                </div>
                <div className="flex justify-between">
                  <span>{t('pwa.status.network')}</span>
                  <span>{debug.isOnline ? `🟢 ${t('pwa.online')}` : `🔴 ${t('pwa.offline')}`}</span>
                </div>
                <div className="flex justify-between">
                  <span>{t('pwa.status.networkType')}</span>
                  <span>{debug.networkType}</span>
                </div>
                <div className="flex justify-between">
                  <span>{t('pwa.status.lastUpdate')}</span>
                  <span>{debug.lastUpdate ? new Date(debug.lastUpdate).toLocaleString() : t('pwa.never')}</span>
                </div>
                <div className="flex justify-between">
                  <span>{t('pwa.status.cachedResources')}</span>
                  <span>{debug.cacheSize}</span>
                </div>
              </div>

              <div className="space-y-2">
                {debug.installPromptEvent && (
                  <button
                    onClick={() => debug.installPromptEvent.prompt()}
                    className="w-full px-4 py-2 bg-green-500 text-white rounded-md hover:bg-green-600"
                  >
                    {t('pwa.install')}
                  </button>
                )}
                
                <button
                  onClick={clearCache}
                  className="w-full px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
                >
                  {t('pwa.clearCache')}
                </button>
                
                <button
                  onClick={() => {
                    checkPWAFeatures();
                    updateStats();
                  }}
                  className="w-full px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
                >
                  {t('pwa.refresh')}
                </button>
              </div>
            </div>
          )}

          {activeTab === 'analytics' && stats && (
            <div className="space-y-4">
                <AnalyticsVisualizations stats={stats} />
                <div className="mt-4 pt-4 border-t">
                  <button
                    onClick={() => {
                      pwaAnalytics.clearData();
                      updateStats();
                    }}
                    className="w-full px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600"
                  >
                    {t('pwa.resetAnalytics')}
                  </button>
                </div>
            </div>
          )}

          {activeTab === 'notifications' && (
            <div className="space-y-4">
              <div>
                <h4 className="font-semibold mb-2">{t('pwa.notifications.title')}</h4>
                <NotificationButton />
              </div>

              <div>
                <h4 className="font-semibold mb-2">Test Notification</h4>
                <button
                  onClick={async () => {
                    try {
                      const { sendTestNotification } = await import('../services/pushNotification');
                      await sendTestNotification('Test Notification', 'This is a test from the debug panel');
                      alert('Test notification sent!');
                    } catch (error) {
                      alert('Failed to send test notification: ' + error.message);
                    }
                  }}
                  className="px-3 py-1 text-sm bg-purple-500 text-white rounded hover:bg-purple-600"
                >
                  Send Test
                </button>
              </div>

              <div className="space-y-2">
                <h4 className="font-semibold">Categories</h4>
                  <div className="flex gap-2 mb-4">
                    <button
                      onClick={() => notificationPrefs.exportPreferences()}
                      className="px-3 py-1 text-sm bg-blue-500 text-white rounded hover:bg-blue-600"
                    >
                      {t('pwa.notifications.export')}
                    </button>
                    <label className="px-3 py-1 text-sm bg-green-500 text-white rounded hover:bg-green-600 cursor-pointer">
                      {t('pwa.notifications.import')}
                      <input
                        type="file"
                        accept=".json"
                        className="hidden"
                        onChange={async (e) => {
                          if (e.target.files?.length) {
                            const success = await notificationPrefs.importPreferences(e.target.files[0]);
                            if (success) {
                              loadNotificationPrefs();
                              setDebug(prev => ({ ...prev, importError: null }));
                            } else {
                              setDebug(prev => ({ ...prev, importError: t('pwa.notifications.importFailed') }));
                            }
                          }
                        }}
                      />
                    </label>
                  </div>
                  {debug.importError && (
                    <div className="text-sm text-red-500 mb-4">
                      {debug.importError}
                    </div>
                  )}
                {notificationCategories.map(category => (
                  <div key={category.id} className="flex items-center justify-between p-2 bg-gray-50 rounded">
                    <div>
                      <div className="font-medium">{category.label}</div>
                      <div className="text-xs text-gray-500">{category.description}</div>
                    </div>
                    <label className="relative inline-flex items-center cursor-pointer">
                      <input
                        type="checkbox"
                        checked={category.enabled}
                        onChange={(e) => {
                          notificationPrefs.updateCategory(category.id, e.target.checked);
                          loadNotificationPrefs();
                        }}
                        className="sr-only peer"
                      />
                      <div className="w-11 h-6 bg-gray-200 rounded-full peer peer-focus:ring-4 peer-focus:ring-blue-300 peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-0.5 after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                    </label>
                  </div>
                ))}
              </div>

              <div className="space-y-2">
                <h4 className="font-semibold">Quiet Hours</h4>
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    checked={notificationPrefs.getPreferences().schedule.quiet}
                    onChange={(e) => {
                      notificationPrefs.updateQuietHours(e.target.checked);
                      loadNotificationPrefs();
                    }}
                  />
                  <span>Enable Quiet Hours</span>
                </div>
                <div className="grid grid-cols-2 gap-2">
                  <div>
                    <label className="text-xs text-gray-500">Start</label>
                    <input
                      type="time"
                      value={notificationPrefs.getPreferences().schedule.quietHoursStart}
                      onChange={(e) => {
                        notificationPrefs.updateQuietHours(true, e.target.value);
                        loadNotificationPrefs();
                      }}
                      className="w-full p-1 border rounded"
                    />
                  </div>
                  <div>
                    <label className="text-xs text-gray-500">End</label>
                    <input
                      type="time"
                      value={notificationPrefs.getPreferences().schedule.quietHoursEnd}
                      onChange={(e) => {
                        notificationPrefs.updateQuietHours(true, null, e.target.value);
                        loadNotificationPrefs();
                      }}
                      className="w-full p-1 border rounded"
                    />
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}