import React, { useEffect, useMemo, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { campaignMessagingService, campaignTasksService, complianceService } from '../services/campaignCommandService';

const leaderRoles = new Set(['Admin', 'SuperAdmin', 'RegionalLeader', 'CountyLeader', 'SubCountyLeader', 'ConstituencyLeader', 'WardLeader']);

const taskTemplate = {
  title: '',
  description: '',
  deadline: '',
  priority: 'Medium',
  assignedToUserId: ''
};

export default function TaskExecutionHub() {
  const { user } = useAuth();
  const [tasks, setTasks] = useState([]);
  const [complianceSummary, setComplianceSummary] = useState(null);
  const [taskForm, setTaskForm] = useState(taskTemplate);
  const [inbox, setInbox] = useState([]);
  const [templateKey, setTemplateKey] = useState('DailyVoterCardReminder');
  const [dualChannel, setDualChannel] = useState(true);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [feedback, setFeedback] = useState('');

  const canManage = useMemo(() => {
    const roles = Array.isArray(user?.roles) ? user.roles : [];
    return roles.some((role) => leaderRoles.has(role));
  }, [user]);

  useEffect(() => {
    loadData();
  }, [canManage]);

  async function loadData() {
    try {
      setLoading(true);
      setError('');

      const taskPromise = campaignTasksService.getMine();
      const compliancePromise = canManage ? complianceService.getSummary() : Promise.resolve(null);
      const inboxPromise = campaignMessagingService.getInbox();

      const [taskItems, compliance, inboxItems] = await Promise.all([taskPromise, compliancePromise, inboxPromise]);
      setTasks(Array.isArray(taskItems) ? taskItems : []);
      setComplianceSummary(compliance);
      setInbox(Array.isArray(inboxItems) ? inboxItems : []);
    } catch (err) {
      console.error('Failed to load task execution data', err);
      setError(err.response?.data?.message || 'Unable to load task execution data.');
    } finally {
      setLoading(false);
    }
  }

  async function updateToInProgress(taskId) {
    setError('');
    setFeedback('');
    try {
      await campaignTasksService.updateStatus({ taskId, status: 'In Progress' });
      setFeedback('Task moved to In Progress.');
      await loadData();
    } catch (err) {
      console.error('Task status update failed', err);
      setError(err.response?.data?.message || 'Could not move task to In Progress.');
    }
  }

  async function completeTask(taskId) {
    setError('');
    setFeedback('');
    try {
      await campaignTasksService.complete({ taskId, completionNotes: 'Completed from execution hub.' });
      setFeedback('Task completed successfully.');
      await loadData();
    } catch (err) {
      console.error('Task completion failed', err);
      setError(err.response?.data?.message || 'Could not complete task.');
    }
  }

  async function createTask(event) {
    event.preventDefault();
    setError('');
    setFeedback('');

    try {
      await campaignTasksService.create({
        title: taskForm.title,
        description: taskForm.description,
        deadline: new Date(taskForm.deadline).toISOString(),
        priority: taskForm.priority,
        assignedToUserId: taskForm.assignedToUserId || null
      });

      setTaskForm(taskTemplate);
      setFeedback('Task created successfully.');
      await loadData();
    } catch (err) {
      console.error('Task creation failed', err);
      setError(err.response?.data?.message || 'Could not create task.');
    }
  }

  async function queueComplianceReminder(channel) {
    setError('');
    setFeedback('');
    try {
      const result = await complianceService.queueReminder({
        channel,
        dryRun: false,
        templateKey,
        dualChannel
      });
      setFeedback(`Queued ${result.queuedCount || 0} compliance reminders via ${channel}.`);
      await loadData();
    } catch (err) {
      console.error('Compliance reminder queue failed', err);
      setError(err.response?.data?.message || 'Could not queue compliance reminders.');
    }
  }

  async function markMessageRead(messageId) {
    setError('');
    try {
      await campaignMessagingService.acknowledgeRead(messageId);
      await loadData();
    } catch (err) {
      console.error('Read acknowledgment failed', err);
      setError(err.response?.data?.message || 'Could not mark message as read.');
    }
  }

  if (loading) {
    return <div className="p-8 text-sm text-gray-600">Loading task execution hub...</div>;
  }

  return (
    <div className="min-h-screen bg-stone-50 p-[5%] space-y-6">
      <section className="rounded-2xl bg-gradient-to-r from-emerald-900 via-emerald-700 to-amber-600 p-8 text-white shadow-lg">
        <h1 className="text-3xl font-bold">Daily Execution Hub</h1>
        <p className="mt-2 text-sm text-emerald-100">Unified task action surface with compliance reminder controls.</p>
      </section>

      {error && <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">{error}</div>}
      {feedback && <div className="rounded-lg border border-emerald-200 bg-emerald-50 p-3 text-sm text-emerald-700">{feedback}</div>}

      <section className="grid grid-cols-1 xl:grid-cols-[1.2fr_1fr] gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h2 className="text-xl font-semibold text-stone-900">My tasks</h2>
          <div className="mt-4 space-y-3">
            {tasks.length === 0 ? (
              <p className="text-sm text-stone-500">No tasks assigned right now.</p>
            ) : (
              tasks.map((task) => (
                <div key={task.id} className="rounded-lg border border-stone-200 bg-stone-50 p-4">
                  <div className="flex flex-wrap items-center gap-2">
                    <p className="font-semibold text-stone-900">{task.title}</p>
                    <span className="rounded-full bg-sky-100 px-2 py-0.5 text-xs font-semibold text-sky-700">{task.status}</span>
                    <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">{task.priority}</span>
                  </div>
                  <p className="mt-2 text-sm text-stone-700">{task.description}</p>
                  <p className="mt-1 text-xs text-stone-500">Due: {new Date(task.deadline).toLocaleString()}</p>

                  <div className="mt-3 flex flex-wrap gap-2">
                    {task.status === 'Pending' && (
                      <button type="button" onClick={() => updateToInProgress(task.id)} className="rounded-lg border border-amber-300 px-3 py-1 text-xs text-amber-700 hover:bg-amber-50">Start</button>
                    )}
                    {task.status === 'In Progress' && (
                      <button type="button" onClick={() => completeTask(task.id)} className="rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Complete</button>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="space-y-6">
          {canManage && (
            <form onSubmit={createTask} className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-3">
              <h2 className="text-xl font-semibold text-stone-900">Leader task creation</h2>
              <input value={taskForm.title} onChange={(event) => setTaskForm((c) => ({ ...c, title: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2" placeholder="Task title" required />
              <textarea value={taskForm.description} onChange={(event) => setTaskForm((c) => ({ ...c, description: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2" rows={3} placeholder="Task description" required />
              <input type="datetime-local" value={taskForm.deadline} onChange={(event) => setTaskForm((c) => ({ ...c, deadline: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2" required />
              <select value={taskForm.priority} onChange={(event) => setTaskForm((c) => ({ ...c, priority: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2">
                <option value="Low">Low</option>
                <option value="Medium">Medium</option>
                <option value="High">High</option>
                <option value="Critical">Critical</option>
              </select>
              <input value={taskForm.assignedToUserId} onChange={(event) => setTaskForm((c) => ({ ...c, assignedToUserId: event.target.value }))} className="w-full rounded-lg border border-stone-300 px-3 py-2" placeholder="Assigned user ID (optional)" />
              <button type="submit" className="rounded-lg bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700">Create Task</button>
            </form>
          )}

          {canManage && (
            <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm space-y-3">
              <h2 className="text-xl font-semibold text-stone-900">Compliance reminders</h2>
              <p className="text-sm text-stone-600">Queue voter-card reminders from the same daily execution screen.</p>
              <select value={templateKey} onChange={(event) => setTemplateKey(event.target.value)} className="w-full rounded-lg border border-stone-300 px-3 py-2 text-sm">
                <option value="DailyVoterCardReminder">Daily reminder template</option>
                <option value="NearestOffice">Nearest IEC office guidance</option>
                <option value="Day3Escalation">Day-3 escalation template</option>
                <option value="Day7Escalation">Day-7 escalation template</option>
              </select>
              <label className="flex items-center gap-2 text-sm text-stone-700">
                <input type="checkbox" checked={dualChannel} onChange={(event) => setDualChannel(event.target.checked)} />
                Send via dual channel (selected channel + WhatsApp)
              </label>
              <div className="grid grid-cols-2 gap-2 text-sm">
                <button type="button" onClick={() => queueComplianceReminder('WhatsApp')} className="rounded-lg border border-emerald-300 bg-emerald-50 px-3 py-2 font-semibold text-emerald-700 hover:bg-emerald-100">Queue WhatsApp</button>
                <button type="button" onClick={() => queueComplianceReminder('InApp')} className="rounded-lg border border-sky-300 bg-sky-50 px-3 py-2 font-semibold text-sky-700 hover:bg-sky-100">Queue In-App</button>
              </div>
              {complianceSummary && (
                <div className="rounded-lg border border-stone-200 bg-stone-50 p-3 text-xs text-stone-700 space-y-1">
                  <p>Total users: {complianceSummary.totalUsers}</p>
                  <p>Missing voter cards: {complianceSummary.missingVoterCards}</p>
                  <p>Day-3 escalations: {complianceSummary.day3Escalations}</p>
                  <p>Day-7 escalations: {complianceSummary.day7Escalations}</p>
                </div>
              )}
            </div>
          )}
        </div>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <h2 className="text-xl font-semibold text-stone-900">Message inbox and read status</h2>
        <p className="mt-1 text-sm text-stone-600">Track delivered updates and acknowledge reads for command reporting.</p>

        <div className="mt-4 space-y-3">
          {inbox.length === 0 ? (
            <p className="text-sm text-stone-500">No messages in inbox.</p>
          ) : (
            inbox.slice(0, 40).map((message) => (
              <div key={message.id} className="rounded-lg border border-stone-200 bg-stone-50 p-4">
                <div className="flex flex-wrap items-center gap-2">
                  <p className="font-semibold text-stone-900">{message.title || 'Campaign Message'}</p>
                  <span className="rounded-full bg-sky-100 px-2 py-0.5 text-xs font-semibold text-sky-700">{message.channel}</span>
                  <span className="rounded-full bg-stone-200 px-2 py-0.5 text-xs text-stone-700">{message.status}</span>
                </div>
                <p className="mt-2 text-sm text-stone-700">{message.body}</p>
                <p className="mt-1 text-xs text-stone-500">Delivered: {message.deliveredAt ? new Date(message.deliveredAt).toLocaleString() : 'Pending'}</p>
                <p className="text-xs text-stone-500">Read: {message.readAt ? new Date(message.readAt).toLocaleString() : 'Not read'}</p>

                {!message.readAt && (
                  <button type="button" onClick={() => markMessageRead(message.id)} className="mt-2 rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50">
                    Mark Read
                  </button>
                )}
              </div>
            ))
          )}
        </div>
      </section>
    </div>
  );
}
