import React, { useEffect, useState } from 'react';
import { electionResultsService } from '../../services/campaignCommandService';

const defaultSubmission = {
  pollingStationCode: '',
  constituency: '',
  county: '',
  region: '',
  latitude: '',
  longitude: '',
  deviceFingerprint: '',
  candidateA: 0,
  candidateB: 0,
  candidateC: 0,
  rejectedVotes: 0,
  registeredVoters: 0
};

export default function AdminElectionDashboard() {
  const [submissionForm, setSubmissionForm] = useState(defaultSubmission);
  const [reportingWindow, setReportingWindow] = useState('');
  const [countyFilter, setCountyFilter] = useState('');
  const [status, setStatus] = useState(null);
  const [aggregateRows, setAggregateRows] = useState([]);
  const [pendingReview, setPendingReview] = useState([]);
  const [conflicts, setConflicts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [feedback, setFeedback] = useState('');

  useEffect(() => {
    loadDashboardData();
  }, []);

  async function loadDashboardData() {
    try {
      setLoading(true);
      setError('');

      const [statusData, aggregateData, pendingData, conflictData] = await Promise.all([
        electionResultsService.getStatus({ reportingWindow: reportingWindow || undefined }),
        electionResultsService.getAggregate({
          reportingWindow: reportingWindow || undefined,
          county: countyFilter || undefined
        }),
        electionResultsService.getPendingReview({ reportingWindow: reportingWindow || undefined }),
        electionResultsService.getConflicts({ reportingWindow: reportingWindow || undefined })
      ]);

      setStatus(statusData);
      setAggregateRows(Array.isArray(aggregateData) ? aggregateData : []);
      setPendingReview(Array.isArray(pendingData) ? pendingData : []);
      setConflicts(Array.isArray(conflictData) ? conflictData : []);
    } catch (err) {
      console.error('Failed to load election dashboard data', err);
      setError(err.response?.data?.message || 'Unable to load election data.');
    } finally {
      setLoading(false);
    }
  }

  const onSubmissionChange = (field, value) => {
    setSubmissionForm((current) => ({
      ...current,
      [field]: ['candidateA', 'candidateB', 'candidateC', 'rejectedVotes', 'registeredVoters'].includes(field)
        ? Number(value || 0)
        : value
    }));
  };

  const resetSubmission = () => {
    setSubmissionForm(defaultSubmission);
  };

  const handleSubmitResult = async (event) => {
    event.preventDefault();
    setError('');
    setFeedback('');
    setSubmitting(true);

    try {
      const payload = {
        ...submissionForm,
        latitude: submissionForm.latitude === '' ? null : Number(submissionForm.latitude),
        longitude: submissionForm.longitude === '' ? null : Number(submissionForm.longitude),
        deviceFingerprint: submissionForm.deviceFingerprint || null
      };

      const response = await electionResultsService.submit(payload);
      setFeedback(`Submission accepted. Status: ${response.status}.`);
      resetSubmission();
      await loadDashboardData();
    } catch (err) {
      console.error('Failed to submit election result', err);
      setError(err.response?.data?.message || 'Submission failed. Please review values and try again.');
    } finally {
      setSubmitting(false);
    }
  };

  const reviewResult = async (resultId, decision) => {
    setError('');
    try {
      await electionResultsService.reviewResult(resultId, {
        decision,
        notes: `Reviewed in dashboard with decision ${decision}.`
      });
      await loadDashboardData();
    } catch (err) {
      console.error('Failed to review result', err);
      setError(err.response?.data?.message || 'Could not review result.');
    }
  };

  const adjudicateConflict = async (conflictGroupKey, acceptedResultId) => {
    setError('');
    try {
      await electionResultsService.adjudicateConflict(conflictGroupKey, {
        acceptedResultId,
        notes: 'Adjudicated from conflict board.'
      });
      await loadDashboardData();
    } catch (err) {
      console.error('Failed to adjudicate conflict', err);
      setError(err.response?.data?.message || 'Could not adjudicate conflict.');
    }
  };

  const downloadCsv = () => {
    const headers = ['Window', 'County', 'Candidate A', 'Candidate B', 'Candidate C', 'Rejected Votes', 'Stations Reported', 'Pending Validation'];
    const rows = aggregateRows.map((row) => [
      row.reportingWindow ?? '',
      row.county ?? 'Unspecified',
      row.candidateA ?? 0,
      row.candidateB ?? 0,
      row.candidateC ?? 0,
      row.rejectedVotes ?? 0,
      row.stationsReported ?? 0,
      row.pendingValidation ?? 0
    ]);

    const csvContent = [headers, ...rows]
      .map((cols) => cols.map((v) => `"${String(v).replace(/"/g, '""')}"`).join(','))
      .join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `election-aggregate${reportingWindow ? `-${reportingWindow}` : ''}.csv`;
    link.click();
    URL.revokeObjectURL(url);
  };

  const statusCards = [
    { label: 'Submitted', value: status?.submitted ?? 0, tone: 'bg-sky-50 text-sky-900 border-sky-200' },
    { label: 'Validated', value: status?.validated ?? 0, tone: 'bg-emerald-50 text-emerald-900 border-emerald-200' },
    { label: 'Pending Validation', value: status?.pending ?? 0, tone: 'bg-amber-50 text-amber-900 border-amber-200' },
    { label: 'Rejected', value: status?.rejected ?? 0, tone: 'bg-rose-50 text-rose-900 border-rose-200' }
  ];

  return (
    <div className="p-8 space-y-6 bg-stone-50 rounded-lg">
      <section className="rounded-3xl bg-gradient-to-r from-stone-900 via-indigo-900 to-blue-700 text-white p-8 shadow-lg">
        <p className="text-xs uppercase tracking-[0.22em] text-blue-200">Election Operations</p>
        <h2 className="text-3xl font-bold mt-2">Submit results and monitor aggregation health</h2>
        <p className="mt-3 text-sm text-blue-100 max-w-3xl">
          Polling station agents can file results, while leadership tracks validated totals, pending anomalies, and county-level aggregation.
        </p>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <h3 className="text-xl font-semibold text-stone-900">Result submission</h3>

        {error && <div className="mt-4 rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">{error}</div>}
        {feedback && <div className="mt-4 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">{feedback}</div>}

        <form onSubmit={handleSubmitResult} className="mt-5 space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-3">
            <input
              value={submissionForm.pollingStationCode}
              onChange={(event) => onSubmissionChange('pollingStationCode', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Polling station code"
              required
            />
            <input
              value={submissionForm.region}
              onChange={(event) => onSubmissionChange('region', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Region"
            />
            <input
              value={submissionForm.county}
              onChange={(event) => onSubmissionChange('county', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="County"
            />
            <input
              value={submissionForm.constituency}
              onChange={(event) => onSubmissionChange('constituency', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Constituency"
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
            <input
              value={submissionForm.latitude}
              onChange={(event) => onSubmissionChange('latitude', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Latitude (optional)"
            />
            <input
              value={submissionForm.longitude}
              onChange={(event) => onSubmissionChange('longitude', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Longitude (optional)"
            />
            <input
              value={submissionForm.deviceFingerprint}
              onChange={(event) => onSubmissionChange('deviceFingerprint', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Device fingerprint (optional)"
            />
          </div>

          <div className="grid grid-cols-1 md:grid-cols-5 gap-3">
            <input
              type="number"
              min="0"
              value={submissionForm.candidateA}
              onChange={(event) => onSubmissionChange('candidateA', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Candidate A"
              required
            />
            <input
              type="number"
              min="0"
              value={submissionForm.candidateB}
              onChange={(event) => onSubmissionChange('candidateB', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Candidate B"
              required
            />
            <input
              type="number"
              min="0"
              value={submissionForm.candidateC}
              onChange={(event) => onSubmissionChange('candidateC', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Candidate C"
              required
            />
            <input
              type="number"
              min="0"
              value={submissionForm.rejectedVotes}
              onChange={(event) => onSubmissionChange('rejectedVotes', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Rejected votes"
              required
            />
            <input
              type="number"
              min="0"
              value={submissionForm.registeredVoters}
              onChange={(event) => onSubmissionChange('registeredVoters', event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Registered voters"
              required
            />
          </div>

          <div className="flex justify-end gap-3">
            <button
              type="button"
              onClick={resetSubmission}
              className="rounded-xl border border-stone-300 px-4 py-2 text-sm font-medium text-stone-700"
            >
              Reset
            </button>
            <button
              type="submit"
              disabled={submitting}
              className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700 disabled:cursor-not-allowed disabled:bg-stone-400"
            >
              {submitting ? 'Submitting...' : 'Submit Result'}
            </button>
          </div>
        </form>
      </section>

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <h3 className="text-xl font-semibold text-stone-900">Aggregation dashboard</h3>
            <p className="text-sm text-stone-500">Filter reporting windows and county aggregates.</p>
          </div>

          <div className="flex flex-col gap-3 sm:flex-row">
            <input
              value={reportingWindow}
              onChange={(event) => setReportingWindow(event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="Reporting window (YYYYMMDD)"
            />
            <input
              value={countyFilter}
              onChange={(event) => setCountyFilter(event.target.value)}
              className="rounded-xl border border-stone-300 px-3 py-2"
              placeholder="County filter"
            />
            <button
              type="button"
              onClick={loadDashboardData}
              disabled={loading}
              className="rounded-xl bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700 disabled:cursor-not-allowed disabled:bg-stone-400"
            >
              {loading ? 'Refreshing...' : 'Refresh'}
            </button>
            <button
              type="button"
              onClick={downloadCsv}
              disabled={aggregateRows.length === 0}
              title="Export visible rows to CSV"
              className="rounded-xl border border-stone-300 px-4 py-2 text-sm font-medium text-stone-700 hover:bg-stone-100 disabled:cursor-not-allowed disabled:opacity-40"
            >
              Export CSV
            </button>
          </div>
        </div>

        <div className="mt-6 grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-3">
          {statusCards.map((card) => (
            <div key={card.label} className={`rounded-xl border p-4 ${card.tone}`}>
              <p className="text-sm">{card.label}</p>
              <p className="mt-2 text-2xl font-bold">{card.value}</p>
            </div>
          ))}
        </div>

        <div className="mt-6 overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b border-stone-200 text-left text-stone-500">
                <th className="py-3 pr-4 font-medium">Window</th>
                <th className="py-3 pr-4 font-medium">County</th>
                <th className="py-3 pr-4 font-medium">Candidate A</th>
                <th className="py-3 pr-4 font-medium">Candidate B</th>
                <th className="py-3 pr-4 font-medium">Candidate C</th>
                <th className="py-3 pr-4 font-medium">Rejected</th>
                <th className="py-3 pr-4 font-medium">Stations</th>
                <th className="py-3 font-medium">Pending Validation</th>
              </tr>
            </thead>
            <tbody>
              {aggregateRows.length === 0 ? (
                <tr>
                  <td className="py-4 text-stone-500" colSpan="8">No aggregate rows for the selected filters.</td>
                </tr>
              ) : (
                aggregateRows.map((row, index) => (
                  <tr key={`${row.reportingWindow}-${row.county || 'n-a'}-${index}`} className="border-b border-stone-100 last:border-b-0">
                    <td className="py-3 pr-4 text-stone-700">{row.reportingWindow}</td>
                    <td className="py-3 pr-4 text-stone-700">{row.county || 'Unspecified'}</td>
                    <td className="py-3 pr-4 font-medium text-stone-900">{row.candidateA}</td>
                    <td className="py-3 pr-4 font-medium text-stone-900">{row.candidateB}</td>
                    <td className="py-3 pr-4 font-medium text-stone-900">{row.candidateC}</td>
                    <td className="py-3 pr-4 text-stone-700">{row.rejectedVotes}</td>
                    <td className="py-3 pr-4 text-stone-700">{row.stationsReported}</td>
                    <td className="py-3 text-stone-700">{row.pendingValidation}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-[1fr_1fr] gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Pending validation queue</h3>
          <div className="mt-4 space-y-3 max-h-[30rem] overflow-y-auto pr-1">
            {pendingReview.length === 0 ? (
              <p className="text-sm text-stone-500">No pending anomalies in current filter.</p>
            ) : (
              pendingReview.map((item) => (
                <div key={item.id} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <div className="flex flex-wrap items-center gap-2">
                    <p className="font-semibold text-stone-900">Result #{item.id}</p>
                    {item.isConflictFlagged && <span className="rounded-full bg-rose-100 px-2 py-0.5 text-xs text-rose-700">Conflict</span>}
                    {item.isTamperSuspected && <span className="rounded-full bg-amber-100 px-2 py-0.5 text-xs text-amber-700">Low integrity</span>}
                  </div>
                  <p className="mt-1 text-xs text-stone-600">{item.pollingStationCode} • {item.county || 'Unspecified'} • {item.constituency || 'Unspecified'}</p>
                  <p className="mt-2 text-xs text-stone-600">{item.validationNotes}</p>
                  <p className="mt-1 text-xs text-stone-500">Integrity score: {item.integrityConfidenceScore ?? 'N/A'}</p>

                  <div className="mt-3 flex flex-wrap gap-2">
                    <button type="button" onClick={() => reviewResult(item.id, 'Validated')} className="rounded-lg border border-emerald-300 px-3 py-1 text-xs text-emerald-700 hover:bg-emerald-50">Validate</button>
                    <button type="button" onClick={() => reviewResult(item.id, 'Rejected')} className="rounded-lg border border-rose-300 px-3 py-1 text-xs text-rose-700 hover:bg-rose-50">Reject</button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Duplicate submitter conflicts</h3>
          <div className="mt-4 space-y-3 max-h-[30rem] overflow-y-auto pr-1">
            {conflicts.length === 0 ? (
              <p className="text-sm text-stone-500">No active conflicts for this window.</p>
            ) : (
              conflicts.map((conflict) => (
                <div key={conflict.conflictGroupKey} className="rounded-xl border border-stone-200 bg-stone-50 p-4">
                  <p className="font-semibold text-stone-900">{conflict.pollingStationCode}</p>
                  <p className="text-xs text-stone-500">{conflict.conflictGroupKey}</p>
                  <div className="mt-2 space-y-2">
                    {(conflict.submissions || []).map((item) => (
                      <div key={item.id} className="rounded-lg border border-stone-200 bg-white p-3">
                        <p className="text-xs text-stone-700">Submission #{item.id} • {item.submittedByUserId}</p>
                        <p className="text-xs text-stone-500">Integrity: {item.integrityConfidenceScore ?? 'N/A'} {item.isTamperSuspected ? '(tamper suspected)' : ''}</p>
                        <button type="button" onClick={() => adjudicateConflict(conflict.conflictGroupKey, item.id)} className="mt-2 rounded-lg border border-sky-300 px-2 py-1 text-xs text-sky-700 hover:bg-sky-50">Accept This Submission</button>
                      </div>
                    ))}
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
