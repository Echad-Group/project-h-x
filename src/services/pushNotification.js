// Push notification utility functions
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

// Sample VAPID key - Replace with your actual VAPID public key
const VAPID_PUBLIC_KEY = 'BJ5IxJBWdeqFDJTvrZ4wNRu7UY2XigDXjgiUBYEYVXDudxhEs0ReOJRBcBHsPYgZ5dyV8VjyqzbQKS8V7bUAglk';

// Development mode flag
const isDevelopment = process.env.NODE_ENV === 'development';

export async function subscribeToPushNotifications() {
  try {
    const registration = await navigator.serviceWorker.ready;
    
    // Check permission
    let permission = await Notification.requestPermission();
    if (permission !== 'granted') {
      throw new Error('Notification permission denied');
    }

    // Get subscription
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
  if (isDevelopment) {
    // In development, just log the subscription and return mock data
    console.log('Development mode: Subscription would be sent to server', subscription);
    return { success: true, message: 'Subscription stored (development mode)' };
  }

  try {
    // Replace with your actual API endpoint in production
    const response = await fetch('/api/push/subscribe', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(subscription)
    });
    
    if (!response.ok) {
      throw new Error('Failed to store subscription on server');
    }
    
    return response.json();
  } catch (error) {
    console.error('Error sending subscription to server:', error);
    if (isDevelopment) {
      // In development, continue despite the error
      return { success: true, message: 'Development mode - ignoring API error' };
    }
    throw error;
  }
}

export async function unsubscribeFromPushNotifications() {
  const registration = await navigator.serviceWorker.ready;
  const subscription = await registration.pushManager.getSubscription();
  
  if (subscription) {
    await subscription.unsubscribe();

    if (isDevelopment) {
      // In development, just log the unsubscription
      console.log('Development mode: Unsubscription would be sent to server', subscription);
      return;
    }

    try {
      // Notify server about unsubscription in production
      await fetch('/api/push/unsubscribe', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(subscription)
      });
    } catch (error) {
      console.error('Error notifying server about unsubscription:', error);
      if (!isDevelopment) {
        throw error;
      }
    }
  }
}