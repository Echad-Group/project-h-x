import React from 'react'

export default function Footer(){
  return (
    <footer className="bg-[var(--kenya-black)] text-white py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div>
          <h4 className="text-lg font-semibold">New Kenya</h4>
          <p className="text-sm text-gray-200 mt-2">A campaign for every Kenyan — unity, growth, and opportunity.</p>
        </div>
        <div>
          <h5 className="font-semibold">Quick Links</h5>
          <ul className="mt-2 text-sm text-gray-200 space-y-1">
            <li><a href="/about">About</a></li>
            <li><a href="/issues">Issues</a></li>
            <li><a href="/events">Events</a></li>
            <li><a href="/news">News</a></li>
            <li><a href="/get-involved">Get Involved</a></li>
          </ul>
        </div>
        <div>
          <h5 className="font-semibold">Contact</h5>
          <p className="text-sm text-gray-200 mt-2"><a href="mailto:info@newkenya.org" tagert="_blank">info@newkenya.org</a></p>
          <p className="text-sm text-gray-200"><a href="tel:+254700000000" target="_blank">+254 700 000 000</a></p>
        </div>
      </div>
      <div className="mt-8 text-center text-xs text-gray-400">© {new Date().getFullYear()} New Kenya — All rights reserved</div>
    </footer>
  )
}
