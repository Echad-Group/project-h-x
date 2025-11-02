import React, { useState, useEffect } from 'react';
import pwaAnalytics from '../services/pwaAnalytics';

export default function InstallPrompt() {
  const [installPromptEvent, setInstallPromptEvent] = useState(null);
  const [isInstalled, setIsInstalled] = useState(false);
  const [showPrompt, setShowPrompt] = useState(false);

  useEffect(() => {
    // Check if already installed
    if (window.matchMedia('(display-mode: standalone)').matches) {
      setIsInstalled(true);
      return;
    }

    // Listen for beforeinstallprompt
    const handleBeforeInstallPrompt = (e) => {
      e.preventDefault();
      setInstallPromptEvent(e);
      setShowPrompt(true);
      pwaAnalytics.trackEvent('install', 'prompt_ready');
    };

    // Listen for successful installation
    const handleAppInstalled = () => {
      setIsInstalled(true);
      setShowPrompt(false);
      pwaAnalytics.trackInstall();
      pwaAnalytics.trackEvent('install', 'success');
    };

    window.addEventListener('beforeinstallprompt', handleBeforeInstallPrompt);
    window.addEventListener('appinstalled', handleAppInstalled);

    return () => {
      window.removeEventListener('beforeinstallprompt', handleBeforeInstallPrompt);
      window.removeEventListener('appinstalled', handleAppInstalled);
    };
  }, []);

  const handleInstall = async () => {
    if (!installPromptEvent) return;

    try {
      const result = await installPromptEvent.prompt();
      console.log('Install prompt result:', result);
      pwaAnalytics.trackEvent('install', 'prompt_shown');
    } catch (error) {
      console.error('Error showing install prompt:', error);
      pwaAnalytics.trackEvent('install', 'error', error.message);
    }
  };

  if (!showPrompt || isInstalled) return null;

  return (
    <div className="fixed bottom-4 left-4 z-50 flex items-center bg-white rounded-lg shadow-lg p-4 gap-4 max-w-sm animate-slide-up">
      <div className="flex-shrink-0">
        <img 
          src="/src/assets/icons/icon-96.svg" 
          alt="App Icon" 
          className="w-12 h-12"
        />
      </div>
      <div className="flex-1">
        <h3 className="font-semibold text-gray-900">Install New Kenya</h3>
        <p className="text-sm text-gray-600">Get quick access to updates and stay connected with the movement</p>
        <div className="mt-2 flex gap-2">
          <button
            onClick={handleInstall}
            className="px-4 py-2 bg-[var(--kenya-green)] text-white rounded-md text-sm font-medium hover:bg-opacity-90"
          >
            Install Now
          </button>
          <button
            onClick={() => {
              setShowPrompt(false);
              pwaAnalytics.trackEvent('install', 'dismissed');
            }}
            className="px-4 py-2 text-gray-600 text-sm font-medium hover:text-gray-900"
          >
            Maybe Later
          </button>
        </div>
      </div>
    </div>
  );
}