import React, { useEffect, useState } from 'react';
import { geolocationService } from '../../services/campaignCommandService';

export default function AdminCoverageMap() {
  const [hours, setHours] = useState(24);
  const [coverage, setCoverage] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadCoverage();
  }, []);

  async function loadCoverage() {
    try {
      setLoading(true);
      setError('');
      const data = await geolocationService.getCoverage({ hours });
      setCoverage(data);
    } catch (err) {
      console.error('Failed to load coverage map data', err);
      setError(err.response?.data?.message || 'Unable to load coverage metrics.');
    } finally {
      setLoading(false);
    }
  }

  if (loading) {
    return <div className="p-8 text-sm text-stone-600">Loading field coverage map...</div>;
  }

  return (
    <div className="p-8 space-y-6 bg-stone-50 rounded-lg">
      <section className="rounded-2xl bg-gradient-to-r from-cyan-900 via-sky-700 to-emerald-700 p-8 text-white shadow-lg">
        <h2 className="text-3xl font-bold">Field Deployment Coverage Map</h2>
        <p className="mt-2 text-sm text-cyan-100">Geolocation ingestion KPIs by region/county with privacy-aware anonymization support.</p>
      </section>

      {error && <div className="rounded-lg border border-red-200 bg-red-50 p-3 text-sm text-red-700">{error}</div>}

      <section className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
        <div className="flex items-center justify-between gap-3">
          <h3 className="text-xl font-semibold text-stone-900">Coverage window controls</h3>
          <div className="flex items-center gap-2">
            <input type="number" min="1" max="168" value={hours} onChange={(event) => setHours(Number(event.target.value || 24))} className="w-24 rounded-lg border border-stone-300 px-3 py-2 text-sm" />
            <button type="button" onClick={loadCoverage} className="rounded-lg bg-stone-900 px-4 py-2 text-sm font-semibold text-white hover:bg-stone-700">Refresh</button>
          </div>
        </div>

        <div className="mt-4 grid grid-cols-1 md:grid-cols-3 gap-3">
          <div className="rounded-lg border border-stone-200 bg-stone-50 p-4">
            <p className="text-sm text-stone-500">Total pings</p>
            <p className="text-3xl font-bold text-stone-900">{coverage?.totalPings ?? 0}</p>
          </div>
          <div className="rounded-lg border border-stone-200 bg-stone-50 p-4">
            <p className="text-sm text-stone-500">Regions covered</p>
            <p className="text-3xl font-bold text-stone-900">{(coverage?.regionCoverage || []).length}</p>
          </div>
          <div className="rounded-lg border border-stone-200 bg-stone-50 p-4">
            <p className="text-sm text-stone-500">County clusters</p>
            <p className="text-3xl font-bold text-stone-900">{(coverage?.countyCoverage || []).length}</p>
          </div>
        </div>
      </section>

      <section className="grid grid-cols-1 xl:grid-cols-2 gap-6">
        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">Region overlays</h3>
          <div className="mt-4 space-y-2 max-h-[28rem] overflow-y-auto pr-1">
            {(coverage?.regionCoverage || []).map((row) => (
              <div key={row.region} className="rounded-lg border border-stone-200 bg-stone-50 px-3 py-2 text-sm flex items-center justify-between">
                <span>{row.region}</span>
                <span className="text-stone-700">Pings {row.pings} • Users {row.uniqueUsers}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-2xl border border-stone-200 bg-white p-6 shadow-sm">
          <h3 className="text-xl font-semibold text-stone-900">County overlays</h3>
          <div className="mt-4 space-y-2 max-h-[28rem] overflow-y-auto pr-1">
            {(coverage?.countyCoverage || []).map((row, index) => (
              <div key={`${row.region}-${row.county}-${index}`} className="rounded-lg border border-stone-200 bg-stone-50 px-3 py-2 text-sm">
                <p className="font-semibold text-stone-900">{row.region} / {row.county}</p>
                <p className="text-xs text-stone-600">Pings {row.pings} • Unique users {row.uniqueUsers}</p>
              </div>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
}
