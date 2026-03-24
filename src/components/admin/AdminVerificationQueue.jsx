import React, { useEffect, useState } from 'react';
import { verificationService } from '../../services/campaignCommandService';

export default function AdminVerificationQueue() {
  const [items, setItems] = useState([]);
  const [statusFilter, setStatusFilter] = useState('Pending');
  const [selected, setSelected] = useState(null);
  const [reviewerNotes, setReviewerNotes] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadQueue(statusFilter);
  }, [statusFilter]);

  async function loadQueue(status) {
    try {
      setLoading(true);
      setError('');
      const queue = await verificationService.getQueue(status);
      setItems(Array.isArray(queue) ? queue : []);
      setSelected((Array.isArray(queue) && queue.length > 0) ? queue[0] : null);
    } catch (err) {
      console.error('Failed to load verification queue', err);
      setError(err.response?.data?.message || 'Unable to load verification queue.');
    } finally {
      setLoading(false);
    }
  }

  async function selectItem(userId) {
    try {
      const item = await verificationService.getQueueItem(userId);
      setSelected(item);
      setReviewerNotes('');
    } catch (err) {
      console.error('Failed to load queue item', err);
      setError(err.response?.data?.message || 'Unable to load verification item.');
    }
  }

  async function decide(decision) {
    if (!selected) {
      return;
    }

    try {
      await verificationService.decide(selected.userId, {
        decision,
        reviewerNotes: reviewerNotes || null
      });
      await loadQueue(statusFilter);
    } catch (err) {
      console.error('Failed to submit verification decision', err);
      setError(err.response?.data?.message || 'Unable to submit review decision.');
    }
  }

  if (loading) {
    return <div className="p-8 text-sm text-gray-600">Loading verification review queue...</div>;
  }

  return (
    <div className="p-8 space-y-6 bg-gray-50 rounded-lg">
      <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between gap-3">
          <div>
            <h2 className="text-2xl font-bold text-gray-900">Verification Review Queue</h2>
            <p className="mt-1 text-sm text-gray-600">Approve or reject pending identity packs with reviewer notes and timeline tracking.</p>
          </div>
          <select
            value={statusFilter}
            onChange={(event) => setStatusFilter(event.target.value)}
            className="rounded-lg border border-gray-300 px-3 py-2 text-sm"
          >
            <option value="Pending">Pending</option>
            <option value="Verified">Verified</option>
            <option value="Rejected">Rejected</option>
          </select>
        </div>
        {error && (
          <div className="mt-4 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700 flex items-center justify-between">
            <span>{error}</span>
            <button onClick={() => loadQueue(statusFilter)} className="ml-4 text-sm underline hover:no-underline shrink-0">Retry</button>
          </div>
        )}
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1.3fr] gap-6">
        <div className="rounded-2xl border border-gray-200 bg-white p-4 shadow-sm">
          <h3 className="text-lg font-semibold text-gray-900">Queue items</h3>
          <div className="mt-3 space-y-3 max-h-[34rem] overflow-y-auto pr-1">
            {items.length === 0 ? (
              <p className="text-sm text-gray-500">No users in this queue.</p>
            ) : (
              items.map((item) => (
                <button
                  key={item.userId}
                  type="button"
                  onClick={() => selectItem(item.userId)}
                  className={`w-full text-left rounded-lg border p-3 ${selected?.userId === item.userId ? 'border-sky-400 bg-sky-50' : 'border-gray-200 bg-gray-50 hover:bg-gray-100'}`}
                >
                  <p className="font-semibold text-gray-900">{item.fullName || item.email}</p>
                  <p className="text-xs text-gray-500 mt-1">{item.email}</p>
                  <p className="text-xs text-gray-500 mt-1">{item.region || '-'} / {item.county || '-'}</p>
                </button>
              ))
            )}
          </div>
        </div>

        <div className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm">
          {!selected ? (
            <p className="text-sm text-gray-500">Select a queue item to review documents.</p>
          ) : (
            <div className="space-y-4">
              <div>
                <h3 className="text-xl font-semibold text-gray-900">{selected.fullName || selected.email}</h3>
                <p className="text-sm text-gray-600">Role: {selected.campaignRole || 'User'}</p>
                <p className="text-sm text-gray-600">Created: {new Date(selected.createdAt).toLocaleString()}</p>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-3 text-sm">
                <a href={selected.nidaDocumentUrl} target="_blank" rel="noreferrer" className="rounded-lg border border-gray-200 bg-gray-50 px-3 py-2 text-sky-700 hover:bg-sky-50">Open NIDA Document</a>
                <a href={selected.voterCardDocumentUrl} target="_blank" rel="noreferrer" className="rounded-lg border border-gray-200 bg-gray-50 px-3 py-2 text-sky-700 hover:bg-sky-50">Open Voter Card</a>
                <a href={selected.selfieDocumentUrl} target="_blank" rel="noreferrer" className="rounded-lg border border-gray-200 bg-gray-50 px-3 py-2 text-sky-700 hover:bg-sky-50">Open Selfie</a>
              </div>

              <textarea
                value={reviewerNotes}
                onChange={(event) => setReviewerNotes(event.target.value)}
                rows={4}
                className="w-full rounded-lg border border-gray-300 px-3 py-2"
                placeholder="Reviewer notes (reasoning, evidence, follow-up action)"
              />

              <div className="flex flex-wrap gap-2">
                <button type="button" onClick={() => decide('Approved')} className="rounded-lg border border-emerald-300 bg-emerald-50 px-4 py-2 text-sm font-semibold text-emerald-700 hover:bg-emerald-100">Approve</button>
                <button type="button" onClick={() => decide('Rejected')} className="rounded-lg border border-rose-300 bg-rose-50 px-4 py-2 text-sm font-semibold text-rose-700 hover:bg-rose-100">Reject</button>
              </div>

              <div className="rounded-lg border border-gray-200 bg-gray-50 p-4">
                <h4 className="text-sm font-semibold text-gray-900">Review timeline</h4>
                <div className="mt-2 space-y-2">
                  {(selected.timeline || []).length === 0 ? (
                    <p className="text-xs text-gray-500">No review events yet.</p>
                  ) : (
                    selected.timeline.map((event, index) => (
                      <div key={`${event.timestamp}-${index}`} className="rounded border border-gray-200 bg-white p-2">
                        <p className="text-xs font-semibold text-gray-800">{event.action}</p>
                        <p className="text-xs text-gray-500">{event.reviewerName || 'Reviewer'} • {new Date(event.timestamp).toLocaleString()}</p>
                        {event.notes && <p className="text-xs text-gray-600 mt-1">{event.notes}</p>}
                      </div>
                    ))
                  )}
                </div>
              </div>
            </div>
          )}
        </div>
      </section>
    </div>
  );
}
