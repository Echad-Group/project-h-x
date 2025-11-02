import React, { useState } from 'react';

export default function DonateModal({ open: donateOpen, setDonateOpen, onClose }) {
  const [step, setStep] = useState('form');
  const [amount, setAmount] = useState('500');
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [processing, setProcessing] = useState(false);
  const presetAmounts = ['250', '500', '1000', '2500', '5000'];

  function handleDonate(e) {
    e.preventDefault();
    setProcessing(true);
    setTimeout(() => {
      setProcessing(false);
      setStep('success');
    }, 1200);
  }

  function closeModal() {
    setDonateOpen(false);
    setStep('form');
    setName('');
    setEmail('');
    setAmount('500');
  }

  return (
    <div>
      {/*<button onClick={() => setDonateOpen(true)} className="px-4 py-2 bg-[var(--kenya-red)] text-white rounded-md">Donate</button>*/}

      {donateOpen && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white rounded-lg p-6 w-full max-w-md relative animate-fade-in">
            <button
              onClick={closeModal}
              className="absolute top-3 right-3 text-gray-400 hover:text-gray-700 text-2xl font-bold"
              aria-label="Close donation modal"
            >
              ×
            </button>
            {step === 'form' && (
              <form onSubmit={handleDonate} className="space-y-5">
                <h3 className="text-2xl font-bold text-center mb-2">Support New Kenya</h3>
                <p className="text-sm text-gray-600 text-center">Your contribution helps mobilize volunteers and reach voters.</p>
                <div>
                  <label className="block text-sm font-medium mb-1">Donation Amount (KES)</label>
                  <div className="flex gap-2 mb-2 flex-wrap">
                    {presetAmounts.map(a => (
                      <button
                        type="button"
                        key={a}
                        className={`px-3 py-2 rounded border ${amount === a ? 'bg-[var(--kenya-green)] text-white' : 'bg-gray-50 text-gray-700'}`}
                        onClick={() => setAmount(a)}
                      >
                        {a}
                      </button>
                    ))}
                    <input
                      type="number"
                      min="50"
                      step="50"
                      value={amount}
                      onChange={e => setAmount(e.target.value)}
                      className="w-24 px-2 py-1 border rounded ml-2"
                      placeholder="Other"
                      required
                    />
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Name</label>
                  <input
                    type="text"
                    value={name}
                    onChange={e => setName(e.target.value)}
                    className="w-full px-3 py-2 border rounded"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Email</label>
                  <input
                    type="email"
                    value={email}
                    onChange={e => setEmail(e.target.value)}
                    className="w-full px-3 py-2 border rounded"
                    required
                  />
                </div>
                <button
                  type="submit"
                  className="w-full bg-[var(--kenya-green)] text-white py-2 rounded font-semibold hover:bg-green-700 disabled:opacity-60"
                  disabled={processing}
                >
                  {processing ? 'Processing...' : `Donate KES ${amount}`}
                </button>
              </form>
            )}
            {step === 'success' && (
              <div className="text-center p-6">
                <div className="text-4xl mb-2 text-green-600">✓</div>
                <h3 className="text-xl font-bold mb-2">Thank you for your support!</h3>
                <p className="text-gray-700 mb-4">Your (mock) donation of <span className="font-semibold">KES {amount}</span> has been received.</p>
                <button
                  onClick={closeModal}
                  className="mt-2 px-5 py-2 bg-[var(--kenya-green)] text-white rounded font-semibold hover:bg-green-700"
                >
                  Close
                </button>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
