import React, { useEffect, useState } from 'react';
import { warRoomService } from '../../services/campaignCommandService';

const incidentTemplate = {
  title: '',
  description: '',
  severity: 'Medium',
  lane: 'strategy'
};

const legalCaseTemplate = {
  incidentId: '',
  title: '',
  jurisdiction: '',
  filingType: '',
  summary: ''
};

const podTemplate = {
  name: '',
  battlegroundRegion: '',
  demographicBloc: '',
  opponentLane: '',
  focusObjective: ''
};

const decisionTemplate = {
  decisionTitle: '',
  decisionSummary: '',
  checkpointHourLabel: '',
  ownerRole: '',
  severity: 'Medium',
  notes: ''
};

const coalitionTemplate = {
  name: '',
  directorName: ''
};

export default function AdminWarRoom() {
  const [state, setState] = useState(null);
  const [battleRhythm, setBattleRhythm] = useState([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');
  const [feedback, setFeedback] = useState('');
  const [incidentForm, setIncidentForm] = useState(incidentTemplate);
  const [legalCases, setLegalCases] = useState([]);
  const [legalCaseForm, setLegalCaseForm] = useState(legalCaseTemplate);
  const [podForm, setPodForm] = useState(podTemplate);
  const [redZoneState, setRedZoneState] = useState(null);
  const [decisionForm, setDecisionForm] = useState(decisionTemplate);
  const [commandGrid, setCommandGrid] = useState([]);
  const [coalitions, setCoalitions] = useState([]);
  const [coalitionForm, setCoalitionForm] = useState(coalitionTemplate);
  const [mobilizationRoles, setMobilizationRoles] = useState([]);
  const [campaignPhases, setCampaignPhases] = useState(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    loadState();
  }, []);

  async function loadState({ silent = false } = {}) {
    try {
      if (!silent) {
        setLoading(true);
      } else {
        setRefreshing(true);
      }
      setError('');

      const [snapshot, rhythmItems, legalCaseItems, redZone, grid, coalitionItems, roleItems, phaseState] = await Promise.all([
        warRoomService.getState(),
        warRoomService.getBattleRhythm(),
        warRoomService.getLegalCases(),
        warRoomService.getRedZoneState(),
        warRoomService.getCommandGrid(),
        warRoomService.getCoalitions(),
        warRoomService.getMobilizationRoles(),
        warRoomService.getCampaignPhases()
      ]);
      setState(snapshot);
      setBattleRhythm(Array.isArray(rhythmItems) ? rhythmItems : []);
      setLegalCases(Array.isArray(legalCaseItems) ? legalCaseItems : []);
      setRedZoneState(redZone || null);
      setCommandGrid(Array.isArray(grid) ? grid : []);
      setCoalitions(Array.isArray(coalitionItems) ? coalitionItems : []);
      setMobilizationRoles(Array.isArray(roleItems) ? roleItems : []);
      setCampaignPhases(phaseState || null);
    } catch (err) {
      console.error('Failed to load war room state', err);
      setError(err.response?.data?.message || 'Unable to load war room state.');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }

  const onIncidentChange = (key, value) => {
    setIncidentForm((current) => ({ ...current, [key]: value }));
  };

  const submitIncident = async (event) => {
    event.preventDefault();
    setError('');
    setFeedback('');
    setSubmitting(true);

    try {
      await warRoomService.createIncident(incidentForm);
      setFeedback('Incident created and routed to command lane.');
      setIncidentForm(incidentTemplate);
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to create incident', err);
      setError(err.response?.data?.message || 'Could not create incident.');
    } finally {
      setSubmitting(false);
    }
  };

  const updateIncidentStatus = async (incidentId, status) => {
    setError('');
    try {
      await warRoomService.updateIncident(incidentId, { status });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to update incident', err);
      setError(err.response?.data?.message || 'Could not update incident.');
    }
  };

  const escalateIncident = async (incidentId, escalationLevel) => {
    setError('');
    try {
      await warRoomService.escalateIncident(incidentId, {
        escalationLevel,
        rationale: `Escalated to level ${escalationLevel} from war-room board.`
      });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to escalate incident', err);
      setError(err.response?.data?.message || 'Could not escalate incident.');
    }
  };

  const completeRhythmItem = async (itemId) => {
    setError('');
    try {
      await warRoomService.completeBattleRhythmItem(itemId, {
        attendanceCount: 0,
        completionNotes: 'Completed from war-room board.'
      });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to complete rhythm item', err);
      setError(err.response?.data?.message || 'Could not complete battle-rhythm item.');
    }
  };

  const onLegalCaseChange = (key, value) => {
    setLegalCaseForm((current) => ({ ...current, [key]: value }));
  };

  const onPodChange = (key, value) => {
    setPodForm((current) => ({ ...current, [key]: value }));
  };

  const submitCommandPod = async (event) => {
    event.preventDefault();
    setError('');
    try {
      await warRoomService.createCommandPod(podForm);
      setPodForm(podTemplate);
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to create command pod', err);
      setError(err.response?.data?.message || 'Could not create command pod.');
    }
  };

  const updatePodStatus = async (podId, status) => {
    setError('');
    try {
      await warRoomService.updateCommandPod(podId, { status });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to update pod status', err);
      setError(err.response?.data?.message || 'Could not update command pod.');
    }
  };

  const onDecisionChange = (key, value) => {
    setDecisionForm((current) => ({ ...current, [key]: value }));
  };

  const toggleRedZone = async (enable) => {
    setError('');
    try {
      await warRoomService.toggleRedZoneMode({
        enable,
        decisionIntervalMinutes: 60
      });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to toggle red-zone mode', err);
      setError(err.response?.data?.message || 'Could not change election-week mode.');
    }
  };

  const submitDecision = async (event) => {
    event.preventDefault();
    setError('');
    try {
      await warRoomService.addRedZoneDecision(decisionForm);
      setDecisionForm(decisionTemplate);
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to log decision', err);
      setError(err.response?.data?.message || 'Could not log red-zone decision.');
    }
  };

  const updateGridStatus = async (nodeId, readinessStatus) => {
    setError('');
    try {
      await warRoomService.updateCommandGridNode(nodeId, { readinessStatus });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to update grid node', err);
      setError(err.response?.data?.message || 'Could not update command grid node.');
    }
  };

  const onCoalitionChange = (key, value) => {
    setCoalitionForm((current) => ({ ...current, [key]: value }));
  };

  const submitCoalition = async (event) => {
    event.preventDefault();
    setError('');
    try {
      await warRoomService.createCoalition(coalitionForm);
      setCoalitionForm(coalitionTemplate);
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to create coalition command', err);
      setError(err.response?.data?.message || 'Could not create coalition command.');
    }
  };

  const updateCoalitionModuleStatus = async (coalitionId, groupType, status) => {
    setError('');
    try {
      await warRoomService.updateCoalitionModule(coalitionId, { groupType, status });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to update coalition module', err);
      setError(err.response?.data?.message || 'Could not update coalition module.');
    }
  };

  const updateMobilizationRole = async (roleCode, status) => {
    setError('');
    try {
      await warRoomService.updateMobilizationRole(roleCode, { status });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to update mobilization role', err);
      setError(err.response?.data?.message || 'Could not update mobilization role.');
    }
  };

  const switchPhase = async (phase) => {
    setError('');
    try {
      await warRoomService.switchCampaignPhase({ phase });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to switch campaign phase', err);
      setError(err.response?.data?.message || 'Could not switch campaign phase.');
    }
  };

  const submitLegalCase = async (event) => {
    event.preventDefault();
    setError('');
    try {
      await warRoomService.createLegalCase({
        incidentId: legalCaseForm.incidentId ? Number(legalCaseForm.incidentId) : null,
        title: legalCaseForm.title,
        jurisdiction: legalCaseForm.jurisdiction,
        filingType: legalCaseForm.filingType,
        summary: legalCaseForm.summary || null
      });
      setLegalCaseForm(legalCaseTemplate);
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to create legal case', err);
      setError(err.response?.data?.message || 'Could not create legal case.');
    }
  };

  const updateLegalCaseStatus = async (caseId, status) => {
    setError('');
    try {
      await warRoomService.updateLegalCase(caseId, { status });
      await loadState({ silent: true });
    } catch (err) {
      console.error('Failed to update legal case', err);
      setError(err.response?.data?.message || 'Could not update legal case.');
    }
  };

  if (loading) {
    return (
      <div className="p-8 text-center text-stone-600">
        <div className="text-4xl animate-pulse">🛰️</div>
        <p className="mt-2">Booting war-room command board...</p>
      </div>
    );
  }

  const lanes = state?.lanes || [];
  const incidents = state?.activeIncidents || [];
  const pods = state?.commandPods || [];

  return (
    <div className="p-8 space-y-6 bg-stone-50 rounded-lg">
      <section className="rounded-3xl bg-gradient-to-r from-slate-950 via-slate-800 to-blue-900 text-white p-8 shadow-lg">
        <div className="flex items-start justify-between gap-4">
          <div>
            <p className="text-xs uppercase tracking-[0.22em] text-blue-200">War Room</p>
            <h2 className="text-3xl font-bold mt-2">Integrated command orchestration board</h2>
            <p className="mt-3 text-sm text-blue-100 max-w-3xl">
              Track command lanes, create incidents, and drive tactical response from a single operations surface.
            </p>
          </div>
          <button
            type="button"
            onClick={() => loadState({ silent: true })}
            disabled={refreshing}
            className="rounded-xl bg-white/10 px-4 py-2 text-sm font-semibold hover:bg-white/20 disabled:opacity-40"
          >
            {refreshing ? 'Refreshing...' : 'Refresh State'}
          </button>
        </div>
      </section>

      {error && <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
      {feedback && <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{feedback}</div>}

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <h3 className="text-xl font-semibold text-stone-900">Command lanes</h3>
          <p className="text-xs text-stone-500">Snapshot: {new Date(state.snapshotAt).toLocaleString()}</p>
        </div>

        <div className="mt-5 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
          {lanes.map((lane) => (
            <div key={lane.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="font-semibold text-stone-900">{lane.name}</p>
                  <p className="text-xs text-stone-500 mt-1">Lead: {lane.lead}</p>
                </div>
                <span className="rounded-full bg-sky-100 px-3 py-1 text-xs font-semibold text-sky-700">
                  {lane.status}
                </span>
              </div>
              <div className="mt-4 flex items-center justify-between text-sm text-stone-700">
                <span>Open incidents</span>
                <span className="font-bold text-stone-900">{lane.openIncidents}</span>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <h3 className="text-xl font-semibold text-stone-900">Daily battle rhythm</h3>
          <p className="text-sm text-stone-500">06:00 / 09:00 / 14:00 / 19:00 command cadence</p>
        </div>

        <div className="mt-4 grid grid-cols-1 md:grid-cols-2 xl:grid-cols-4 gap-3">
          {battleRhythm.map((item) => (
            <div key={item.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
              <div className="flex items-center justify-between gap-3">
                <p className="text-sm font-semibold text-stone-900">{item.timeSlot}</p>
                <span className={`rounded-full px-2 py-0.5 text-xs font-semibold ${item.completed ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'}`}>
                  {item.completed ? 'Completed' : 'Pending'}
                </span>
              </div>
              <p className="mt-2 font-medium text-stone-900">{item.name}</p>
              <p className="mt-2 text-xs text-stone-600">{item.purpose}</p>

              {!item.completed && (
                <button
                  type="button"
                  onClick={() => completeRhythmItem(item.id)}
                  className="mt-3 rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100"
                >
                  Mark Completed
                </button>
              )}
            </div>
          ))}
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1.2fr] gap-6">
        <form onSubmit={submitCommandPod} className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-3">
          <h3 className="text-xl font-semibold text-stone-900">Integrated command matrix</h3>
          <p className="text-sm text-stone-500">Create cross-functional pods (data + field + digital + comms) and assign mission lanes.</p>

          <input
            value={podForm.name}
            onChange={(event) => onPodChange('name', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Pod name"
            required
          />
          <input
            value={podForm.battlegroundRegion}
            onChange={(event) => onPodChange('battlegroundRegion', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Battleground region"
            required
          />
          <input
            value={podForm.demographicBloc}
            onChange={(event) => onPodChange('demographicBloc', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Demographic bloc"
            required
          />
          <input
            value={podForm.opponentLane}
            onChange={(event) => onPodChange('opponentLane', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Opponent lane"
            required
          />
          <textarea
            value={podForm.focusObjective}
            onChange={(event) => onPodChange('focusObjective', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            rows={3}
            placeholder="Focus objective"
          />

          <button type="submit" className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700">
            Create Pod
          </button>
        </form>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Active pods</h3>
          <div className="mt-4 space-y-3 max-h-[30rem] overflow-y-auto pr-1">
            {pods.length === 0 ? (
              <p className="text-sm text-stone-500">No command pods configured yet.</p>
            ) : (
              pods.map((pod) => (
                <div key={pod.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <div className="flex items-center justify-between gap-3">
                    <p className="font-semibold text-stone-900">{pod.name}</p>
                    <span className="rounded-full bg-sky-100 px-2 py-0.5 text-xs font-semibold text-sky-700">{pod.status}</span>
                  </div>
                  <p className="mt-2 text-xs text-stone-600">Region: {pod.battlegroundRegion}</p>
                  <p className="text-xs text-stone-600">Bloc: {pod.demographicBloc}</p>
                  <p className="text-xs text-stone-600">Opponent lane: {pod.opponentLane}</p>
                  {pod.focusObjective && <p className="mt-2 text-sm text-stone-700">{pod.focusObjective}</p>}

                  <div className="mt-3 flex flex-wrap gap-2">
                    <button type="button" onClick={() => updatePodStatus(pod.id, 'Active')} className="rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100">Active</button>
                    <button type="button" onClick={() => updatePodStatus(pod.id, 'Standby')} className="rounded-lg border border-amber-300 px-3 py-1 text-xs text-amber-700 hover:bg-amber-50">Standby</button>
                    <button type="button" onClick={() => updatePodStatus(pod.id, 'Escalated')} className="rounded-lg border border-orange-300 px-3 py-1 text-xs text-orange-700 hover:bg-orange-50">Escalated</button>
                    <button type="button" onClick={() => updatePodStatus(pod.id, 'Completed')} className="rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Completed</button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1.2fr] gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-4">
          <div className="flex items-center justify-between">
            <h3 className="text-xl font-semibold text-stone-900">Election-week red-zone mode</h3>
            <span className={`rounded-full px-3 py-1 text-xs font-semibold ${redZoneState?.isElectionWeekModeEnabled ? 'bg-rose-100 text-rose-700' : 'bg-stone-200 text-stone-700'}`}>
              {redZoneState?.isElectionWeekModeEnabled ? 'Enabled' : 'Disabled'}
            </span>
          </div>

          <p className="text-sm text-stone-600">
            Hourly operational checkpoint mode for rapid decisions, escalation, and checkpoint tracking.
          </p>

          <div className="flex flex-wrap gap-2">
            <button type="button" onClick={() => toggleRedZone(true)} className="rounded-xl border border-rose-300 bg-rose-50 px-4 py-2 text-sm font-semibold text-rose-700 hover:bg-rose-100">Enable Red Zone</button>
            <button type="button" onClick={() => toggleRedZone(false)} className="rounded-xl border border-stone-300 px-4 py-2 text-sm font-semibold text-stone-700 hover:bg-stone-100">Disable Red Zone</button>
          </div>

          {redZoneState?.nextCheckpointAt && (
            <p className="text-xs text-stone-500">Next checkpoint: {new Date(redZoneState.nextCheckpointAt).toLocaleString()}</p>
          )}

          <form onSubmit={submitDecision} className="space-y-3">
            <input
              value={decisionForm.decisionTitle}
              onChange={(event) => onDecisionChange('decisionTitle', event.target.value)}
              className="w-full rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Decision title"
              required
            />
            <textarea
              value={decisionForm.decisionSummary}
              onChange={(event) => onDecisionChange('decisionSummary', event.target.value)}
              className="w-full rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Decision summary"
              rows={3}
              required
            />
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-2">
              <input
                value={decisionForm.checkpointHourLabel}
                onChange={(event) => onDecisionChange('checkpointHourLabel', event.target.value)}
                className="rounded-xl border border-stone-300 px-3 py-2"
                placeholder="Checkpoint (e.g. 14:00)"
                required
              />
              <input
                value={decisionForm.ownerRole}
                onChange={(event) => onDecisionChange('ownerRole', event.target.value)}
                className="rounded-xl border border-stone-300 px-3 py-2"
                placeholder="Owner role"
              />
              <select value={decisionForm.severity} onChange={(event) => onDecisionChange('severity', event.target.value)} className="rounded-xl border border-stone-300 px-3 py-2">
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
                <option value="Critical">Critical</option>
              </select>
            </div>
            <textarea
              value={decisionForm.notes}
              onChange={(event) => onDecisionChange('notes', event.target.value)}
              className="w-full rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Additional notes"
              rows={2}
            />

            <button type="submit" className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700">Log Hourly Decision</button>
          </form>
        </div>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Decision log</h3>
          <div className="mt-4 space-y-3 max-h-[30rem] overflow-y-auto pr-1">
            {(redZoneState?.decisions || []).length === 0 ? (
              <p className="text-sm text-stone-500">No hourly decisions logged yet.</p>
            ) : (
              redZoneState.decisions.map((item) => (
                <div key={item.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <div className="flex flex-wrap items-center gap-2">
                    <p className="font-semibold text-stone-900">{item.decisionTitle}</p>
                    <span className="rounded-full bg-amber-100 px-2 py-0.5 text-xs font-semibold text-amber-800">{item.severity}</span>
                    <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">{item.checkpointHourLabel}</span>
                  </div>
                  <p className="mt-2 text-sm text-stone-700">{item.decisionSummary}</p>
                  <p className="mt-2 text-xs text-stone-500">{item.ownerRole || 'Unassigned'} • {new Date(item.loggedAt).toLocaleString()}</p>
                </div>
              ))
            )}
          </div>
        </div>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between">
          <h3 className="text-xl font-semibold text-stone-900">National to polling-unit command grid</h3>
          <p className="text-sm text-stone-500">Readiness by tier: national, regional, county, sub-county, ward, polling unit</p>
        </div>

        <div className="mt-4 space-y-3">
          {commandGrid.map((node) => (
            <div key={node.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                  <p className="font-semibold text-stone-900">{node.name}</p>
                  <p className="text-xs text-stone-500">{node.tier} • Staffing {node.staffingPercent}% • Connectivity {node.connectivityStatus}</p>
                </div>
                <span className="rounded-full bg-sky-100 px-3 py-1 text-xs font-semibold text-sky-700">{node.readinessStatus}</span>
              </div>

              <div className="mt-3 flex flex-wrap gap-2">
                <button type="button" onClick={() => updateGridStatus(node.id, 'Ready')} className="rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Ready</button>
                <button type="button" onClick={() => updateGridStatus(node.id, 'Degraded')} className="rounded-lg border border-amber-300 px-3 py-1 text-xs text-amber-700 hover:bg-amber-50">Degraded</button>
                <button type="button" onClick={() => updateGridStatus(node.id, 'Critical')} className="rounded-lg border border-orange-300 px-3 py-1 text-xs text-orange-700 hover:bg-orange-50">Critical</button>
                <button type="button" onClick={() => updateGridStatus(node.id, 'Offline')} className="rounded-lg border border-rose-300 px-3 py-1 text-xs text-rose-700 hover:bg-rose-50">Offline</button>
              </div>
            </div>
          ))}
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1.2fr] gap-6">
        <form onSubmit={submitCoalition} className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-3">
          <h3 className="text-xl font-semibold text-stone-900">Coalition and special-groups command</h3>
          <p className="text-sm text-stone-500">Track youth, women, faith, diaspora, and business outreach modules with coalition directors.</p>

          <input
            value={coalitionForm.name}
            onChange={(event) => onCoalitionChange('name', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Coalition command name"
            required
          />
          <input
            value={coalitionForm.directorName}
            onChange={(event) => onCoalitionChange('directorName', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Director name"
            required
          />

          <button type="submit" className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700">
            Create Coalition Command
          </button>
        </form>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Coalition module workflows</h3>
          <div className="mt-4 space-y-4 max-h-[30rem] overflow-y-auto pr-1">
            {coalitions.length === 0 ? (
              <p className="text-sm text-stone-500">No coalition command desks configured.</p>
            ) : (
              coalitions.map((coalition) => (
                <div key={coalition.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <p className="font-semibold text-stone-900">{coalition.name}</p>
                  <p className="text-xs text-stone-500 mt-1">Director: {coalition.directorName}</p>

                  <div className="mt-3 space-y-2">
                    {(coalition.modules || []).map((module) => (
                      <div key={`${coalition.id}-${module.groupType}`} className="rounded-lg border border-stone-200 bg-white p-3">
                        <div className="flex items-center justify-between gap-2">
                          <p className="text-sm font-semibold text-stone-900">{module.groupType}</p>
                          <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">{module.status}</span>
                        </div>
                        <p className="mt-1 text-xs text-stone-600">{module.objective}</p>
                        <div className="mt-2 flex flex-wrap gap-2">
                          <button type="button" onClick={() => updateCoalitionModuleStatus(coalition.id, module.groupType, 'Planned')} className="rounded-lg border border-stone-300 px-2 py-1 text-xs text-stone-700 hover:bg-stone-100">Planned</button>
                          <button type="button" onClick={() => updateCoalitionModuleStatus(coalition.id, module.groupType, 'Active')} className="rounded-lg border border-emerald-300 px-2 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Active</button>
                          <button type="button" onClick={() => updateCoalitionModuleStatus(coalition.id, module.groupType, 'Blocked')} className="rounded-lg border border-rose-300 px-2 py-1 text-xs text-rose-700 hover:bg-rose-50">Blocked</button>
                          <button type="button" onClick={() => updateCoalitionModuleStatus(coalition.id, module.groupType, 'Completed')} className="rounded-lg border border-sky-300 px-2 py-1 text-xs text-sky-700 hover:bg-sky-50">Completed</button>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1fr] gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Kenya mobilization roles</h3>
          <p className="text-sm text-stone-500 mt-1">Party liaison, tribal/regional mobilization, religious advisory, and village elder coordination.</p>

          <div className="mt-4 space-y-3">
            {mobilizationRoles.map((role) => (
              <div key={role.roleCode} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                <div className="flex items-center justify-between gap-2">
                  <p className="font-semibold text-stone-900">{role.roleTitle}</p>
                  <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">{role.status}</span>
                </div>
                <div className="mt-2 flex flex-wrap gap-2">
                  <button type="button" onClick={() => updateMobilizationRole(role.roleCode, 'Open')} className="rounded-lg border border-stone-300 px-2 py-1 text-xs text-stone-700 hover:bg-stone-100">Open</button>
                  <button type="button" onClick={() => updateMobilizationRole(role.roleCode, 'Assigned')} className="rounded-lg border border-amber-300 px-2 py-1 text-xs text-amber-700 hover:bg-amber-50">Assigned</button>
                  <button type="button" onClick={() => updateMobilizationRole(role.roleCode, 'Active')} className="rounded-lg border border-emerald-300 px-2 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Active</button>
                  <button type="button" onClick={() => updateMobilizationRole(role.roleCode, 'Completed')} className="rounded-lg border border-sky-300 px-2 py-1 text-xs text-sky-700 hover:bg-sky-50">Completed</button>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Campaign phase management</h3>
          <p className="text-sm text-stone-500 mt-1">Switch campaign doctrine across Exploratory, Launch, Persuasion, and GOTV phases.</p>

          <div className="mt-4 rounded-xl border border-sky-200 bg-sky-50 p-4">
            <p className="text-xs uppercase tracking-[0.18em] text-sky-700">Active phase</p>
            <p className="mt-1 text-lg font-semibold text-sky-900">{campaignPhases?.activePhase || 'Exploratory'}</p>
            {campaignPhases?.activeSince && <p className="text-xs text-sky-700 mt-1">Since: {new Date(campaignPhases.activeSince).toLocaleString()}</p>}
          </div>

          <div className="mt-4 space-y-3">
            {(campaignPhases?.phases || []).map((phase) => (
              <div key={phase.phase} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                <div className="flex items-center justify-between gap-2">
                  <p className="font-semibold text-stone-900">{phase.phase}</p>
                  <span className={`rounded-full px-2 py-0.5 text-xs font-semibold ${phase.status === 'Active' ? 'bg-emerald-100 text-emerald-700' : 'bg-stone-200 text-stone-700'}`}>
                    {phase.status}
                  </span>
                </div>
                <p className="mt-2 text-xs text-stone-600">{phase.playbookSummary}</p>
                <div className="mt-2 space-y-1">
                  {(phase.kpis || []).map((kpi) => (
                    <p key={`${phase.phase}-${kpi.name}`} className="text-xs text-stone-500">{kpi.name}: {kpi.targetValue}</p>
                  ))}
                </div>

                <button type="button" onClick={() => switchPhase(phase.phase)} className="mt-3 rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100">
                  Activate Phase
                </button>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1.2fr] gap-6">
        <form onSubmit={submitIncident} className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-4">
          <h3 className="text-xl font-semibold text-stone-900">Create incident</h3>

          <input
            value={incidentForm.title}
            onChange={(event) => onIncidentChange('title', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Incident title"
            required
          />

          <textarea
            value={incidentForm.description}
            onChange={(event) => onIncidentChange('description', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Describe what happened, impact, and immediate risk"
            rows={4}
            required
          />

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <select
              value={incidentForm.severity}
              onChange={(event) => onIncidentChange('severity', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
            >
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>

            <select
              value={incidentForm.lane}
              onChange={(event) => onIncidentChange('lane', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
            >
              <option value="strategy">Strategy & Intelligence</option>
              <option value="communications">Communications</option>
              <option value="data">Data & Analytics</option>
              <option value="field">Field Operations</option>
              <option value="legal">Legal & Compliance</option>
              <option value="digital">Digital & Cyber</option>
            </select>
          </div>

          <button
            type="submit"
            disabled={submitting}
            className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700 disabled:cursor-not-allowed disabled:bg-stone-400"
          >
            {submitting ? 'Creating...' : 'Create Incident'}
          </button>
        </form>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Active incidents</h3>
          <div className="mt-4 space-y-3 max-h-[32rem] overflow-y-auto pr-1">
            {incidents.length === 0 ? (
              <p className="text-sm text-stone-500">No incidents logged. War-room lanes are operational.</p>
            ) : (
              incidents.map((incident) => (
                <div key={incident.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <div className="flex flex-wrap items-center gap-2">
                    <p className="font-semibold text-stone-900">#{incident.id} {incident.title}</p>
                    <span className="rounded-full bg-amber-100 px-2 py-0.5 text-xs font-semibold text-amber-800">
                      {incident.severity}
                    </span>
                    <span className="rounded-full bg-sky-100 px-2 py-0.5 text-xs font-semibold text-sky-700">
                      {incident.status}
                    </span>
                    <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">
                      {incident.lane}
                    </span>
                  </div>

                  <p className="mt-2 text-sm text-stone-700">{incident.description}</p>
                  <p className="mt-2 text-xs text-stone-500">Updated: {new Date(incident.updatedAt).toLocaleString()}</p>
                  <p className="mt-1 text-xs text-stone-500">Escalation: L{incident.escalationLevel} • Owner: {incident.escalationOwnerRole || 'Unassigned'}</p>
                  {incident.escalationDueAt && (
                    <p className="mt-1 text-xs text-stone-500">SLA due: {new Date(incident.escalationDueAt).toLocaleString()}</p>
                  )}

                  <div className="mt-3 flex flex-wrap gap-2">
                    <button
                      type="button"
                      onClick={() => updateIncidentStatus(incident.id, 'Triaged')}
                      className="rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100"
                    >
                      Mark Triaged
                    </button>
                    <button
                      type="button"
                      onClick={() => updateIncidentStatus(incident.id, 'Escalated')}
                      className="rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100"
                    >
                      Escalate
                    </button>
                    <button
                      type="button"
                      onClick={() => updateIncidentStatus(incident.id, 'Resolved')}
                      className="rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50"
                    >
                      Resolve
                    </button>
                    <button
                      type="button"
                      onClick={() => escalateIncident(incident.id, 1)}
                      className="rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100"
                    >
                      Escalate L1
                    </button>
                    <button
                      type="button"
                      onClick={() => escalateIncident(incident.id, 2)}
                      className="rounded-lg border border-amber-300 px-3 py-1 text-xs text-amber-700 hover:bg-amber-50"
                    >
                      Escalate L2
                    </button>
                    <button
                      type="button"
                      onClick={() => escalateIncident(incident.id, 3)}
                      className="rounded-lg border border-orange-300 px-3 py-1 text-xs text-orange-700 hover:bg-orange-50"
                    >
                      Escalate L3
                    </button>
                    <button
                      type="button"
                      onClick={() => escalateIncident(incident.id, 4)}
                      className="rounded-lg border border-rose-300 px-3 py-1 text-xs text-rose-700 hover:bg-rose-50"
                    >
                      Escalate L4
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1.2fr] gap-6">
        <form onSubmit={submitLegalCase} className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-3">
          <h3 className="text-xl font-semibold text-stone-900">Legal rapid-response intake</h3>

          <input
            value={legalCaseForm.incidentId}
            onChange={(event) => onLegalCaseChange('incidentId', event.target.value.replace(/\D/g, ''))}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Linked incident ID (optional)"
          />
          <input
            value={legalCaseForm.title}
            onChange={(event) => onLegalCaseChange('title', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Case title"
            required
          />
          <input
            value={legalCaseForm.jurisdiction}
            onChange={(event) => onLegalCaseChange('jurisdiction', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Jurisdiction"
            required
          />
          <input
            value={legalCaseForm.filingType}
            onChange={(event) => onLegalCaseChange('filingType', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            placeholder="Filing type"
            required
          />
          <textarea
            value={legalCaseForm.summary}
            onChange={(event) => onLegalCaseChange('summary', event.target.value)}
            className="w-full rounded-xl border border-stone-300 px-3 py-2"
            rows={3}
            placeholder="Initial legal summary"
          />

          <button
            type="submit"
            className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700"
          >
            Create Legal Case
          </button>
        </form>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Legal caseboard</h3>
          <div className="mt-4 space-y-3 max-h-[28rem] overflow-y-auto pr-1">
            {legalCases.length === 0 ? (
              <p className="text-sm text-stone-500">No legal cases opened yet.</p>
            ) : (
              legalCases.map((item) => (
                <div key={item.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <div className="flex items-center justify-between gap-2">
                    <p className="font-semibold text-stone-900">Case #{item.id}: {item.title}</p>
                    <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">{item.status}</span>
                  </div>
                  <p className="mt-1 text-xs text-stone-500">{item.jurisdiction} • {item.filingType}</p>
                  {item.summary && <p className="mt-2 text-sm text-stone-700">{item.summary}</p>}

                  <div className="mt-3 flex flex-wrap gap-2">
                    <button type="button" onClick={() => updateLegalCaseStatus(item.id, 'Drafting')} className="rounded-lg border border-stone-300 px-3 py-1 text-xs text-stone-700 hover:bg-stone-100">Drafting</button>
                    <button type="button" onClick={() => updateLegalCaseStatus(item.id, 'Filed')} className="rounded-lg border border-amber-300 px-3 py-1 text-xs text-amber-700 hover:bg-amber-50">Filed</button>
                    <button type="button" onClick={() => updateLegalCaseStatus(item.id, 'Hearing')} className="rounded-lg border border-orange-300 px-3 py-1 text-xs text-orange-700 hover:bg-orange-50">Hearing</button>
                    <button type="button" onClick={() => updateLegalCaseStatus(item.id, 'Closed')} className="rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Close</button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      </section>
    </div>
  );
}
