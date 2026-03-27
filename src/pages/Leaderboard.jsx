import React, { useEffect, useState } from 'react';
import { leaderboardService } from '../services/campaignCommandService';

export default function Leaderboard() {
  const [scope, setScope] = useState('National');
  const [region, setRegion] = useState('');
  const [county, setCounty] = useState('');
  const [rows, setRows] = useState([]);
  const [myRank, setMyRank] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadLeaderboard();
  }, [scope]);

  async function loadLeaderboard() {
    try {
      setLoading(true);
      setError('');

      const [scores, rank] = await Promise.all([
        leaderboardService.get({
          scope,
          region: region || undefined,
          county: county || undefined
        }),
        leaderboardService.getMyRank({ scope }).catch(() => null)
      ]);

      setRows(Array.isArray(scores) ? scores : []);
      setMyRank(rank);
    } catch (err) {
      console.error('Failed to load leaderboard', err);
      setError(err.response?.data?.message || 'Unable to load leaderboard.');
    } finally {
      setLoading(false);
    }
  }

  if (loading) {
    return <div className="p-8 text-sm text-stone-600">Loading leaderboard...</div>;
  }

  return (
    <div className="min-h-screen bg-stone-50 p-[5%] space-y-6">
      <section className="rounded-2xl bg-gradient-to-r from-amber-700 via-orange-600 to-rose-600 p-8 text-white shadow-lg">
        <h1 className="text-3xl font-bold">Campaign Leaderboard</h1>
        <p className="mt-2 text-sm text-amber-100">Track national/regional performance, badge tiers, and your rank journey.</p>
      </section>

      {error && <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">{error}</div>}

      {myRank && (
        <section className="rounded-2xl border border-amber-200 bg-amber-50 p-6 shadow-sm">
          <p className="text-xs uppercase tracking-[0.2em] text-amber-700">My Rank Journey</p>
          <p className="mt-2 text-3xl font-bold text-amber-900">#{myRank.rank} of {myRank.totalParticipants}</p>
          <p className="mt-2 text-sm text-amber-800">{myRank.badgeTier} • {myRank.recognitionTitle}</p>
          <p className="text-sm text-amber-700">Incentive: {myRank.incentiveTag}</p>
        </section>
      )}

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex flex-wrap gap-3 items-end">
          <div>
            <label className="block text-xs text-stone-500 mb-1">Scope</label>
            <select value={scope} onChange={(event) => setScope(event.target.value)} className="rounded-lg border border-stone-300 px-3 py-2 text-sm">
              <option value="National">National</option>
            </select>
          </div>
          <div>
            <label className="block text-xs text-stone-500 mb-1">Region</label>
            <input value={region} onChange={(event) => setRegion(event.target.value)} className="rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Optional" />
          </div>
          <div>
            <label className="block text-xs text-stone-500 mb-1">County</label>
            <input value={county} onChange={(event) => setCounty(event.target.value)} className="rounded-lg border border-stone-300 px-3 py-2 text-sm" placeholder="Optional" />
          </div>
          <button type="button" onClick={loadLeaderboard} className="rounded-lg bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700">Refresh</button>
        </div>

        <div className="mt-6 overflow-x-auto">
          <table className="min-w-full text-sm">
            <thead>
              <tr className="border-b border-stone-200 text-left text-stone-500">
                <th className="py-3 pr-4">Rank</th>
                <th className="py-3 pr-4">Member</th>
                <th className="py-3 pr-4">Points</th>
                <th className="py-3 pr-4">Badge</th>
                <th className="py-3 pr-4">Title</th>
                <th className="py-3">Region / County</th>
              </tr>
            </thead>
            <tbody>
              {rows.length === 0 ? (
                <tr>
                  <td colSpan="6" className="py-4 text-stone-500">No leaderboard rows found.</td>
                </tr>
              ) : (
                rows.map((row, index) => (
                  <tr key={row.id || `${row.userId}-${index}`} className="border-b border-stone-100 last:border-b-0">
                    <td className="py-3 pr-4 font-semibold text-stone-900">#{index + 1}</td>
                    <td className="py-3 pr-4 text-stone-800">{row.fullName || 'Unknown'}</td>
                    <td className="py-3 pr-4 text-stone-900 font-semibold">{row.totalPoints}</td>
                    <td className="py-3 pr-4 text-stone-700">{row.badgeTier || '-'}</td>
                    <td className="py-3 pr-4 text-stone-700">{row.recognitionTitle || '-'}</td>
                    <td className="py-3 text-stone-600">{row.region || '-'} / {row.county || '-'}</td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>
    </div>
  );
}
