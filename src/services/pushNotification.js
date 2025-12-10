// Push notification utility functions
import api from './api';

const convertVapidKey = (base64UrlKey) => {
  const padding = '='.repeat((4 - base64UrlKey.length % 4) % 4);
  const base64 = (base64UrlKey + padding)
    .replace(/\-/g, '+')
    .replace(/_/g, '/');

  const rawData = window.atob(base64);
  const outputArray = new Uint8Array(rawData.length);

  for (let i = 0; i < rawData.length; ++i) {
    outputArray[i] = rawData.charCodeAt(i);
  }
  return outputArray;
};

// Production VAPID key - Generated via npx web-push generate-vapid-keys
const VAPID_PUBLIC_KEY = 'BDt1RwD549VmPgqxiUnTqSS3v8ARon-crLUZJ33QjHDaERZOW-ZJ5kV0OT4A1wwzoJP_OYyO6PlF6AbGkzPm8qE';

export async function subscribeToPushNotifications() {
  try {
    // Check if service workers and push notifications are supported
    if (!('serviceWorker' in navigator) || !('PushManager' in window)) {
      throw new Error('Push notifications are not supported in this browser');
    }

    const registration = await navigator.serviceWorker.ready;
    
    // Check permission
    let permission = await Notification.requestPermission();
    if (permission !== 'granted') {
      throw new Error('Notification permission denied');
    }

    // Get existing subscription or create new one
    let subscription = await registration.pushManager.getSubscription();
    
    // If no subscription exists, create one
    if (!subscription) {
      subscription = await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: convertVapidKey(VAPID_PUBLIC_KEY)
      });
    }

    // Send subscription to server
    await sendSubscriptionToServer(subscription);
    
    return subscription;
  } catch (error) {
    console.error('Error subscribing to push notifications:', error);
    throw error;
  }
}

async function sendSubscriptionToServer(subscription) {
  try {
    const subscriptionData = subscription.toJSON();
    
    const response = await api.post('/push/subscribe', {
      endpoint: subscriptionData.endpoint,
      keys: {
        p256dh: subscriptionData.keys?.p256dh,
        auth: subscriptionData.keys?.auth
      }
    });
    
    return response.data;
  } catch (error) {
    console.error('Error sending subscription to server:', error);
    // Don't throw error to prevent blocking the subscription
    return { success: false, error: error.message };
  }
}

export async function unsubscribeFromPushNotifications() {
  try {
    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();
    
    if (subscription) {
      const subscriptionData = subscription.toJSON();
      
      // Notify server about unsubscription
      try {
        await api.post('/push/unsubscribe', {
          endpoint: subscriptionData.endpoint
        });
      } catch (error) {
        console.error('Error notifying server about unsubscription:', error);
      }
      
      // Unsubscribe locally
      await subscription.unsubscribe();
    }
    
    return true;
  } catch (error) {
    console.error('Error unsubscribing from push notifications:', error);
    throw error;
  }
}

export async function checkSubscriptionStatus() {
  try {
    if (!('serviceWorker' in navigator) || !('PushManager' in window)) {
      return { isSubscribed: false, supported: false };
    }

    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();
    
    return { 
      isSubscribed: !!subscription, 
      supported: true,
      subscription: subscription ? subscription.toJSON() : null
    };
  } catch (error) {
    console.error('Error checking subscription status:', error);
    return { isSubscribed: false, supported: true, error: error.message };
  }
}

// Send a test notification (for development/testing)
export async function sendTestNotification(title = 'Test Notification', body = 'This is a test notification') {
  try {
    // Check if we have permission
    if (Notification.permission !== 'granted') {
      throw new Error('Notification permission not granted');
    }

    const registration = await navigator.serviceWorker.ready;
    
    // Show notification
    await registration.showNotification(title, {
      body,
      icon: '/assets/icons/icon-192.svg',
      badge: '/assets/icons/icon-96.svg',
      vibrate: [100, 50, 100],
      data: {
        dateOfArrival: Date.now(),
        url: '/'
      },
      actions: [
        {
          action: 'explore',
          title: 'View Details'
        }
      ]
    });
    
    return true;
  } catch (error) {
    console.error('Error sending test notification:', error);
    throw error;
  }
}