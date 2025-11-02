import React, { useState } from 'react';

export default function DonateModal({ open: donateOpen, setDonateOpen, onClose }) {
  const [step, setStep] = useState('form');
  const [amount, setAmount] = useState('500');
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
   const [phone, setPhone] = useState('');
   const [paymentMethod, setPaymentMethod] = useState('mpesa');
   const [recurringInterval, setRecurringInterval] = useState('once');
   const [processing, setProcessing] = useState(false);
   
   const presetAmounts = ['250', '500', '1000', '2500', '5000'];
   
   const paymentMethods = [
     { id: 'mpesa', name: 'M-Pesa', icon: '📱' },
     { id: 'card', name: 'Credit/Debit Card', icon: '💳' },
     { id: 'bank', name: 'Bank Transfer', icon: '🏦' }
   ];
   
   const recurringOptions = [
     { id: 'once', name: 'One-time' },
     { id: 'monthly', name: 'Monthly' },
     { id: 'quarterly', name: 'Quarterly' },
     { id: 'annually', name: 'Annually' }
   ];

  function handleDonate(e) {
    e.preventDefault();
    setProcessing(true);
    
    // Simulate payment processing
    const paymentData = {
      amount,
      name,
      email,
      phone,
      paymentMethod,
      recurringInterval,
      currency: 'KES',
      timestamp: new Date().toISOString()
    };

    // In production, this would call your payment API
    console.log('Processing payment:', paymentData);
    
    setTimeout(() => {
      setProcessing(false);
      setStep('success');
      // Reset form
      setAmount('500');
      setName('');
      setEmail('');
      setPhone('');
      setPaymentMethod('mpesa');
      setRecurringInterval('once');
    }, 2000);
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
                
                {/* Recurring Options */}
                <div className="flex gap-2 justify-center">
                  {recurringOptions.map(option => (
                    <button
                      key={option.id}
                      type="button"
                      className={`px-3 py-1 rounded text-sm ${
                        recurringInterval === option.id 
                          ? 'bg-[var(--kenya-green)] text-white' 
                          : 'bg-gray-100 text-gray-700'
                      }`}
                      onClick={() => setRecurringInterval(option.id)}
                    >
                      {option.name}
                    </button>
                  ))}
                </div>

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
                   <p className="text-xs text-gray-500 mt-1">
                     {recurringInterval !== 'once' ? `You will be charged KES ${amount} ${recurringInterval}` : ''}
                   </p>
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
                <div>
                  <label className="block text-sm font-medium mb-1">Phone</label>
                  <input
                    type="tel"
                    value={phone}
                    onChange={e => setPhone(e.target.value)}
                    className="w-full px-3 py-2 border rounded"
                    placeholder="+254"
                    required
                  />
                </div>
                
                {/* Payment Methods */}
                <div>
                  <label className="block text-sm font-medium mb-2">Payment Method</label>
                  <div className="grid grid-cols-3 gap-2">
                    {paymentMethods.map(method => (
                      <button
                        key={method.id}
                        type="button"
                        className={`p-3 rounded border flex flex-col items-center gap-1 ${
                          paymentMethod === method.id 
                            ? 'bg-[var(--kenya-green)]/10 border-[var(--kenya-green)]' 
                            : 'bg-gray-50 border-gray-200'
                        }`}
                        onClick={() => setPaymentMethod(method.id)}
                      >
                        <span className="text-xl">{method.icon}</span>
                        <span className="text-xs font-medium">{method.name}</span>
                      </button>
                    ))}
                  </div>
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
