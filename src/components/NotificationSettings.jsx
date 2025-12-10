import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { 
  subscribeToPushNotifications, 
  unsubscribeFromPushNotifications, 
  checkSubscriptionStatus,
  sendTestNotification 
} from '../services/pushNotification';
import notificationPrefs from '../services/notificationPreferences';

export default function NotificationSettings() {
  const { t } = useTranslation();
  const [isSubscribed, setIsSubscribed] = useState(false);
  const [isSupported, setIsSupported] = useState(true);
  const [loading, setLoading] = useState(true);
  const [categories, setCategories] = useState({});

  useEffect(() => {
    initializeSettings();
  }, []);

  async function initializeSettings() {
    try {
      const status = await checkSubscriptionStatus();
      setIsSubscribed(status.isSubscribed);
      setIsSupported(status.supported);
      
      const prefs = notificationPrefs.getCategories();
      setCategories(prefs);
    } catch (error) {
      console.error('Error initializing notification settings:', error);
    } finally {
      setLoading(false);
    }
  }

  async function handleSubscriptionToggle() {
    try {
      setLoading(true);
      if (isSubscribed) {
        await unsubscribeFromPushNotifications();
        setIsSubscribed(false);
      } else {
        await subscribeToPushNotifications();
        setIsSubscribed(true);
      }
    } catch (error) {
      console.error('Error toggling subscription:', error);
      alert(error.message || 'Failed to update notification settings');
    } finally {
      setLoading(false);
    }
  }

  function handleCategoryToggle(categoryId) {
    notificationPrefs.setCategory(categoryId, !categories[categoryId].enabled);
    setCategories(notificationPrefs.getCategories());
  }

  async function handleTestNotification() {
    try {
      await sendTestNotification(
        'Test Notification',
        'This is a test notification from New Kenya'
      );
      alert('Test notification sent! Check your notifications.');
    } catch (error) {
      console.error('Error sending test notification:', error);
      alert('Failed to send test notification: ' + error.message);
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="animate-spin text-3xl">↻</div>
      </div>
    );
  }

  if (!isSupported) {
    return (
      <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 text-center">
        <p className="text-yellow-800">
          Push notifications are not supported in your browser.
        </p>
      </div>
    );
  }

  return (
    <div className="space-y-6 max-w-2xl mx-auto">
      {/* Main Toggle */}
      <div className="bg-white rounded-lg card-shadow p-6">
        <div className="flex items-center justify-between">
          <div>
            <h2 className="text-xl font-bold mb-2">Push Notifications</h2>
            <p className="text-gray-600 text-sm">
              {isSubscribed 
                ? 'You are receiving push notifications' 
                : 'Enable push notifications to stay updated'}
            </p>
          </div>
          <button
            onClick={handleSubscriptionToggle}
            disabled={loading}
            className={`px-6 py-3 rounded-lg font-medium transition-all ${
              isSubscribed
                ? 'bg-gray-200 text-gray-700 hover:bg-gray-300'
                : 'bg-[var(--kenya-green)] text-white hover:opacity-90'
            } disabled:opacity-50`}
          >
            {isSubscribed ? 'Disable' : 'Enable'}
          </button>
        </div>
      </div>

      {/* Category Settings */}
      {isSubscribed && (
        <div className="bg-white rounded-lg card-shadow p-6">
          <h3 className="text-lg font-bold mb-4">Notification Categories</h3>
          <p className="text-gray-600 text-sm mb-4">
            Choose which types of notifications you want to receive
          </p>
          
          <div className="space-y-3">
            {Object.entries(categories).map(([id, category]) => (
              <label
                key={id}
                className="flex items-center justify-between p-3 border rounded-lg hover:bg-gray-50 cursor-pointer"
              >
                <div className="flex items-center gap-3">
                  <span className="text-2xl">{category.icon}</span>
                  <div>
                    <div className="font-medium">{category.name}</div>
                    <div className="text-sm text-gray-500">{category.description}</div>
                  </div>
                </div>
                <input
                  type="checkbox"
                  checked={category.enabled}
                  onChange={() => handleCategoryToggle(id)}
                  className="w-5 h-5 rounded"
                />
              </label>
            ))}
          </div>
        </div>
      )}

      {/* Test Notification */}
      {isSubscribed && (
        <div className="bg-white rounded-lg card-shadow p-6">
          <h3 className="text-lg font-bold mb-2">Test Notifications</h3>
          <p className="text-gray-600 text-sm mb-4">
            Send a test notification to verify everything is working
          </p>
          <button
            onClick={handleTestNotification}
            className="fluent-btn fluent-btn-primary"
          >
            Send Test Notification
          </button>
        </div>
      )}

      {/* Info */}
      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
        <h4 className="font-semibold text-blue-900 mb-2">About Notifications</h4>
        <ul className="text-sm text-blue-800 space-y-1">
          <li>• Notifications work even when the app is closed</li>
          <li>• You can manage these settings anytime</li>
          <li>• Your browser may ask for permission to show notifications</li>
          <li>• Quiet hours: 10 PM - 8 AM (local time)</li>
        </ul>
      </div>
    </div>
  );
}
