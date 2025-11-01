// Notification preferences management
const PREFS_KEY = 'notification_preferences';

const DEFAULT_CATEGORIES = {
  events: {
    id: 'events',
    label: 'Campaign Events',
    description: 'Updates about rallies, townhalls, and community meetings',
    enabled: true
  },
  news: {
    id: 'news',
    label: 'News Updates',
    description: 'Latest campaign news and policy announcements',
    enabled: true
  },
  volunteer: {
    id: 'volunteer',
    label: 'Volunteer Opportunities',
    description: 'Ways to get involved and help the campaign',
    enabled: true
  },
  local: {
    id: 'local',
    label: 'Local Updates',
    description: 'News and events in your county',
    enabled: true
  }
};

class NotificationPreferences {
  constructor() {
    this.prefs = this.loadPreferences();
  }

  loadPreferences() {
    try {
      const stored = localStorage.getItem(PREFS_KEY);
      return stored ? JSON.parse(stored) : this.getDefaultPreferences();
    } catch (error) {
      console.error('Error loading notification preferences:', error);
      return this.getDefaultPreferences();
    }
  }

  getDefaultPreferences() {
    return {
      categories: { ...DEFAULT_CATEGORIES },
      schedule: {
        quiet: false,
        quietHoursStart: '22:00',
        quietHoursEnd: '08:00'
      },
      frequency: 'normal', // 'minimal', 'normal', 'all'
      lastUpdated: new Date().toISOString()
    };
  }

  savePreferences() {
    try {
      this.prefs.lastUpdated = new Date().toISOString();
      localStorage.setItem(PREFS_KEY, JSON.stringify(this.prefs));
    } catch (error) {
      console.error('Error saving notification preferences:', error);
    }
  }

  updateCategory(categoryId, enabled) {
    if (this.prefs.categories[categoryId]) {
      this.prefs.categories[categoryId].enabled = enabled;
      this.savePreferences();
    }
  }

  updateQuietHours(enabled, start = null, end = null) {
    this.prefs.schedule.quiet = enabled;
    if (start) this.prefs.schedule.quietHoursStart = start;
    if (end) this.prefs.schedule.quietHoursEnd = end;
    this.savePreferences();
  }

  updateFrequency(frequency) {
    if (['minimal', 'normal', 'all'].includes(frequency)) {
      this.prefs.frequency = frequency;
      this.savePreferences();
    }
  }

  isQuietTime() {
    if (!this.prefs.schedule.quiet) return false;

    const now = new Date();
    const current = now.getHours() * 60 + now.getMinutes();
    
    const [startHours, startMinutes] = this.prefs.schedule.quietHoursStart.split(':').map(Number);
    const [endHours, endMinutes] = this.prefs.schedule.quietHoursEnd.split(':').map(Number);
    
    const start = startHours * 60 + startMinutes;
    const end = endHours * 60 + endMinutes;
    
    if (start <= end) {
      return current >= start && current <= end;
    } else {
      // Handles cases where quiet hours span midnight
      return current >= start || current <= end;
    }
  }

  shouldShowNotification(category) {
    // Check if notifications should be shown based on preferences
    if (!category || !this.prefs.categories[category]) return true;
    if (!this.prefs.categories[category].enabled) return false;
    if (this.isQuietTime()) return false;
    
    return true;
  }

  getCategories() {
    return Object.values(this.prefs.categories);
  }

  getPreferences() {
    return { ...this.prefs };
  }
}

export const notificationPrefs = new NotificationPreferences();
export default notificationPrefs;