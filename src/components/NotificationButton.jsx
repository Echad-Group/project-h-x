import React, { useState, useEffect } from 'react';
import { subscribeToPushNotifications, unsubscribeFromPushNotifications, checkSubscriptionStatus } from '../services/pushNotification';
import pwaAnalytics from '../services/pwaAnalytics';

export default function NotificationButton() {
  const [isSubscribed, setIsSubscribed] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [isSupported, setIsSupported] = useState(true);

  useEffect(() => {
    checkSubscription();
  }, []);

  async function checkSubscription() {
    try {
      const status = await checkSubscriptionStatus();
      setIsSubscribed(status.isSubscribed);
      setIsSupported(status.supported);
      setIsLoading(false);
    } catch (error) {
      console.error('Error checking subscription:', error);
      setIsLoading(false);
    }
  }

  async function toggleSubscription() {
    try {
      setIsLoading(true);
      if (isSubscribed) {
        await unsubscribeFromPushNotifications();
        setIsSubscribed(false);
        pwaAnalytics.trackNotification('unsubscribe');
      } else {
        await subscribeToPushNotifications();
        setIsSubscribed(true);
        pwaAnalytics.trackNotification('subscribe');
      }
    } catch (error) {
      console.error('Error toggling subscription:', error);
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
      onClick={toggleSubscription}
      disabled={isLoading}
      className={`px-4 py-2 rounded-md font-medium flex items-center gap-2 transition-all ${
        isSubscribed 
          ? 'bg-gray-100 text-gray-700 hover:bg-gray-200' 
          : 'bg-[var(--kenya-green)] text-white hover:opacity-90'
      } disabled:opacity-50 disabled:cursor-not-allowed`}
      aria-label={isSubscribed ? 'Disable notifications' : 'Enable notifications'}
    >
      {isLoading ? (
        <>
          <span className="animate-spin">↻</span>
          <span>Loading...</span>
        </>
      ) : (
        <>
          <span>{isSubscribed ? '🔔' : '🔕'}</span>
          <span>{isSubscribed ? 'Notifications On' : 'Get Notifications'}</span>
        </>
      )}
    </button>
  );
}