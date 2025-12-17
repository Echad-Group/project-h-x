import React, { useState } from 'react';

export default function AdminEmails() {
  const [emailForm, setEmailForm] = useState({
    recipients: 'all',
    subject: '',
    message: '',
    unit: '',
    team: '',
    region: ''
  });
  const [sending, setSending] = useState(false);
  const [sentEmails, setSentEmails] = useState([
    {
      id: 1,
      subject: 'Welcome to New Kenya Movement',
      recipients: 245,
      sentAt: '2025-12-15T10:30:00',
      status: 'sent'
    },
    {
      id: 2,
      subject: 'Upcoming Town Hall Meeting',
      recipients: 180,
      sentAt: '2025-12-14T14:20:00',
      status: 'sent'
    }
  ]);

  async function handleSendEmail(e) {
    e.preventDefault();
    setSending(true);
    
    try {
      // TODO: Implement actual email sending
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      alert('Email sent successfully!');
      setEmailForm({
        recipients: 'all',
        subject: '',
        message: '',
        unit: '',
        team: '',
        region: ''
      });
    } catch (error) {
      console.error('Error sending email:', error);
      alert('Failed to send email');
    } finally {
      setSending(false);
    }
  }

  function getRecipientCount() {
    // TODO: Calculate based on filters
    switch (emailForm.recipients) {
      case 'all':
        return 500;
      case 'volunteers':
        return 350;
      case 'unit':
        return 80;
      case 'team':
        return 25;
      case 'region':
        return 120;
      default:
        return 0;
    }
  }

  return (
    <div className="p-8">
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Email Composition */}
        <div className="lg:col-span-2">
          <div className="bg-white border rounded-lg p-6">
            <h3 className="text-xl font-bold text-gray-900 mb-4">Compose Email Update</h3>
            <form onSubmit={handleSendEmail} className="space-y-4">
              <div>
                <label className="block text-sm font-medium mb-1">Recipients</label>
                <select
                  className="fluent-input"
                  value={emailForm.recipients}
                  onChange={e => setEmailForm({ ...emailForm, recipients: e.target.value })}
                >
                  <option value="all">All Subscribers</option>
                  <option value="volunteers">All Volunteers</option>
                  <option value="unit">Specific Unit</option>
                  <option value="team">Specific Team</option>
                  <option value="region">Specific Region</option>
                </select>
              </div>

              {emailForm.recipients === 'unit' && (
                <div>
                  <label className="block text-sm font-medium mb-1">Select Unit</label>
                  <select
                    className="fluent-input"
                    value={emailForm.unit}
                    onChange={e => setEmailForm({ ...emailForm, unit: e.target.value })}
                  >
                    <option value="">Choose unit...</option>
                    <option value="1">Communications Unit</option>
                    <option value="2">Security & Integrity Unit</option>
                    <option value="3">Outreach Unit</option>
                  </select>
                </div>
              )}

              {emailForm.recipients === 'team' && (
                <div>
                  <label className="block text-sm font-medium mb-1">Select Team</label>
                  <select
                    className="fluent-input"
                    value={emailForm.team}
                    onChange={e => setEmailForm({ ...emailForm, team: e.target.value })}
                  >
                    <option value="">Choose team...</option>
                    <option value="1">Social Media Team</option>
                    <option value="2">Field Mobilization Team</option>
                    <option value="3">Events Team</option>
                  </select>
                </div>
              )}

              {emailForm.recipients === 'region' && (
                <div>
                  <label className="block text-sm font-medium mb-1">Select Region</label>
                  <select
                    className="fluent-input"
                    value={emailForm.region}
                    onChange={e => setEmailForm({ ...emailForm, region: e.target.value })}
                  >
                    <option value="">Choose region...</option>
                    <option value="Nairobi">Nairobi</option>
                    <option value="Mombasa">Mombasa</option>
                    <option value="Kisumu">Kisumu</option>
                  </select>
                </div>
              )}

              <div className="bg-blue-50 border border-blue-200 rounded p-3">
                <p className="text-sm text-blue-800">
                  This email will be sent to approximately <strong>{getRecipientCount()}</strong> recipients
                </p>
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Subject</label>
                <input
                  type="text"
                  className="fluent-input"
                  value={emailForm.subject}
                  onChange={e => setEmailForm({ ...emailForm, subject: e.target.value })}
                  placeholder="Email subject..."
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium mb-1">Message</label>
                <textarea
                  className="fluent-input"
                  rows={10}
                  value={emailForm.message}
                  onChange={e => setEmailForm({ ...emailForm, message: e.target.value })}
                  placeholder="Compose your message here..."
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  Tip: Keep messages clear and concise. Include a call-to-action.
                </p>
              </div>

              <div className="flex gap-3">
                <button
                  type="submit"
                  className="flex-1 fluent-btn fluent-btn-primary"
                  disabled={sending}
                >
                  {sending ? 'Sending...' : '📧 Send Email'}
                </button>
                <button
                  type="button"
                  className="fluent-btn fluent-btn-ghost"
                  onClick={() => setEmailForm({
                    recipients: 'all',
                    subject: '',
                    message: '',
                    unit: '',
                    team: '',
                    region: ''
                  })}
                >
                  Clear
                </button>
              </div>
            </form>
          </div>
        </div>

        {/* Email Templates & History */}
        <div className="space-y-6">
          {/* Quick Templates */}
          <div className="bg-white border rounded-lg p-6">
            <h3 className="text-lg font-bold text-gray-900 mb-4">Quick Templates</h3>
            <div className="space-y-2">
              <button
                onClick={() => setEmailForm({
                  ...emailForm,
                  subject: 'Event Reminder',
                  message: 'Dear supporter,\n\nThis is a friendly reminder about our upcoming event...'
                })}
                className="w-full text-left p-3 border rounded hover:bg-gray-50 transition-colors"
              >
                <p className="font-medium text-sm">📅 Event Reminder</p>
                <p className="text-xs text-gray-600">Notify about upcoming events</p>
              </button>
              <button
                onClick={() => setEmailForm({
                  ...emailForm,
                  subject: 'Weekly Update',
                  message: 'Dear team,\n\nHere\'s what happened this week...'
                })}
                className="w-full text-left p-3 border rounded hover:bg-gray-50 transition-colors"
              >
                <p className="font-medium text-sm">📰 Weekly Update</p>
                <p className="text-xs text-gray-600">Share weekly highlights</p>
              </button>
              <button
                onClick={() => setEmailForm({
                  ...emailForm,
                  subject: 'Volunteer Opportunity',
                  message: 'We have an exciting opportunity for volunteers...'
                })}
                className="w-full text-left p-3 border rounded hover:bg-gray-50 transition-colors"
              >
                <p className="font-medium text-sm">🤝 Volunteer Call</p>
                <p className="text-xs text-gray-600">Request volunteer help</p>
              </button>
            </div>
          </div>

          {/* Sent Emails */}
          <div className="bg-white border rounded-lg p-6">
            <h3 className="text-lg font-bold text-gray-900 mb-4">Recent Emails</h3>
            <div className="space-y-3">
              {sentEmails.map(email => (
                <div key={email.id} className="border rounded p-3">
                  <div className="flex items-start justify-between mb-2">
                    <p className="font-medium text-sm text-gray-900">{email.subject}</p>
                    <span className="px-2 py-1 bg-green-100 text-green-700 text-xs rounded">
                      {email.status}
                    </span>
                  </div>
                  <p className="text-xs text-gray-600">
                    {email.recipients} recipients
                  </p>
                  <p className="text-xs text-gray-500 mt-1">
                    {new Date(email.sentAt).toLocaleString()}
                  </p>
                </div>
              ))}
            </div>
          </div>

          {/* Email Stats */}
          <div className="bg-white border rounded-lg p-6">
            <h3 className="text-lg font-bold text-gray-900 mb-4">Email Statistics</h3>
            <div className="space-y-3">
              <div>
                <div className="flex justify-between text-sm mb-1">
                  <span className="text-gray-600">Open Rate</span>
                  <span className="font-medium">68%</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div className="bg-green-500 h-2 rounded-full" style={{ width: '68%' }}></div>
                </div>
              </div>
              <div>
                <div className="flex justify-between text-sm mb-1">
                  <span className="text-gray-600">Click Rate</span>
                  <span className="font-medium">24%</span>
                </div>
                <div className="w-full bg-gray-200 rounded-full h-2">
                  <div className="bg-blue-500 h-2 rounded-full" style={{ width: '24%' }}></div>
                </div>
              </div>
              <div className="pt-3 border-t">
                <p className="text-sm text-gray-600">Total Sent</p>
                <p className="text-2xl font-bold">1,247</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
