import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { subscribeToPushNotifications, unsubscribeFromPushNotifications, checkSubscriptionStatus } from '../services/pushNotification';
import pwaAnalytics from '../services/pwaAnalytics';

export default function NotificationButton() {
  const [isSubscribed, setIsSubscribed] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSupported, setIsSupported] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    // Check subscription status without blocking UI
    checkSubscription();
  }, []);

  async function checkSubscription() {
    try {
      // Don't check if we're clearly offline or backend is unreachable
      if (!navigator.onLine) {
        setIsSubscribed(false);
        setIsSupported(true);
        setIsLoading(false);
        return;
      }
      
      const status = await checkSubscriptionStatus();
      setIsSubscribed(status.isSubscribed);
      setIsSupported(status.supported);
    } catch (error) {
      console.error('Error checking subscription:', error);
      // Fail silently - assume not subscribed
      setIsSubscribed(false);
      setIsSupported(true);
    } finally {
      setIsLoading(false);
    }
  }

  async function handleClick() {
    try {
      if (isSubscribed) {
        // Navigate to settings page when notifications are already on
        navigate('/notification-settings');
        pwaAnalytics.trackEvent('notification', 'open_settings');
      } else {
        // Subscribe when notifications are off
        setIsLoading(true);
        await subscribeToPushNotifications();
        setIsSubscribed(true);
        pwaAnalytics.trackNotification('subscribe');
      }
    } catch (error) {
      console.error('Error handling notification action:', error);
      pwaAnalytics.trackEvent('notification', 'error', error.message);
      alert(error.message || 'Failed to update notification settings');
    } finally {
      setIsLoading(false);
    }
  }

  if (!isSupported) {
    return null;
  }

  return (
    <button
      onClick={handleClick}
      disabled={isLoading}
      className={`px-4 py-2 rounded-full font-medium flex items-center gap-2 transition-all shadow-lg hover:shadow-xl ${
        isSubscribed 
          ? 'bg-white text-gray-700 hover:bg-gray-50 border border-gray-200 hover:border-[var(--kenya-green)]' 
          : 'bg-[var(--kenya-green)] text-white hover:opacity-90'
      } disabled:opacity-50 disabled:cursor-not-allowed`}
      aria-label={isSubscribed ? 'Open notification settings' : 'Enable notifications'}
      title={isSubscribed ? 'Click to manage notification settings' : 'Click to enable notifications'}
    >
      {isLoading ? (
        <>
          <span className="animate-spin">↻</span>
          <span>Loading...</span>
        </>
      ) : (
        <>
          <span>{isSubscribed ? '🔔' : '🔕'}</span>
          <span className="hidden sm:inline">{isSubscribed ? 'Settings' : 'Get Notifications'}</span>
        </>
      )}
    </button>
  );
}