import React, { useState, useEffect } from 'react';
// import { subscribeToPushNotifications, unsubscribeFromPushNotifications } from '../services/pushNotification';

export default function NotificationButton() {
  const [isSubscribed, setIsSubscribed] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    checkSubscription();
  }, []);

  async function checkSubscription() {
    try {
      if (!('serviceWorker' in navigator) || !('PushManager' in window)) {
        setIsLoading(false);
        return;
      }

      const registration = await navigator.serviceWorker.ready;
      const subscription = await registration.pushManager.getSubscription();
      
      setIsSubscribed(!!subscription);
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
      } else {
        await subscribeToPushNotifications();
        setIsSubscribed(true);
      }
    } catch (error) {
      console.error('Error toggling subscription:', error);
    } finally {
      setIsLoading(false);
    }
  }

  if (!('serviceWorker' in navigator) || !('PushManager' in window)) {
    return null;
  }

  return (
    <button
      onClick={toggleSubscription}
      disabled={isLoading}
      className={`px-4 py-2 rounded-md font-medium flex items-center gap-2 ${
        isSubscribed 
          ? 'bg-gray-100 text-gray-700 hover:bg-gray-200' 
          : 'bg-[var(--kenya-green)] text-white hover:opacity-90'
      }`}
    >
      {isLoading ? (
        <span className="animate-spin">↻</span>
      ) : (
        <span>{isSubscribed ? '🔔 Notifications On' : '🔕 Get Notifications'}</span>
      )}
    </button>
  );
}