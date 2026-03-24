import React, { useEffect, useMemo, useState } from 'react';
import { downlinesService } from '../../services/campaignCommandService';

const addTemplate = {
  leaderUserId: '',
  downlineUserId: ''
};

const reassignTemplate = {
  currentLeaderUserId: '',
  newLeaderUserId: '',
  downlineUserId: ''
};

const removeTemplate = {
  leaderUserId: '',
  downlineUserId: ''
};

function TreeNode({ node }) {
  return (
    <div className="ml-4 border-l border-stone-300 pl-3">
      <div className="rounded-lg border border-stone-200 bg-white px-3 py-2">
        <p className="text-sm font-semibold text-stone-900">{node.fullName}</p>
        <p className="text-xs text-stone-500">{node.campaignRole} • {node.verificationStatus} • Downlines: {node.downlineCount}</p>
      </div>
      {(node.children || []).length > 0 && (
        <div className="mt-2 space-y-2">
          {node.children.map((child) => (
            <TreeNode key={child.id} node={child} />
          ))}
        </div>
      )}
    </div>
  );
}

export default function AdminHierarchyManager() {
  const [tree, setTree] = useState(null);
  const [capacity, setCapacity] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [feedback, setFeedback] = useState('');
  const [directOnly, setDirectOnly] = useState(false);
  const [zoom, setZoom] = useState(1);
  const [addForm, setAddForm] = useState(addTemplate);
  const [reassignForm, setReassignForm] = useState(reassignTemplate);
  const [removeForm, setRemoveForm] = useState(removeTemplate);

  const maxDepth = directOnly ? 2 : 6;

  useEffect(() => {
    loadAll();
  }, [directOnly]);

  async function loadAll() {
    try {
      setLoading(true);
      setError('');
      const [treeData, capacityData] = await Promise.all([
        downlinesService.getTree(null, maxDepth),
        downlinesService.getLeaderCapacity()
      ]);
      setTree(treeData || null);
      setCapacity(Array.isArray(capacityData) ? capacityData : []);
    } catch (err) {
      console.error('Failed to load hierarchy data', err);
      setError(err.response?.data?.message || 'Unable to load hierarchy manager data.');
    } finally {
      setLoading(false);
    }
  }

  async function addDownline(event) {
    event.preventDefault();
    setError('');
    setFeedback('');
    try {
      await downlinesService.add(addForm);
      setFeedback('Downline added successfully.');
      setAddForm(addTemplate);
      await loadAll();
    } catch (err) {
      console.error('Add downline failed', err);
      setError(err.response?.data?.message || 'Failed to add downline.');
    }
  }

  async function reassignDownline(event) {
    event.preventDefault();
    setError('');
    setFeedback('');
    try {
      await downlinesService.reassign(reassignForm);
      setFeedback('Downline reassigned successfully.');
      setReassignForm(reassignTemplate);
      await loadAll();
    } catch (err) {
      console.error('Reassign downline failed', err);
      setError(err.response?.data?.message || 'Failed to reassign downline.');
    }
  }

  async function removeDownline(event) {
    event.preventDefault();
    setError('');
    setFeedback('');
    try {
      await downlinesService.remove(removeForm);
      setFeedback('Downline removed successfully.');
      setRemoveForm(removeTemplate);
      await loadAll();
    } catch (err) {
      console.error('Remove downline failed', err);
      setError(err.response?.data?.message || 'Failed to remove downline.');
    }
  }

  const capacityAtRisk = useMemo(
    () => capacity.filter((item) => item.remainingCapacity <= 1),
    [capacity]
  );

  if (loading) {
    return <div className="p-8 text-sm text-stone-600">Loading hierarchy manager...</div>;
  }

  return (
    <div className="p-8 space-y-6 bg-stone-50 rounded-lg">
      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex flex-wrap items-center justify-between gap-4">
          <div>
            <h2 className="text-2xl font-bold text-stone-900">Downline Hierarchy Manager</h2>
            <p className="text-sm text-stone-600 mt-1">Full-screen tree view with direct-only/full-tree toggle, assignment tools, and cap visibility.</p>
          </div>
          <div className="flex items-center gap-3">
            <label className="text-sm text-stone-600">Zoom</label>
            <input type="range" min="0.7" max="1.4" step="0.1" value={zoom} onChange={(event) => setZoom(Number(event.target.value))} />
            <button type="button" onClick={() => setDirectOnly((current) => !current)} className="rounded-lg border border-stone-300 px-3 py-2 text-sm text-stone-700 hover:bg-stone-100">
              {directOnly ? 'Show Full Tree' : 'Direct-Only View'}
            </button>
          </div>
        </div>
        {error && (
          <div className="mt-3 rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700 flex items-center justify-between">
            <span>{error}</span>
            <button onClick={loadAll} className="ml-4 text-sm underline hover:no-underline shrink-0">Retry</button>
          </div>
        )}
        {feedback && <div className="mt-3 rounded-lg border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-700">{feedback}</div>}
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1.2fr_1fr] gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-4 shadow-sm overflow-auto">
          <div style={{ transform: `scale(${zoom})`, transformOrigin: 'top left' }}>
            {!tree ? (
              <p className="text-sm text-stone-500">Hierarchy root not available.</p>
            ) : (
              <TreeNode node={tree} />
            )}
          </div>
        </div>

        <div className="space-y-4">
          <form onSubmit={addDownline} className="rounded-2xl border border-stone-200 bg-white p-4 shadow-sm space-y-2">
            <h3 className="text-lg font-semibold text-stone-900">Add Downline</h3>
            <input value={addForm.leaderUserId} onChange={(event) => setAddForm((c) => ({ ...c, leaderUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Leader user ID" required />
            <input value={addForm.downlineUserId} onChange={(event) => setAddForm((c) => ({ ...c, downlineUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Downline user ID" required />
            <button type="submit" className="rounded-lg bg-stone-900 px-3 py-2 text-sm font-semibold text-white hover:bg-stone-700">Add</button>
          </form>

          <form onSubmit={reassignDownline} className="rounded-2xl border border-stone-200 bg-white p-4 shadow-sm space-y-2">
            <h3 className="text-lg font-semibold text-stone-900">Reassign Downline</h3>
            <input value={reassignForm.currentLeaderUserId} onChange={(event) => setReassignForm((c) => ({ ...c, currentLeaderUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Current leader user ID" required />
            <input value={reassignForm.newLeaderUserId} onChange={(event) => setReassignForm((c) => ({ ...c, newLeaderUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="New leader user ID" required />
            <input value={reassignForm.downlineUserId} onChange={(event) => setReassignForm((c) => ({ ...c, downlineUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Downline user ID" required />
            <button type="submit" className="rounded-lg bg-stone-900 px-3 py-2 text-sm font-semibold text-white hover:bg-stone-700">Reassign</button>
          </form>

          <form onSubmit={removeDownline} className="rounded-2xl border border-stone-200 bg-white p-4 shadow-sm space-y-2">
            <h3 className="text-lg font-semibold text-stone-900">Remove Downline</h3>
            <input value={removeForm.leaderUserId} onChange={(event) => setRemoveForm((c) => ({ ...c, leaderUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Leader user ID" required />
            <input value={removeForm.downlineUserId} onChange={(event) => setRemoveForm((c) => ({ ...c, downlineUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Downline user ID" required />
            <button type="submit" className="rounded-lg border border-rose-300 bg-rose-50 px-3 py-2 text-sm font-semibold text-rose-700 hover:bg-rose-100">Remove</button>
          </form>
        </div>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <h3 className="text-xl font-semibold text-stone-900">Leader capacity and conflict visibility</h3>
        <p className="text-sm text-stone-600 mt-1">Leaders with 0-1 slots remaining are flagged for assignment conflicts.</p>

        {capacityAtRisk.length > 0 && (
          <div className="mt-3 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
            {capacityAtRisk.length} leader(s) are near max downline cap.
          </div>
        )}

        <div className="mt-4 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-3">
          {capacity.map((item) => (
            <div key={item.id} className={`rounded-lg border p-3 ${item.remainingCapacity <= 1 ? 'border-amber-300 bg-amber-50' : 'border-stone-200 bg-stone-50'}`}>
              <p className="font-semibold text-stone-900">{item.fullName}</p>
              <p className="text-xs text-stone-600">{item.campaignRole} • {item.verificationStatus}</p>
              <p className="text-xs text-stone-600 mt-1">{item.directDownlines}/{item.maxDownlines} direct downlines</p>
              <p className="text-xs text-stone-700 mt-1">Remaining capacity: {item.remainingCapacity}</p>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
}
