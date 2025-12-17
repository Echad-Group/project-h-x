import React, { useState } from 'react';

export default function AdminEngagement() {
  const [dateRange, setDateRange] = useState('30days');

  const engagementData = {
    likes: 1250,
    comments: 340,
    shares: 890,
    upvotes: 670,
    downvotes: 45,
    totalPosts: 48,
    avgEngagement: 42.5
  };

  const topPosts = [
    {
      id: 1,
      title: 'Town Hall Meeting Recap',
      type: 'news',
      likes: 234,
      comments: 56,
      shares: 89,
      date: '2025-12-15'
    },
    {
      id: 2,
      title: 'Youth Employment Initiative Launch',
      type: 'event',
      likes: 189,
      comments: 42,
      shares: 67,
      date: '2025-12-14'
    },
    {
      id: 3,
      title: 'Healthcare Reform Proposal',
      type: 'policy',
      likes: 156,
      comments: 38,
      shares: 54,
      date: '2025-12-13'
    }
  ];

  const engagementTrend = [
    { date: 'Dec 1', value: 420 },
    { date: 'Dec 5', value: 580 },
    { date: 'Dec 10', value: 750 },
    { date: 'Dec 15', value: 920 },
  ];

  return (
    <div className="p-8">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Engagement Analytics</h2>
          <p className="text-gray-600">Track comments, likes, dislikes, and upvotes</p>
        </div>
        <select
          className="fluent-input"
          value={dateRange}
          onChange={e => setDateRange(e.target.value)}
        >
          <option value="7days">Last 7 Days</option>
          <option value="30days">Last 30 Days</option>
          <option value="90days">Last 90 Days</option>
          <option value="all">All Time</option>
        </select>
      </div>

      {/* Engagement Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        <div className="bg-white border rounded-lg p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-3xl">👍</span>
            <span className="text-green-600 text-sm font-medium">+12%</span>
          </div>
          <p className="text-gray-600 text-sm">Total Likes</p>
          <p className="text-2xl font-bold text-gray-900">{engagementData.likes.toLocaleString()}</p>
        </div>

        <div className="bg-white border rounded-lg p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-3xl">💬</span>
            <span className="text-green-600 text-sm font-medium">+8%</span>
          </div>
          <p className="text-gray-600 text-sm">Comments</p>
          <p className="text-2xl font-bold text-gray-900">{engagementData.comments.toLocaleString()}</p>
        </div>

        <div className="bg-white border rounded-lg p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-3xl">⬆️</span>
            <span className="text-green-600 text-sm font-medium">+15%</span>
          </div>
          <p className="text-gray-600 text-sm">Upvotes</p>
          <p className="text-2xl font-bold text-gray-900">{engagementData.upvotes.toLocaleString()}</p>
        </div>

        <div className="bg-white border rounded-lg p-6">
          <div className="flex items-center justify-between mb-2">
            <span className="text-3xl">⬇️</span>
            <span className="text-red-600 text-sm font-medium">-3%</span>
          </div>
          <p className="text-gray-600 text-sm">Downvotes</p>
          <p className="text-2xl font-bold text-gray-900">{engagementData.downvotes.toLocaleString()}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Engagement Trend Chart */}
        <div className="lg:col-span-2 bg-white border rounded-lg p-6">
          <h3 className="text-lg font-bold text-gray-900 mb-4">Engagement Trend</h3>
          <div className="h-64 flex items-end justify-around gap-4">
            {engagementTrend.map((point, idx) => (
              <div key={idx} className="flex-1 flex flex-col items-center">
                <div className="w-full bg-[var(--kenya-green)] rounded-t" style={{ height: `${(point.value / 1000) * 100}%` }}></div>
                <p className="text-xs text-gray-600 mt-2">{point.date}</p>
                <p className="text-sm font-medium">{point.value}</p>
              </div>
            ))}
          </div>
        </div>

        {/* Engagement Breakdown */}
        <div className="bg-white border rounded-lg p-6">
          <h3 className="text-lg font-bold text-gray-900 mb-4">Engagement Breakdown</h3>
          <div className="space-y-4">
            <div>
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">👍 Likes</span>
                <span className="font-medium">42%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-blue-500 h-2 rounded-full" style={{ width: '42%' }}></div>
              </div>
            </div>
            <div>
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">💬 Comments</span>
                <span className="font-medium">28%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-green-500 h-2 rounded-full" style={{ width: '28%' }}></div>
              </div>
            </div>
            <div>
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">🔄 Shares</span>
                <span className="font-medium">22%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-purple-500 h-2 rounded-full" style={{ width: '22%' }}></div>
              </div>
            </div>
            <div>
              <div className="flex justify-between text-sm mb-1">
                <span className="text-gray-600">⬆️ Upvotes</span>
                <span className="font-medium">8%</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-yellow-500 h-2 rounded-full" style={{ width: '8%' }}></div>
              </div>
            </div>
          </div>

          <div className="mt-6 pt-6 border-t">
            <p className="text-sm text-gray-600">Avg. Engagement Rate</p>
            <p className="text-3xl font-bold text-[var(--kenya-green)]">{engagementData.avgEngagement}%</p>
            <p className="text-xs text-gray-500 mt-1">Across all content</p>
          </div>
        </div>
      </div>

      {/* Top Performing Content */}
      <div className="bg-white border rounded-lg p-6 mt-6">
        <h3 className="text-lg font-bold text-gray-900 mb-4">Top Performing Content</h3>
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Title</th>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Type</th>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Likes</th>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Comments</th>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Shares</th>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Total</th>
                <th className="px-4 py-3 text-left text-sm font-medium text-gray-700">Date</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {topPosts.map(post => {
                const total = post.likes + post.comments + post.shares;
                return (
                  <tr key={post.id} className="hover:bg-gray-50">
                    <td className="px-4 py-3 font-medium text-gray-900">{post.title}</td>
                    <td className="px-4 py-3">
                      <span className="px-2 py-1 bg-blue-100 text-blue-700 text-xs rounded capitalize">
                        {post.type}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-gray-600">👍 {post.likes}</td>
                    <td className="px-4 py-3 text-gray-600">💬 {post.comments}</td>
                    <td className="px-4 py-3 text-gray-600">🔄 {post.shares}</td>
                    <td className="px-4 py-3 font-medium text-gray-900">{total}</td>
                    <td className="px-4 py-3 text-sm text-gray-600">
                      {new Date(post.date).toLocaleDateString()}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
