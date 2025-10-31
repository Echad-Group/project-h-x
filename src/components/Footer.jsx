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
            <li>About</li>
            <li>Issues</li>
            <li>Events</li>
          </ul>
        </div>
        <div>
          <h5 className="font-semibold">Contact</h5>
          <p className="text-sm text-gray-200 mt-2">info@newkenya.org</p>
          <p className="text-sm text-gray-200">+254 700 000 000</p>
        </div>
      </div>
      <div className="mt-8 text-center text-xs text-gray-400">© {new Date().getFullYear()} New Kenya — All rights reserved</div>
    </footer>
  )
}
