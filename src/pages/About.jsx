import React from 'react'

export default function About(){
  return (
    <section className="max-w-4xl mx-auto py-16 px-4">
      <h1 className="text-3xl font-extrabold brand-gradient">About the Candidate</h1>
      <p className="mt-6 text-lg text-gray-700">This is a modern, dynamic campaign — focused on renewing institutions, investing in youth, and delivering inclusive growth. The candidate brings public service experience, a clear plan for economic transformation, and a commitment to national unity.</p>
      <div className="mt-8 grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="p-6 bg-white card-shadow rounded-lg">
          <h3 className="font-semibold">Vision</h3>
          <p className="mt-2 text-sm text-gray-600">A New Kenya — united, prosperous and secure for every citizen.</p>
        </div>
        <div className="p-6 bg-white card-shadow rounded-lg">
          <h3 className="font-semibold">Track Record</h3>
          <p className="mt-2 text-sm text-gray-600">Public leadership across sectors with a focus on results and transparency.</p>
        </div>
      </div>
    </section>
  )
}
