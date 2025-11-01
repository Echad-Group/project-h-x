// PWA usage analytics
const ANALYTICS_KEY = 'pwa_analytics';

class PWAAnalytics {
  constructor() {
    this.data = this.loadData();
    this.startSession();
  }

  loadData() {
    try {
      const stored = localStorage.getItem(ANALYTICS_KEY);
      return stored ? JSON.parse(stored) : this.getInitialData();
    } catch (error) {
      console.error('Error loading analytics:', error);
      return this.getInitialData();
    }
  }

  getInitialData() {
    return {
      installDate: null,
      sessions: [],
      events: [],
      notifications: {
        granted: false,
        subscribed: false,
        received: 0,
        clicked: 0
      },
      offline: {
        usage: 0,
        lastUsed: null
      }
    };
  }

  saveData() {
    try {
      localStorage.setItem(ANALYTICS_KEY, JSON.stringify(this.data));
    } catch (error) {
      console.error('Error saving analytics:', error);
    }
  }

  startSession() {
    const session = {
      startTime: new Date().toISOString(),
      endTime: null,
      events: [],
      isStandalone: window.matchMedia('(display-mode: standalone)').matches
    };
    this.data.sessions.push(session);
    this.saveData();

    // Update session end time on page unload
    window.addEventListener('unload', () => {
      session.endTime = new Date().toISOString();
      this.saveData();
    });
  }

  trackEvent(category, action, label = null) {
    const event = {
      timestamp: new Date().toISOString(),
      category,
      action,
      label,
      isStandalone: window.matchMedia('(display-mode: standalone)').matches,
      online: navigator.onLine
    };
    
    this.data.events.push(event);
    this.data.sessions[this.data.sessions.length - 1].events.push(event);
    this.saveData();

    // If you have a real analytics service, send the event there too
    if (window.gtag) {
      window.gtag('event', action, {
        event_category: category,
        event_label: label
      });
    }
  }

  trackInstall() {
    if (!this.data.installDate) {
      this.data.installDate = new Date().toISOString();
      this.trackEvent('pwa', 'install');
      this.saveData();
    }
  }

  trackNotification(action, category = null) {
    switch (action) {
      case 'grant':
        this.data.notifications.granted = true;
        break;
      case 'subscribe':
        this.data.notifications.subscribed = true;
        break;
      case 'receive':
        this.data.notifications.received++;
        break;
      case 'click':
        this.data.notifications.clicked++;
        break;
    }
    this.trackEvent('notification', action, category);
    this.saveData();
  }

  trackOfflineUsage() {
    this.data.offline.usage++;
    this.data.offline.lastUsed = new Date().toISOString();
    this.trackEvent('offline', 'use');
    this.saveData();
  }

  getStats() {
    const now = new Date();
    const sessions = this.data.sessions;
    
    return {
      totalSessions: sessions.length,
      installDate: this.data.installDate ? new Date(this.data.installDate) : null,
      lastSession: sessions.length > 0 ? new Date(sessions[sessions.length - 1].startTime) : null,
      averageSessionLength: this.calculateAverageSessionLength(),
      notifications: { ...this.data.notifications },
      offline: { ...this.data.offline },
      events: this.data.events.length
    };
  }

  calculateAverageSessionLength() {
    const completedSessions = this.data.sessions.filter(s => s.endTime);
    if (completedSessions.length === 0) return 0;

    const totalLength = completedSessions.reduce((acc, session) => {
      const start = new Date(session.startTime);
      const end = new Date(session.endTime);
      return acc + (end - start);
    }, 0);

    return totalLength / completedSessions.length;
  }
}

export const pwaAnalytics = new PWAAnalytics();
export default pwaAnalytics;