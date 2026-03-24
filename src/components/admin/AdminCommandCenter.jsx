import React, { useEffect, useState } from 'react';
import { commandDashboardService, complianceService } from '../../services/campaignCommandService';

const initialSummary = {
  totalUsers: 0,
  totalLeaders: 0,
  totalVolunteers: 0,
  verifiedUsers: 0,
  pendingVerification: 0,
  compliantUsers: 0,
  missingVoterCards: 0,
  totalTasks: 0,
  activeTasks: 0,
  overdueTasks: 0,
  reminderBacklog: 0,
  topLeaders: [],
  verificationDistribution: [],
  taskDistribution: [],
  regionBreakdown: [],
  countyBreakdown: [],
  subCountyBreakdown: [],
  wardBreakdown: []
};

const initialCompliance = {
  totalUsers: 0,
  missingVoterCards: 0,
  pendingVoterCards: 0,
  verifiedVoterCards: 0,
  day3Escalations: 0,
  day7Escalations: 0,
  recentReminders: []
};

export default function AdminCommandCenter() {
  const [summary, setSummary] = useState(initialSummary);
  const [compliance, setCompliance] = useState(initialCompliance);
  const [opsHealth, setOpsHealth] = useState(null);
  const [loading, setLoading] = useState(true);
  const [refreshingReminders, setRefreshingReminders] = useState(false);
  const [error, setError] = useState('');
  const [reminderMessage, setReminderMessage] = useState('');

  useEffect(() => {
    loadCommandCenter();
  }, []);

  async function loadCommandCenter() {
    try {
      setLoading(true);
      setError('');

      const [summaryData, complianceData] = await Promise.all([
        commandDashboardService.getSummary(),
        complianceService.getSummary()
      ]);

      const healthData = await commandDashboardService.getOperationsHealth().catch(() => null);

      setSummary(summaryData);
      setCompliance(complianceData);
      setOpsHealth(healthData);
    } catch (err) {
      console.error('Failed to load command center', err);
      setError('The command center could not load. Check backend auth and the new campaign endpoints.');
    } finally {
      setLoading(false);
    }
  }

  async function handleQueueReminders() {
    try {
      setRefreshingReminders(true);
      setReminderMessage('');
      const response = await complianceService.queueReminder({ channel: 'InApp', dryRun: false });
      setReminderMessage(`${response.queuedCount} reminder(s) queued.`);
      await loadCommandCenter();
    } catch (err) {
      console.error('Failed to queue reminders', err);
      setReminderMessage('Failed to queue reminders.');
    } finally {
      setRefreshingReminders(false);
    }
  }

  const statCards = [
    { label: 'Total Network', value: summary.totalUsers, accent: 'bg-emerald-600' },
    { label: 'Active Leaders', value: summary.totalLeaders, accent: 'bg-sky-600' },
    { label: 'Verified Users', value: summary.verifiedUsers, accent: 'bg-amber-500' },
    { label: 'Missing Voter Cards', value: summary.missingVoterCards, accent: 'bg-rose-600' },
    { label: 'Open Tasks', value: summary.activeTasks, accent: 'bg-violet-600' },
    { label: 'Overdue Tasks', value: summary.overdueTasks, accent: 'bg-slate-800' },
    { label: 'Reminder Backlog', value: summary.reminderBacklog, accent: 'bg-orange-600' },
    { label: 'Volunteer Base', value: summary.totalVolunteers, accent: 'bg-teal-600' }
  ];

  if (loading) {
    return (
      <div className="p-8 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin text-4xl mb-4">↻</div>
          <p className="text-gray-600">Loading command center...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="p-8">
        <div className="rounded-2xl border border-red-200 bg-red-50 p-6 text-red-800">
          <h3 className="text-lg font-semibold mb-2">Command center unavailable</h3>
          <p className="mb-4">{error}</p>
          <button onClick={loadCommandCenter} className="fluent-btn fluent-btn-primary text-sm">
            ↻ Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="p-8 space-y-8 bg-stone-50 rounded-lg">
      <section className="rounded-3xl bg-gradient-to-r from-slate-900 via-emerald-900 to-green-700 text-white p-8 shadow-lg">
        <div className="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="text-sm uppercase tracking-[0.25em] text-emerald-200">National Campaign Command</p>
            <h2 className="text-3xl font-bold mt-2">Hierarchy, compliance, and field execution</h2>
            <p className="mt-3 max-w-2xl text-sm text-emerald-100">
              This view maps the existing codebase to the SRS command platform model: downlines, verification, voter-card compliance, and execution tasks.
            </p>
          </div>
          <div className="rounded-2xl bg-white/10 px-5 py-4 backdrop-blur">
            <p className="text-xs uppercase tracking-[0.2em] text-emerald-200">Compliance Escalations</p>
            <div className="mt-2 flex gap-6">
              <div>
                <p className="text-3xl font-bold">{compliance.day3Escalations}</p>
                <p className="text-xs text-emerald-100">Day 3 warnings</p>
              </div>
              <div>
                <p className="text-3xl font-bold">{compliance.day7Escalations}</p>
                <p className="text-xs text-emerald-100">Day 7 escalations</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-4">
        {statCards.map(card => (
          <div key={card.label} className="overflow-hidden rounded-2xl border border-stone-200 bg-white shadow-sm">
            <div className={`h-2 ${card.accent}`}></div>
            <div className="p-5">
              <p className="text-sm text-stone-500">{card.label}</p>
              <p className="mt-3 text-3xl font-bold text-stone-900">{card.value}</p>
            </div>
          </div>
        ))}
      </section>

      {opsHealth && (
        <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Operational health</h3>
          <div className="mt-4 grid grid-cols-1 sm:grid-cols-3 gap-3">
            <div className="rounded-xl bg-stone-50 p-4">
              <p className="text-sm text-stone-500">Health score</p>
              <p className="text-3xl font-bold text-stone-900">{opsHealth.healthScore}</p>
            </div>
            <div className="rounded-xl bg-stone-50 p-4">
              <p className="text-sm text-stone-500">Status</p>
              <p className="text-2xl font-bold text-stone-900">{opsHealth.status}</p>
            </div>
            <div className="rounded-xl bg-stone-50 p-4">
              <p className="text-sm text-stone-500">Queued messages</p>
              <p className="text-3xl font-bold text-stone-900">{opsHealth.queuedMessages}</p>
            </div>
          </div>
        </section>
      )}

      <section className="grid grid-cols-1 gap-6 xl:grid-cols-[1.4fr_1fr]">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <div className="flex items-center justify-between gap-4 mb-5">
            <div>
              <h3 className="text-xl font-semibold text-stone-900">Top hierarchy leaders</h3>
              <p className="text-sm text-stone-500">Direct-downline capacity and compliance status at a glance.</p>
            </div>
          </div>

          <div className="space-y-3">
            {summary.topLeaders.length === 0 ? (
              <p className="text-sm text-stone-500">No hierarchy leaders have been linked yet.</p>
            ) : (
              summary.topLeaders.map(leader => (
                <div key={leader.id} className="rounded-2xl border border-stone-200 bg-stone-50 p-4">
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div>
                      <p className="font-semibold text-stone-900">{leader.fullName}</p>
                      <p className="text-sm text-stone-500">{leader.campaignRole} {leader.region ? `• ${leader.region}` : ''}{leader.county ? ` / ${leader.county}` : ''}</p>
                    </div>
                    <div className="flex flex-wrap gap-2 text-sm">
                      <span className="rounded-full bg-sky-100 px-3 py-1 text-sky-700">{leader.downlineCount}/10 downlines</span>
                      <span className="rounded-full bg-emerald-100 px-3 py-1 text-emerald-700">{leader.verificationStatus}</span>
                      <span className="rounded-full bg-amber-100 px-3 py-1 text-amber-700">{leader.voterCardStatus}</span>
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="space-y-6">
          <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
            <h3 className="text-xl font-semibold text-stone-900">Distribution snapshot</h3>
            <div className="mt-5 space-y-4">
              <div>
                <p className="text-sm font-medium text-stone-700 mb-2">Verification</p>
                <div className="space-y-2">
                  {summary.verificationDistribution.map(item => (
                    <div key={item.status} className="flex items-center justify-between rounded-xl bg-stone-50 px-3 py-2 text-sm">
                      <span>{item.status}</span>
                      <span className="font-semibold text-stone-900">{item.count}</span>
                    </div>
                  ))}
                </div>
              </div>

              <div>
                <p className="text-sm font-medium text-stone-700 mb-2">Tasks</p>
                <div className="space-y-2">
                  {summary.taskDistribution.map(item => (
                    <div key={item.status} className="flex items-center justify-between rounded-xl bg-stone-50 px-3 py-2 text-sm">
                      <span>{item.status}</span>
                      <span className="font-semibold text-stone-900">{item.count}</span>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>

          <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
            <div className="flex items-start justify-between gap-4">
              <div>
                <h3 className="text-xl font-semibold text-stone-900">Compliance actions</h3>
                <p className="text-sm text-stone-500">Queue daily voter-card reminders from the admin panel.</p>
              </div>
              <button
                onClick={handleQueueReminders}
                disabled={refreshingReminders}
                className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white transition hover:bg-stone-700 disabled:cursor-not-allowed disabled:bg-stone-400"
              >
                {refreshingReminders ? 'Queueing...' : 'Queue Reminders'}
              </button>
            </div>
            {reminderMessage && <p className="mt-3 text-sm text-stone-600">{reminderMessage}</p>}
            <div className="mt-5 grid grid-cols-1 gap-3 sm:grid-cols-3">
              <div className="rounded-xl bg-rose-50 p-4">
                <p className="text-sm text-rose-700">Missing cards</p>
                <p className="mt-2 text-2xl font-bold text-rose-900">{compliance.missingVoterCards}</p>
              </div>
              <div className="rounded-xl bg-amber-50 p-4">
                <p className="text-sm text-amber-700">Pending cards</p>
                <p className="mt-2 text-2xl font-bold text-amber-900">{compliance.pendingVoterCards}</p>
              </div>
              <div className="rounded-xl bg-emerald-50 p-4">
                <p className="text-sm text-emerald-700">Verified cards</p>
                <p className="mt-2 text-2xl font-bold text-emerald-900">{compliance.verifiedVoterCards}</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <h3 className="text-xl font-semibold text-stone-900">Recent reminder queue</h3>
        <div className="mt-5 overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b border-stone-200 text-left text-stone-500">
                <th className="pb-3 pr-4 font-medium">User</th>
                <th className="pb-3 pr-4 font-medium">Channel</th>
                <th className="pb-3 pr-4 font-medium">Status</th>
                <th className="pb-3 pr-4 font-medium">Escalation</th>
                <th className="pb-3 font-medium">Queued</th>
              </tr>
            </thead>
            <tbody>
              {compliance.recentReminders.length === 0 ? (
                <tr>
                  <td className="py-4 text-stone-500" colSpan="5">No compliance reminders have been queued yet.</td>
                </tr>
              ) : (
                compliance.recentReminders.map(reminder => (
                  <tr key={reminder.id} className="border-b border-stone-100 last:border-b-0">
                    <td className="py-4 pr-4 font-medium text-stone-900">{reminder.fullName}</td>
                    <td className="py-4 pr-4 text-stone-600">{reminder.channel}</td>
                    <td className="py-4 pr-4 text-stone-600">{reminder.status}</td>
                    <td className="py-4 pr-4 text-stone-600">Level {reminder.escalationLevel}</td>
                    <td className="py-4 text-stone-600">{new Date(reminder.createdAt).toLocaleString()}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-2 gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Regional drilldown</h3>
          <div className="mt-4 space-y-2 max-h-72 overflow-y-auto pr-1">
            {(summary.regionBreakdown || []).map((row) => (
              <div key={row.region} className="rounded-lg border border-stone-200 bg-stone-50 px-3 py-2 text-sm flex items-center justify-between">
                <span>{row.region}</span>
                <span className="text-stone-700">Users {row.users} • Verified {row.verified}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">County and sub-county drilldown</h3>
          <div className="mt-4 space-y-2 max-h-72 overflow-y-auto pr-1">
            {(summary.countyBreakdown || []).slice(0, 25).map((row, index) => (
              <div key={`${row.region}-${row.county}-${index}`} className="rounded-lg border border-stone-200 bg-stone-50 px-3 py-2 text-sm">
                <p className="font-semibold text-stone-900">{row.region} / {row.county}</p>
                <p className="text-xs text-stone-600">Users {row.users} • Verified {row.verified}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
}