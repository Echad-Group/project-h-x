import React from 'react';

export default function AnalyticsVisualizations({ stats }) {
  const calculatePercentage = (value, total) => {
    return total ? Math.round((value / total) * 100) : 0;
  };

  const getSessionTrend = () => {
    const sessions = stats.sessions || [];
    const today = new Date();
    const last7Days = new Array(7).fill(0);
    
    sessions.forEach(session => {
      const date = new Date(session.startTime);
      const dayDiff = Math.floor((today - date) / (1000 * 60 * 60 * 24));
      if (dayDiff < 7) {
        last7Days[6 - dayDiff]++;
      }
    });

    return last7Days;
  };

  const sessionTrend = getSessionTrend();
  const maxSessions = Math.max(...sessionTrend, 1);

  return (
    <div className="space-y-6">
      {/* Usage Overview */}
      <div className="grid grid-cols-2 gap-4">
        <div className="p-4 bg-gradient-to-br from-blue-50 to-blue-100 rounded-lg">
          <div className="text-sm text-blue-600">Total Sessions</div>
          <div className="text-2xl font-bold">{stats.totalSessions}</div>
          <div className="text-xs text-blue-500">
            Avg {Math.round(stats.averageSessionLength / 1000 / 60)}min per session
          </div>
        </div>
        
        <div className="p-4 bg-gradient-to-br from-green-50 to-green-100 rounded-lg">
          <div className="text-sm text-green-600">Notifications</div>
          <div className="text-2xl font-bold">{stats.notifications.received}</div>
          <div className="text-xs text-green-500">
            {calculatePercentage(stats.notifications.clicked, stats.notifications.received)}% click rate
          </div>
        </div>
      </div>

      {/* Session Trend Chart */}
      <div className="space-y-2">
        <div className="text-sm font-medium">Last 7 Days Activity</div>
        <div className="h-24 flex items-end gap-2">
          {sessionTrend.map((count, index) => (
            <div key={index} className="flex-1 flex flex-col items-center">
              <div 
                className="w-full bg-blue-500 rounded-t"
                style={{ 
                  height: `${(count / maxSessions) * 100}%`,
                  minHeight: count > 0 ? '4px' : '0'
                }}
              />
              <div className="text-xs text-gray-500 mt-1">
                {new Date(Date.now() - (6 - index) * 24 * 60 * 60 * 1000)
                  .toLocaleDateString(undefined, { weekday: 'short' })}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Notification Categories */}
      {stats.notifications.categories && (
        <div className="space-y-2">
          <div className="text-sm font-medium">Notification Categories</div>
          <div className="space-y-2">
            {Object.entries(stats.notifications.categories).map(([category, count]) => (
              <div key={category} className="flex items-center gap-2">
                <div className="text-sm">{category}</div>
                <div className="flex-1 h-2 bg-gray-100 rounded-full overflow-hidden">
                  <div
                    className="h-full bg-green-500"
                    style={{
                      width: `${calculatePercentage(count, stats.notifications.received)}%`
                    }}
                  />
                </div>
                <div className="text-xs text-gray-500">{count}</div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Offline Usage */}
      {stats.offline.usage > 0 && (
        <div className="p-4 bg-gradient-to-br from-purple-50 to-purple-100 rounded-lg">
          <div className="text-sm text-purple-600">Offline Usage</div>
          <div className="text-2xl font-bold">{stats.offline.usage}</div>
          <div className="text-xs text-purple-500">
            Last used: {new Date(stats.offline.lastUsed).toLocaleDateString()}
          </div>
        </div>
      )}

      {/* Installation Age */}
      {stats.installDate && (
        <div className="text-sm text-gray-500">
          Installed {Math.floor((new Date() - new Date(stats.installDate)) / (1000 * 60 * 60 * 24))} days ago
        </div>
      )}
    </div>
  );
}
