import React, { useEffect, useMemo, useState } from 'react';
import { adminEmailArchiveService } from '../../services/adminEmailArchiveService';

function formatDate(value) {
  if (!value) {
    return '-';
  }

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return value;
  }

  return date.toLocaleString();
}

export default function AdminEmails() {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [emails, setEmails] = useState([]);
  const [selectedEmailKey, setSelectedEmailKey] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');

  const getEmailKey = (email) => `${email.sentAtUtc || ''}|${email.to || ''}|${email.subject || ''}|${email.deliveryStatus || ''}`;

  const loadArchive = async () => {
    setLoading(true);
    setError('');

    try {
      const data = await adminEmailArchiveService.list(300);
      const sorted = Array.isArray(data)
        ? [...data].sort((a, b) => new Date(b.sentAtUtc) - new Date(a.sentAtUtc))
        : [];

      setEmails(sorted);
      setSelectedEmailKey(sorted.length ? getEmailKey(sorted[0]) : '');
    } catch (err) {
      const message = err.response?.data?.message || 'Could not load archived emails.';
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadArchive();
  }, []);

  const filteredEmails = useMemo(() => {
    const query = searchQuery.trim().toLowerCase();

    return emails.filter((email) => {
      const normalizedStatus = (email.deliveryStatus || '').toLowerCase();
      if (statusFilter !== 'all' && normalizedStatus !== statusFilter) {
        return false;
      }

      if (!query) {
        return true;
      }

      const subject = (email.subject || '').toLowerCase();
      const recipient = (email.to || '').toLowerCase();
      return subject.includes(query) || recipient.includes(query);
    });
  }, [emails, searchQuery, statusFilter]);

  useEffect(() => {
    if (!filteredEmails.length) {
      setSelectedEmailKey('');
      return;
    }

    const hasSelected = filteredEmails.some((email) => getEmailKey(email) === selectedEmailKey);
    if (!hasSelected) {
      setSelectedEmailKey(getEmailKey(filteredEmails[0]));
    }
  }, [filteredEmails, selectedEmailKey]);

  const selectedEmail = useMemo(() => {
    if (!filteredEmails.length) {
      return null;
    }

    return filteredEmails.find((email) => getEmailKey(email) === selectedEmailKey) || filteredEmails[0];
  }, [filteredEmails, selectedEmailKey]);

  const handleRefresh = async () => {
    await loadArchive();
  };

  return (
    <div className="p-6 lg:p-8">
      <div className="flex items-center justify-between mb-5">
        <div>
          <h3 className="text-2xl font-bold text-gray-900">Archived Emails</h3>
          <p className="text-sm text-gray-600 mt-1">View every backend email record captured in email-archive.json.</p>
        </div>
        <button
          type="button"
          onClick={handleRefresh}
          className="fluent-btn fluent-btn-ghost"
          disabled={loading}
        >
          {loading ? 'Refreshing...' : 'Refresh'}
        </button>
      </div>

      {error && (
        <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-sm">
          {error}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-5 gap-6">
        <section className="lg:col-span-2 border rounded-xl overflow-hidden bg-white">
          <div className="px-4 py-3 border-b bg-gray-50">
            <div className="flex items-center justify-between mb-3">
              <p className="font-semibold text-gray-800">Email List</p>
              <span className="text-xs text-gray-500">{filteredEmails.length} / {emails.length}</span>
            </div>

            <div className="space-y-2">
              <input
                type="text"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="fluent-input"
                placeholder="Search by recipient or subject"
              />
              <select
                value={statusFilter}
                onChange={(e) => setStatusFilter(e.target.value)}
                className="fluent-input"
              >
                <option value="all">All statuses</option>
                <option value="sent">Sent</option>
                <option value="skipped">Skipped</option>
                <option value="failed">Failed</option>
              </select>
            </div>
          </div>

          <div className="max-h-[520px] overflow-auto divide-y">
            {!loading && filteredEmails.length === 0 && (
              <div className="p-4 text-sm text-gray-600">No emails match the current filters.</div>
            )}

            {loading && (
              <div className="p-4 text-sm text-gray-600">Loading email archive...</div>
            )}

            {!loading && filteredEmails.map((email) => {
              const emailKey = getEmailKey(email);
              return (
                <button
                  key={emailKey}
                  type="button"
                  onClick={() => setSelectedEmailKey(emailKey)}
                  className={`w-full text-left p-4 hover:bg-gray-50 transition-colors ${emailKey === selectedEmailKey ? 'bg-green-50' : ''}`}
                >
                  <div className="flex items-center justify-between gap-2">
                    <p className="font-medium text-sm text-gray-900 truncate">{email.subject || '(No Subject)'}</p>
                    <span className={`text-[11px] px-2 py-1 rounded-full ${
                      email.deliveryStatus === 'sent'
                        ? 'bg-emerald-100 text-emerald-700'
                        : email.deliveryStatus === 'failed'
                          ? 'bg-red-100 text-red-700'
                          : 'bg-amber-100 text-amber-700'
                    }`}>
                      {email.deliveryStatus || 'unknown'}
                    </span>
                  </div>
                  <p className="text-xs text-gray-600 truncate mt-1">To: {email.to}</p>
                  <p className="text-xs text-gray-500 mt-1">{formatDate(email.sentAtUtc)}</p>
                </button>
              );
            })}
          </div>
        </section>

        <section className="lg:col-span-3 border rounded-xl bg-white overflow-hidden">
          <div className="px-4 py-3 border-b bg-gray-50">
            <p className="font-semibold text-gray-800">Email Details</p>
          </div>

          {!selectedEmail && !loading && (
            <div className="p-6 text-sm text-gray-600">Select an email to view full details.</div>
          )}

          {selectedEmail && (
            <div className="p-5 space-y-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-sm">
                <div>
                  <p className="text-gray-500">To</p>
                  <p className="font-medium break-all">{selectedEmail.to}</p>
                </div>
                <div>
                  <p className="text-gray-500">Sent At (UTC)</p>
                  <p className="font-medium">{formatDate(selectedEmail.sentAtUtc)}</p>
                </div>
                <div>
                  <p className="text-gray-500">From</p>
                  <p className="font-medium">{selectedEmail.fromName} &lt;{selectedEmail.fromEmail}&gt;</p>
                </div>
                <div>
                  <p className="text-gray-500">Delivery Status</p>
                  <p className="font-medium">{selectedEmail.deliveryStatus || 'unknown'}</p>
                </div>
                <div>
                  <p className="text-gray-500">SMTP Configured</p>
                  <p className="font-medium">{selectedEmail.smtpConfigured ? 'Yes' : 'No'}</p>
                </div>
                <div>
                  <p className="text-gray-500">Subject</p>
                  <p className="font-medium break-words">{selectedEmail.subject || '(No Subject)'}</p>
                </div>
              </div>

              {selectedEmail.error && (
                <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded text-sm">
                  Error: {selectedEmail.error}
                </div>
              )}

              <div>
                <p className="text-sm font-medium text-gray-700 mb-2">HTML Body</p>
                <pre className="bg-gray-900 text-gray-100 text-xs rounded-lg p-4 overflow-auto max-h-[340px] whitespace-pre-wrap break-words">
                  {selectedEmail.htmlBody}
                </pre>
              </div>
            </div>
          )}
        </section>
      </div>
    </div>
  );
}
