import React, { useEffect, useMemo, useState } from 'react';
import { campaignMessagingService, downlinesService } from '../../services/campaignCommandService';

function flattenTree(node, output = []) {
  if (!node) {
    return output;
  }

  output.push({
    id: node.id,
    fullName: node.fullName,
    campaignRole: node.campaignRole,
    region: node.region,
    county: node.county
  });

  if (Array.isArray(node.children)) {
    node.children.forEach((child) => flattenTree(child, output));
  }

  return output;
}

const defaultForm = {
  channel: 'InApp',
  title: '',
  body: '',
  url: '',
  scheduledFor: '',
  region: '',
  county: '',
  campaignRole: ''
};

export default function AdminMessageComposer() {
  const [mode, setMode] = useState('broadcast');
  const [form, setForm] = useState(defaultForm);
  const [selectedUserIds, setSelectedUserIds] = useState([]);
  const [networkUsers, setNetworkUsers] = useState([]);
  const [loadingUsers, setLoadingUsers] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [analytics, setAnalytics] = useState(null);
  const [feedback, setFeedback] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    loadNetworkUsers();
    loadAnalytics();
  }, []);

  async function loadAnalytics() {
    try {
      const data = await campaignMessagingService.getAnalytics();
      setAnalytics(data);
    } catch (err) {
      console.error('Failed to load messaging analytics', err);
      setAnalytics(null);
    }
  }

  async function loadNetworkUsers() {
    try {
      setLoadingUsers(true);
      const tree = await downlinesService.getTree(null, 6);
      const users = flattenTree(tree);
      setNetworkUsers(users);
    } catch (err) {
      console.error('Failed to load hierarchy users', err);
      setNetworkUsers([]);
    } finally {
      setLoadingUsers(false);
    }
  }

  const selectedUsersPreview = useMemo(() => {
    if (selectedUserIds.length === 0) {
      return [];
    }

    const selectedSet = new Set(selectedUserIds);
    return networkUsers.filter((user) => selectedSet.has(user.id)).slice(0, 5);
  }, [networkUsers, selectedUserIds]);

  const onChange = (key, value) => {
    setForm((current) => ({ ...current, [key]: value }));
  };

  const toggleUserSelection = (userId) => {
    setSelectedUserIds((current) => {
      if (current.includes(userId)) {
        return current.filter((id) => id !== userId);
      }
      return [...current, userId];
    });
  };

  const resetComposer = () => {
    setForm(defaultForm);
    setSelectedUserIds([]);
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setFeedback('');
    setSubmitting(true);

    try {
      const payload = {
        channel: form.channel,
        title: form.title || null,
        body: form.body,
        url: form.url || null,
        scheduledFor: form.scheduledFor ? new Date(form.scheduledFor).toISOString() : null
      };

      let response;

      if (mode === 'broadcast') {
        response = await campaignMessagingService.broadcast(payload);
      } else {
        response = await campaignMessagingService.target({
          ...payload,
          receiverUserIds: selectedUserIds.length > 0 ? selectedUserIds : null,
          region: form.region || null,
          county: form.county || null,
          campaignRole: form.campaignRole || null
        });
      }

      setFeedback(`${response.queuedCount || 0} message(s) queued successfully.`);
      resetComposer();
      await loadAnalytics();
    } catch (err) {
      console.error('Failed to queue messages', err);
      setError(err.response?.data?.message || 'Unable to queue the message batch.');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="p-8 space-y-6 bg-stone-50 rounded-lg">
      <section className="rounded-3xl bg-gradient-to-r from-slate-900 via-cyan-900 to-sky-700 text-white p-8 shadow-lg">
        <p className="text-xs uppercase tracking-[0.22em] text-cyan-200">Operations Messaging</p>
        <h2 className="text-3xl font-bold mt-2">Compose campaign broadcasts and targeted dispatches</h2>
        <p className="mt-3 text-sm text-cyan-100 max-w-3xl">
          Use broadcast to push to the full network, or switch to target mode to scope delivery by selected users, geography, and role.
        </p>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <button
            type="button"
            onClick={() => setMode('broadcast')}
            className={`rounded-2xl border px-4 py-3 text-left ${
              mode === 'broadcast'
                ? 'border-cyan-600 bg-cyan-50 text-cyan-900'
                : 'border-stone-200 bg-white text-stone-700'
            }`}
          >
            <p className="font-semibold">Broadcast</p>
            <p className="text-sm">Queue a global message to every user in scope.</p>
          </button>

          <button
            type="button"
            onClick={() => setMode('target')}
            className={`rounded-2xl border px-4 py-3 text-left ${
              mode === 'target'
                ? 'border-cyan-600 bg-cyan-50 text-cyan-900'
                : 'border-stone-200 bg-white text-stone-700'
            }`}
          >
            <p className="font-semibold">Target</p>
            <p className="text-sm">Send to selected recipients or filtered channels only.</p>
          </button>
        </div>

        <form onSubmit={handleSubmit} className="mt-6 space-y-5">
          {error && <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
          {feedback && <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{feedback}</div>}

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-stone-700 mb-2">Channel</label>
              <select
                value={form.channel}
                onChange={(event) => onChange('channel', event.target.value)}
                className="w-full rounded-xl border border-stone-300 px-3 py-2"
              >
                <option value="InApp">In-App</option>
                <option value="Push">Push</option>
                <option value="WhatsApp">WhatsApp</option>
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-stone-700 mb-2">Title (optional)</label>
              <input
                value={form.title}
                onChange={(event) => onChange('title', event.target.value)}
                className="w-full rounded-xl border border-stone-300 px-3 py-2"
                placeholder="Campaign update"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Message body</label>
            <textarea
              value={form.body}
              onChange={(event) => onChange('body', event.target.value)}
              required
              rows={4}
              className="w-full rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Share mobilization instructions, turnout reminders, or compliance updates."
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Action URL (optional)</label>
            <input
              value={form.url}
              onChange={(event) => onChange('url', event.target.value)}
              className="w-full rounded-xl border border-stone-300 px-3 py-2"
              placeholder="https://example.org/next-action"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-stone-700 mb-2">Send now or schedule</label>
            <input
              type="datetime-local"
              value={form.scheduledFor}
              onChange={(event) => onChange('scheduledFor', event.target.value)}
              className="w-full rounded-xl border border-stone-300 px-3 py-2"
            />
            <p className="mt-1 text-xs text-stone-500">Leave blank to send immediately via queue worker.</p>
          </div>

          {mode === 'target' && (
            <div className="space-y-4 rounded-2xl border border-stone-200 bg-stone-50 p-4">
              <h3 className="font-semibold text-stone-900">Target filters</h3>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                <input
                  value={form.region}
                  onChange={(event) => onChange('region', event.target.value)}
                  className="rounded-xl border border-stone-300 px-3 py-2"
                  placeholder="Region"
                />
                <input
                  value={form.county}
                  onChange={(event) => onChange('county', event.target.value)}
                  className="rounded-xl border border-stone-300 px-3 py-2"
                  placeholder="County"
                />
                <input
                  value={form.campaignRole}
                  onChange={(event) => onChange('campaignRole', event.target.value)}
                  className="rounded-xl border border-stone-300 px-3 py-2"
                  placeholder="Campaign role"
                />
              </div>

              <div>
                <div className="flex items-center justify-between mb-2">
                  <p className="text-sm font-medium text-stone-700">Recipient picker</p>
                  <button
                    type="button"
                    onClick={() => setSelectedUserIds([])}
                    className="text-xs text-stone-500 hover:text-stone-700"
                  >
                    Clear selection
                  </button>
                </div>

                {loadingUsers ? (
                  <p className="text-sm text-stone-500">Loading hierarchy users...</p>
                ) : networkUsers.length === 0 ? (
                  <p className="text-sm text-stone-500">No hierarchy users available. Use geographic filters above.</p>
                ) : (
                  <div className="max-h-56 overflow-y-auto space-y-2 pr-1">
                    {networkUsers.map((user) => (
                      <label key={user.id} className="flex items-center gap-3 rounded-xl border border-stone-200 bg-white p-3 text-sm">
                        <input
                          type="checkbox"
                          checked={selectedUserIds.includes(user.id)}
                          onChange={() => toggleUserSelection(user.id)}
                        />
                        <div>
                          <p className="font-medium text-stone-900">{user.fullName}</p>
                          <p className="text-xs text-stone-500">
                            {user.campaignRole || 'Role not set'}
                            {user.region ? ` • ${user.region}` : ''}
                            {user.county ? ` / ${user.county}` : ''}
                          </p>
                        </div>
                      </label>
                    ))}
                  </div>
                )}

                {selectedUserIds.length > 0 && (
                  <div className="mt-3 rounded-xl bg-cyan-50 px-3 py-2 text-sm text-cyan-800">
                    {selectedUserIds.length} recipient(s) selected
                    {selectedUsersPreview.length > 0 && (
                      <span>
                        {`: ${selectedUsersPreview.map((user) => user.fullName).join(', ')}`}
                        {selectedUserIds.length > selectedUsersPreview.length ? '...' : ''}
                      </span>
                    )}
                  </div>
                )}
              </div>
            </div>
          )}

          <div className="flex items-center justify-end gap-3">
            <button
              type="button"
              onClick={resetComposer}
              className="rounded-xl border border-stone-300 px-4 py-2 text-sm font-medium text-stone-700"
            >
              Reset
            </button>
            <button
              type="submit"
              disabled={submitting}
              className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700 disabled:cursor-not-allowed disabled:bg-stone-400"
            >
              {submitting ? 'Queueing...' : mode === 'broadcast' ? 'Queue Broadcast' : 'Queue Target Message'}
            </button>
          </div>
        </form>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <h3 className="text-xl font-semibold text-stone-900">Delivery intelligence dashboard</h3>
          <button type="button" onClick={loadAnalytics} className="rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100">Refresh</button>
        </div>

        {!analytics ? (
          <p className="mt-3 text-sm text-stone-500">Analytics not available yet.</p>
        ) : (
          <div className="mt-4 space-y-4">
            <div className="grid grid-cols-2 md:grid-cols-5 gap-3">
              {(analytics.totalsByStatus || []).map((item) => (
                <div key={item.status} className="rounded-lg border border-stone-200 bg-stone-50 p-3">
                  <p className="text-xs uppercase text-stone-500">{item.status}</p>
                  <p className="text-lg font-semibold text-stone-900">{item.count}</p>
                </div>
              ))}
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
              {(analytics.totalsByChannel || []).map((row) => (
                <div key={row.channel} className="rounded-lg border border-stone-200 bg-stone-50 p-3 text-sm text-stone-700">
                  <p className="font-semibold text-stone-900">{row.channel}</p>
                  <p>Sent: {row.sent}</p>
                  <p>Delivered: {row.delivered}</p>
                  <p>Read: {row.read}</p>
                  <p>Failed: {row.failed}</p>
                </div>
              ))}
            </div>
          </div>
        )}
      </section>
    </div>
  );
}
