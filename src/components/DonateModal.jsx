import React, { useState, useEffect, useRef } from 'react';
import { useTranslation } from 'react-i18next'

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

  // Accessibility: focus trapping and restore
  const modalRef = useRef(null);
  const previouslyFocused = useRef(null);

  useEffect(() => {
    if (!donateOpen) return;

    previouslyFocused.current = document.activeElement;

    // focus the first focusable element in the modal
    const focusableSelector = 'a[href], area[href], input:not([disabled]), select:not([disabled]), textarea:not([disabled]), button:not([disabled]), [tabindex]:not([tabindex="-1"])';
    const node = modalRef.current;
    const focusable = node ? Array.from(node.querySelectorAll(focusableSelector)) : [];
    const first = focusable.length > 0 ? focusable[0] : node;

    const prevOverflow = document.body.style.overflow;
    document.body.style.overflow = 'hidden';

    const focusTimer = setTimeout(() => {
      if (first && first.focus) first.focus();
    }, 0);

    function onKeyDown(e) {
      if (e.key === 'Escape') {
        e.preventDefault();
        closeModal();
        return;
      }

      if (e.key === 'Tab') {
        // trap focus
        const focusableElems = focusable.length ? focusable : [node];
        if (focusableElems.length === 0) return;
        const idx = focusableElems.indexOf(document.activeElement);
        if (e.shiftKey) {
          // backwards
          if (idx === 0 || document.activeElement === node) {
            e.preventDefault();
            focusableElems[focusableElems.length - 1].focus();
          }
        } else {
          // forwards
          if (idx === focusableElems.length - 1) {
            e.preventDefault();
            focusableElems[0].focus();
          }
        }
      }
    }

  document.addEventListener('keydown', onKeyDown);

    return () => {
      clearTimeout(focusTimer);
      document.removeEventListener('keydown', onKeyDown);
      document.body.style.overflow = prevOverflow;
      // restore focus
      try {
        if (previouslyFocused.current && previouslyFocused.current.focus) previouslyFocused.current.focus();
      } catch (err) {
        // ignore
      }
    };
  }, [donateOpen]);

  const { t } = useTranslation();

  return (
    <div>
      {/*<button onClick={() => setDonateOpen(true)} className="px-4 py-2 bg-[var(--kenya-red)] text-white rounded-md">Donate</button>*/}

      {donateOpen && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
          onMouseDown={(e) => { if (e.target === e.currentTarget) closeModal(); }}
        >
          <div ref={modalRef} role="dialog" aria-modal="true" aria-labelledby="donate-heading" aria-describedby="donate-desc" className="bg-white rounded-lg p-6 w-full max-w-md relative animate-fade-in">
            <button
              onClick={closeModal}
              className="absolute top-3 right-3 text-gray-400 hover:text-gray-700 text-2xl font-bold"
              aria-label="Close donation modal"
            >
              ×
            </button>
            {step === 'form' && (
              <form onSubmit={handleDonate} className="space-y-5">
                <h3 id="donate-heading" className="text-2xl font-bold text-center mb-2">{t('donate.heading')}</h3>
                <p id="donate-desc" className="text-sm text-gray-600 text-center">{t('donate.description')}</p>
                
                {/* Recurring Options */}
                <div className="flex gap-2 justify-center">
                  {recurringOptions.map(option => (
                    <button
                      key={option.id}
                      type="button"
                      className={`fluent-btn ${recurringInterval === option.id ? 'fluent-btn-primary' : 'fluent-btn-ghost'}`}
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
                          className={`fluent-btn ${amount === a ? 'fluent-btn-primary' : 'fluent-btn-ghost'}`}
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
                      className="w-24 fluent-input ml-2"
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
                    className="fluent-input"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Email</label>
                  <input
                    type="email"
                    value={email}
                    onChange={e => setEmail(e.target.value)}
                    className="fluent-input"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Phone</label>
                  <input
                    type="tel"
                    value={phone}
                    onChange={e => setPhone(e.target.value)}
                    className="fluent-input"
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
                        className={`fluent-card-small flex flex-col items-center gap-1 ${paymentMethod === method.id ? 'border-[var(--fluent-accent)]' : ''}`}
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
                  className="fluent-btn fluent-btn-primary w-full"
                  disabled={processing}
                >
                  {processing ? (t('donate.processing') || 'Processing...') : (t('donate.give') ? `${t('donate.give')} KES ${amount}` : `Donate KES ${amount}`)}
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
                  className="fluent-btn fluent-btn-primary mt-2"
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
