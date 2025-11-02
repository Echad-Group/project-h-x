import React from 'react';
import VolunteerSignup from '../components/VolunteerSignup';

export default function GetInvolved() {
  return (
    <section id="get-involved" className="max-w-3xl mx-auto px-4 py-12">
      <h1 className="text-3xl font-bold">Get Involved</h1>
      <p className="mt-3 text-gray-600">Volunteer, host an event, or join a local chapter.</p>
      <div className="mt-6">
        <VolunteerSignup />
      </div>
    </section>
  );
}
