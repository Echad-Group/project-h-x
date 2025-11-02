import React, { useState } from 'react';
import notificationPrefs from '../services/notificationPreferences';
import pwaAnalytics from '../services/pwaAnalytics';

export default function NotificationTester() {
  const [notification, setNotification] = useState({
    title: 'Test Notification',
    body: 'This is a test notification',
    category: 'events',
    image: '',
    actions: []
  });

  const categories = notificationPrefs.getCategories();

  const testNotification = async () => {
    if (!('Notification' in window)) {
      alert('Notifications not supported');
      return;
    }

    if (Notification.permission !== 'granted') {
      alert('Please enable notifications first');
      return;
    }

    if (!notificationPrefs.shouldShowNotification(notification.category)) {
      alert('This notification category is disabled or it is quiet hours');
      return;
    }

    try {
      const registration = await navigator.serviceWorker.ready;

      const options = {
        body: notification.body,
        icon: '/src/assets/icons/icon-192.svg',
        badge: '/src/assets/icons/icon-96.svg',
        image: notification.image || undefined,
        vibrate: [100, 50, 100],
        tag: 'test-notification',
        actions: notification.actions.length > 0 ? notification.actions : undefined,
        data: {
          category: notification.category,
          url: '/',
          timestamp: new Date().toISOString()
        }
      };

      // In development show the notification directly via service worker registration
      if (process.env.NODE_ENV === 'development') {
        await registration.showNotification(notification.title, options);
      } else {
        // In production you would POST to your server which would trigger the push
        await fetch('/api/send-notification', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ notification })
        });
      }

      pwaAnalytics.trackNotification('test', notification.category);
    } catch (error) {
      console.error('Error showing test notification:', error);
      alert('Error showing notification: ' + (error.message || error));
    }
  };

  const addAction = () => {
    const action = {
      action: 'action-' + (notification.actions.length + 1),
      title: 'Action ' + (notification.actions.length + 1)
    };
    setNotification(prev => ({ ...prev, actions: [...prev.actions, action] }));
  };

  const removeAction = (index) => {
    setNotification(prev => ({ ...prev, actions: prev.actions.filter((_, i) => i !== index) }));
  };

  return (
    <div className="space-y-4 p-4 bg-white rounded-lg shadow-md">
      <h3 className="font-bold text-lg">Notification Tester</h3>

      <div className="space-y-3">
        <div>
          <label className="block text-sm font-medium text-gray-700">Title</label>
          <input
            type="text"
            value={notification.title}
            onChange={(e) => setNotification(prev => ({ ...prev, title: e.target.value }))}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Body</label>
          <textarea
            value={notification.body}
            onChange={(e) => setNotification(prev => ({ ...prev, body: e.target.value }))}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            rows={3}
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Category</label>
          <select
            value={notification.category}
            onChange={(e) => setNotification(prev => ({ ...prev, category: e.target.value }))}
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
          >
            {categories.map(cat => (
              <option key={cat.id} value={cat.id}>{cat.label}</option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Image URL (optional)</label>
          <input
            type="url"
            value={notification.image || ''}
            onChange={(e) => setNotification(prev => ({ ...prev, image: e.target.value || '' }))}
            placeholder="https://example.com/image.jpg"
            className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Actions</label>
          <div className="mt-2 space-y-2">
            {notification.actions.map((action, index) => (
              <div key={index} className="flex items-center gap-2">
                <input
                  type="text"
                  value={action.title}
                  onChange={(e) => {
                    const newActions = [...notification.actions];
                    newActions[index].title = e.target.value;
                    setNotification(prev => ({ ...prev, actions: newActions }));
                  }}
                  className="flex-1 rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
                  placeholder="Action title"
                />
                <button
                  onClick={() => removeAction(index)}
                  type="button"
                  className="p-2 text-red-500 hover:text-red-700"
                >
                  ×
                </button>
              </div>
            ))}
            {notification.actions.length < 2 && (
              <button
                onClick={addAction}
                type="button"
                className="text-sm text-blue-500 hover:text-blue-700"
              >
                + Add Action
              </button>
            )}
          </div>
        </div>
      </div>

      <div className="flex justify-end">
        <button
          onClick={testNotification}
          className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
        >
          Send Test Notification
        </button>
      </div>
    </div>
  );
}
