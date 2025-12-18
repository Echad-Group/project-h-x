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
    isStandalone: false,
    serviceWorker: null,
    serviceWorkerState: 'unknown',
    importError: null,
    pushSupport: false,
    subscription: null,
    cacheSize: 0,
    cacheNames: [],
    manifest: null,
    networkType: navigator.connection?.effectiveType || 'unknown',
    downlink: navigator.connection?.downlink || 'unknown',
    isOnline: navigator.onLine,
    lastUpdate: null,
    installPromptEvent: null,
    storageEstimate: null,
    battery: null
  });
  
  const [stats, setStats] = useState(null);
  const [notificationCategories, setNotificationCategories] = useState([]);
  const [refreshing, setRefreshing] = useState(false);
  const { t } = useTranslation();

  useEffect(() => {
    checkPWAFeatures();
    updateStats();
    loadNotificationPrefs();
    checkBattery();

    // Set up network status listeners
    window.addEventListener('online', handleNetworkChange);
    window.addEventListener('offline', handleNetworkChange);
    
    // Listen for connection changes
    if (navigator.connection) {
      navigator.connection.addEventListener('change', handleNetworkChange);
    }
    
    // Listen for service worker updates
    if ('serviceWorker' in navigator) {
      navigator.serviceWorker.addEventListener('controllerchange', handleSWUpdate);
      navigator.serviceWorker.addEventListener('message', handleSWMessage);
    }

    // Listen for beforeinstallprompt
    window.addEventListener('beforeinstallprompt', handleInstallPrompt);

    return () => {
      window.removeEventListener('online', handleNetworkChange);
      window.removeEventListener('offline', handleNetworkChange);
      window.removeEventListener('beforeinstallprompt', handleInstallPrompt);
      if (navigator.connection) {
        navigator.connection.removeEventListener('change', handleNetworkChange);
      }
    };
  }, []);

  function handleNetworkChange() {
    setDebug(prev => ({
      ...prev,
      isOnline: navigator.onLine,
      networkType: navigator.connection?.effectiveType || 'unknown',
      downlink: navigator.connection?.downlink || 'unknown'
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

  function handleSWMessage(event) {
    console.log('Service Worker message:', event.data);
    if (event.data.type === 'CACHE_UPDATED') {
      checkPWAFeatures();
    }
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

  async function checkBattery() {
    if ('getBattery' in navigator) {
      try {
        const battery = await navigator.getBattery();
        setDebug(prev => ({
          ...prev,
          battery: {
            level: Math.round(battery.level * 100),
            charging: battery.charging
          }
        }));
        
        // Listen for battery changes
        battery.addEventListener('levelchange', () => {
          setDebug(prev => ({
            ...prev,
            battery: {
              level: Math.round(battery.level * 100),
              charging: battery.charging
            }
          }));
        });
        
        battery.addEventListener('chargingchange', () => {
          setDebug(prev => ({
            ...prev,
            battery: {
              level: Math.round(battery.level * 100),
              charging: battery.charging
            }
          }));
        });
      } catch (error) {
        console.log('Battery API not available');
      }
    }
  }

  async function checkPWAFeatures() {
    const features = {
      isInstallable: window.matchMedia('(display-mode: standalone)').matches,
      isStandalone: window.matchMedia('(display-mode: standalone)').matches || 
                    window.navigator.standalone === true,
      serviceWorker: 'serviceWorker' in navigator,
      serviceWorkerState: 'unknown',
      pushSupport: 'PushManager' in window,
      subscription: null,
      cacheSize: 0,
      cacheNames: [],
      manifest: null,
      storageEstimate: null
    };

    try {
      // Check service worker status
      if (features.serviceWorker) {
        const registration = await navigator.serviceWorker.ready;
        features.serviceWorkerState = registration.active?.state || 'none';
        features.subscription = await registration.pushManager.getSubscription();
      }

      // Check cache size and names
      if ('caches' in window) {
        const cacheNames = await caches.keys();
        features.cacheNames = cacheNames;
        
        let totalSize = 0;
        for (const cacheName of cacheNames) {
          const cache = await caches.open(cacheName);
          const keys = await cache.keys();
          totalSize += keys.length;
        }
        features.cacheSize = totalSize;
      }

      // Get manifest
      const manifestLink = document.querySelector('link[rel="manifest"]');
      if (manifestLink) {
        const manifestResponse = await fetch(manifestLink.href);
        features.manifest = await manifestResponse.json();
      }

      // Get storage estimate
      if ('storage' in navigator && 'estimate' in navigator.storage) {
        const estimate = await navigator.storage.estimate();
        features.storageEstimate = {
          usage: estimate.usage,
          quota: estimate.quota,
          percent: Math.round((estimate.usage / estimate.quota) * 100)
        };
      }

      setDebug(prev => ({ ...prev, ...features }));
    } catch (error) {
      console.error('Error checking PWA features:', error);
    }
  }

  async function clearCache() {
    try {
      setRefreshing(true);
      const cacheNames = await caches.keys();
      await Promise.all(cacheNames.map(name => caches.delete(name)));
      await checkPWAFeatures();
    } catch (error) {
      console.error('Error clearing cache:', error);
    } finally {
      setRefreshing(false);
    }
  }

  async function unregisterServiceWorker() {
    try {
      if ('serviceWorker' in navigator) {
        const registrations = await navigator.serviceWorker.getRegistrations();
        for (const registration of registrations) {
          await registration.unregister();
        }
        await checkPWAFeatures();
      }
    } catch (error) {
      console.error('Error unregistering service worker:', error);
    }
  }

  async function updateServiceWorker() {
    try {
      if ('serviceWorker' in navigator) {
        const registration = await navigator.serviceWorker.ready;
        await registration.update();
        await checkPWAFeatures();
      }
    } catch (error) {
      console.error('Error updating service worker:', error);
    }
  }

  function formatBytes(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
  }

  function getNetworkIcon(type) {
    const icons = {
      '4g': '📶',
      '3g': '📶',
      '2g': '📶',
      'slow-2g': '📡',
      'wifi': '📶',
      'unknown': '❓'
    };
    return icons[type] || icons.unknown;
  }

  // Helper component for status rows
  function StatusRow({ label, value, badge = null }) {
    return (
      <div className="flex justify-between items-center">
        <span className="text-gray-600">{label}</span>
        <span className="flex items-center gap-2">
          <span className={`font-medium ${value ? 'text-green-600' : 'text-red-600'}`}>
            {value ? '✅' : '❌'}
          </span>
          {badge && (
            <span className="text-xs bg-blue-100 text-blue-700 px-2 py-0.5 rounded-full">
              {badge}
            </span>
          )}
        </span>
      </div>
    );
  }

  if (process.env.NODE_ENV === 'production') {
    return null;
  }

  return (
    <div className="fixed bottom-4 right-4 z-50">
      {/* Toggle Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="bg-gradient-to-r from-blue-600 to-purple-600 text-white p-3 rounded-full shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-110"
        title="PWA Debug Panel"
      >
        {isOpen ? (
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        ) : (
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
        )}
      </button>

      {/* Debug Panel */}
      {isOpen && (
        <div className="absolute bottom-16 right-0 w-[420px] bg-white rounded-lg shadow-2xl border border-gray-200 max-h-[85vh] overflow-hidden flex flex-col">
          {/* Header */}
          <div className="bg-gradient-to-r from-blue-600 to-purple-600 text-white p-4">
            <div className="flex justify-between items-center mb-3">
              <h3 className="font-bold text-lg flex items-center gap-2">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 3v2m6-2v2M9 19v2m6-2v2M5 9H3m2 6H3m18-6h-2m2 6h-2M7 19h10a2 2 0 002-2V7a2 2 0 00-2-2H7a2 2 0 00-2 2v10a2 2 0 002 2zM9 9h6v6H9V9z" />
                </svg>
                PWA Debug Panel
              </h3>
              <button
                onClick={() => {
                  setRefreshing(true);
                  checkPWAFeatures();
                  updateStats();
                  checkBattery();
                  setTimeout(() => setRefreshing(false), 500);
                }}
                className="p-1 hover:bg-white/20 rounded transition-colors"
                disabled={refreshing}
              >
                <svg className={`w-5 h-5 ${refreshing ? 'animate-spin' : ''}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                </svg>
              </button>
            </div>
            
            {/* Tab Navigation */}
            <div className="flex gap-1">
              {[
                { id: 'status', label: 'Status', icon: '📊' },
                { id: 'storage', label: 'Storage', icon: '💾' },
                { id: 'analytics', label: 'Analytics', icon: '📈' },
                { id: 'notifications', label: 'Notifications', icon: '🔔' }
              ].map(tab => (
                <button
                  key={tab.id}
                  onClick={() => setActiveTab(tab.id)}
                  className={`flex-1 px-2 py-2 text-sm rounded transition-all duration-200 ${
                    activeTab === tab.id 
                      ? 'bg-white text-blue-600 shadow-md font-semibold' 
                      : 'bg-white/10 hover:bg-white/20 text-white'
                  }`}
                >
                  <span className="mr-1">{tab.icon}</span>
                  {tab.label}
                </button>
              ))}
            </div>
          </div>

          {/* Content Area */}
          <div className="overflow-y-auto flex-1 p-4">
            {activeTab === 'status' && (
              <div className="space-y-4">
                {/* PWA Status */}
                <div className="bg-gradient-to-br from-blue-50 to-purple-50 rounded-lg p-4 border border-blue-100">
                  <h4 className="font-semibold text-sm text-gray-700 mb-3 flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    PWA Features
                  </h4>
                  <div className="space-y-2 text-sm">
                    <StatusRow label="Standalone Mode" value={debug.isStandalone} />
                    <StatusRow label="Service Worker" value={debug.serviceWorker} badge={debug.serviceWorkerState} />
                    <StatusRow label="Push Notifications" value={debug.pushSupport} />
                    <StatusRow label="Push Subscription" value={!!debug.subscription} />
                    <StatusRow label="Installable" value={!!debug.installPromptEvent} />
                  </div>
                </div>

                {/* Network Status */}
                <div className="bg-gradient-to-br from-green-50 to-teal-50 rounded-lg p-4 border border-green-100">
                  <h4 className="font-semibold text-sm text-gray-700 mb-3 flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.111 16.404a5.5 5.5 0 017.778 0M12 20h.01m-7.08-7.071c3.904-3.905 10.236-3.905 14.141 0M1.394 9.393c5.857-5.857 15.355-5.857 21.213 0" />
                    </svg>
                    Network Information
                  </h4>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between items-center">
                      <span className="text-gray-600">Status</span>
                      <span className={`font-medium flex items-center gap-1 ${debug.isOnline ? 'text-green-600' : 'text-red-600'}`}>
                        <span className={`w-2 h-2 rounded-full ${debug.isOnline ? 'bg-green-500' : 'bg-red-500'} animate-pulse`}></span>
                        {debug.isOnline ? 'Online' : 'Offline'}
                      </span>
                    </div>
                    <div className="flex justify-between items-center">
                      <span className="text-gray-600">Connection Type</span>
                      <span className="font-medium">{getNetworkIcon(debug.networkType)} {debug.networkType}</span>
                    </div>
                    {debug.downlink !== 'unknown' && (
                      <div className="flex justify-between items-center">
                        <span className="text-gray-600">Downlink Speed</span>
                        <span className="font-medium">{debug.downlink} Mbps</span>
                      </div>
                    )}
                  </div>
                </div>

                {/* Battery Status */}
                {debug.battery && (
                  <div className="bg-gradient-to-br from-yellow-50 to-orange-50 rounded-lg p-4 border border-yellow-100">
                    <h4 className="font-semibold text-sm text-gray-700 mb-3 flex items-center gap-2">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
                      </svg>
                      Battery
                    </h4>
                    <div className="space-y-2 text-sm">
                      <div className="flex justify-between items-center">
                        <span className="text-gray-600">Level</span>
                        <span className="font-medium">{debug.battery.level}%</span>
                      </div>
                      <div className="flex justify-between items-center">
                        <span className="text-gray-600">Charging</span>
                        <span className="font-medium">{debug.battery.charging ? '⚡ Yes' : 'No'}</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-2">
                        <div 
                          className={`h-2 rounded-full transition-all ${debug.battery.level > 20 ? 'bg-green-500' : 'bg-red-500'}`}
                          style={{ width: `${debug.battery.level}%` }}
                        ></div>
                      </div>
                    </div>
                  </div>
                )}

                {/* System Info */}
                <div className="bg-gray-50 rounded-lg p-4 border border-gray-200">
                  <h4 className="font-semibold text-sm text-gray-700 mb-3 flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    System Information
                  </h4>
                  <div className="space-y-2 text-sm">
                    <div className="flex justify-between">
                      <span className="text-gray-600">Last SW Update</span>
                      <span className="text-gray-900 text-xs">
                        {debug.lastUpdate ? new Date(debug.lastUpdate).toLocaleString() : 'Never'}
                      </span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-gray-600">User Agent</span>
                      <span className="text-gray-900 text-xs truncate max-w-[200px]" title={navigator.userAgent}>
                        {navigator.userAgent.split(' ').pop()}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Actions */}
                <div className="space-y-2">
                  {debug.installPromptEvent && (
                    <button
                      onClick={() => {
                        debug.installPromptEvent.prompt();
                        pwaAnalytics.trackEvent('install', 'manual_trigger');
                      }}
                      className="w-full px-4 py-3 bg-gradient-to-r from-green-500 to-teal-500 text-white rounded-md hover:from-green-600 hover:to-teal-600 transition-all font-medium shadow-md hover:shadow-lg flex items-center justify-center gap-2"
                    >
                      <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                      </svg>
                      Install App
                    </button>
                  )}
                  
                  <button
                    onClick={updateServiceWorker}
                    className="w-full px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600 transition-all font-medium flex items-center justify-center gap-2"
                  >
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                    </svg>
                    Update Service Worker
                  </button>
                </div>
              </div>
            )}

            {activeTab === 'storage' && (
              <div className="space-y-4">
                {/* Storage Estimate */}
                {debug.storageEstimate && (
                  <div className="bg-gradient-to-br from-purple-50 to-pink-50 rounded-lg p-4 border border-purple-100">
                    <h4 className="font-semibold text-sm text-gray-700 mb-3">Storage Usage</h4>
                    <div className="space-y-2">
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-600">Used</span>
                        <span className="font-medium">{formatBytes(debug.storageEstimate.usage)}</span>
                      </div>
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-600">Quota</span>
                        <span className="font-medium">{formatBytes(debug.storageEstimate.quota)}</span>
                      </div>
                      <div className="w-full bg-gray-200 rounded-full h-3 overflow-hidden">
                        <div 
                          className="h-3 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full transition-all"
                          style={{ width: `${debug.storageEstimate.percent}%` }}
                        ></div>
                      </div>
                      <div className="text-center text-sm font-medium text-purple-600">
                        {debug.storageEstimate.percent}% used
                      </div>
                    </div>
                  </div>
                )}

                {/* Cache Information */}
                <div className="bg-gray-50 rounded-lg p-4 border border-gray-200">
                  <h4 className="font-semibold text-sm text-gray-700 mb-3">Cache Storage</h4>
                  <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-600">Total Cached Items</span>
                      <span className="font-bold text-blue-600">{debug.cacheSize}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-600">Cache Names</span>
                      <span className="font-bold text-blue-600">{debug.cacheNames.length}</span>
                    </div>
                  </div>
                  
                  {debug.cacheNames.length > 0 && (
                    <div className="mt-3 pt-3 border-t border-gray-200">
                      <p className="text-xs text-gray-600 mb-2">Active Caches:</p>
                      <div className="space-y-1">
                        {debug.cacheNames.map(name => (
                          <div key={name} className="text-xs bg-white px-2 py-1 rounded border border-gray-200 font-mono">
                            {name}
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>

                {/* Manifest Info */}
                {debug.manifest && (
                  <div className="bg-blue-50 rounded-lg p-4 border border-blue-100">
                    <h4 className="font-semibold text-sm text-gray-700 mb-3">App Manifest</h4>
                    <div className="space-y-2 text-sm">
                      <div className="flex justify-between">
                        <span className="text-gray-600">Name</span>
                        <span className="font-medium">{debug.manifest.name}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">Short Name</span>
                        <span className="font-medium">{debug.manifest.short_name}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">Theme Color</span>
                        <span className="flex items-center gap-2">
                          <span 
                            className="w-4 h-4 rounded border border-gray-300" 
                            style={{ backgroundColor: debug.manifest.theme_color }}
                          ></span>
                          <span className="font-mono text-xs">{debug.manifest.theme_color}</span>
                        </span>
                      </div>
                    </div>
                  </div>
                )}

                {/* Storage Actions */}
                <div className="space-y-2">
                  <button
                    onClick={clearCache}
                    disabled={refreshing}
                    className="w-full px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600 transition-all font-medium disabled:opacity-50 flex items-center justify-center gap-2"
                  >
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    Clear All Caches
                  </button>
                  
                  <button
                    onClick={unregisterServiceWorker}
                    className="w-full px-4 py-2 bg-orange-500 text-white rounded-md hover:bg-orange-600 transition-all font-medium flex items-center justify-center gap-2"
                  >
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
                    </svg>
                    Unregister Service Worker
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
                    className="w-full px-4 py-2 bg-red-500 text-white rounded-md hover:bg-red-600 font-medium flex items-center justify-center gap-2"
                  >
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    Reset Analytics
                  </button>
                </div>
              </div>
            )}

            {activeTab === 'notifications' && (
              <div className="space-y-4">
                <div>
                  <h4 className="font-semibold mb-2">Notification Permission</h4>
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
                    className="px-4 py-2 bg-purple-500 text-white rounded-md hover:bg-purple-600 font-medium flex items-center gap-2"
                  >
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                    </svg>
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
                      Export
                    </button>
                    <label className="px-3 py-1 text-sm bg-green-500 text-white rounded hover:bg-green-600 cursor-pointer">
                      Import
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
                              setDebug(prev => ({ ...prev, importError: 'Import failed' }));
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
                    <div key={category.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg border border-gray-200">
                      <div>
                        <div className="font-medium text-sm">{category.label}</div>
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
                  <div className="flex items-center gap-2 p-2 bg-gray-50 rounded-lg">
                    <input
                      type="checkbox"
                      checked={notificationPrefs.getPreferences().schedule.quiet}
                      onChange={(e) => {
                        notificationPrefs.updateQuietHours(e.target.checked);
                        loadNotificationPrefs();
                      }}
                      className="w-4 h-4 text-blue-600 rounded focus:ring-2 focus:ring-blue-500"
                    />
                    <span className="text-sm">Enable Quiet Hours</span>
                  </div>
                  <div className="grid grid-cols-2 gap-2">
                    <div>
                      <label className="text-xs text-gray-600 block mb-1">Start Time</label>
                      <input
                        type="time"
                        value={notificationPrefs.getPreferences().schedule.quietHoursStart}
                        onChange={(e) => {
                          notificationPrefs.updateQuietHours(true, e.target.value);
                          loadNotificationPrefs();
                        }}
                        className="w-full p-2 text-sm border border-gray-300 rounded focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      />
                    </div>
                    <div>
                      <label className="text-xs text-gray-600 block mb-1">End Time</label>
                      <input
                        type="time"
                        value={notificationPrefs.getPreferences().schedule.quietHoursEnd}
                        onChange={(e) => {
                          notificationPrefs.updateQuietHours(true, null, e.target.value);
                          loadNotificationPrefs();
                        }}
                        className="w-full p-2 text-sm border border-gray-300 rounded focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                      />
                    </div>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
