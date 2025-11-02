import React, { useState } from 'react';
import { Link, NavLink } from 'react-router-dom'
import LanguageSwitcher from './LanguageSwitcher'
import { Link as RouterLink } from 'react-router-dom'


const NavItem = ({to, children}) => (
  <NavLink to={to} className={({isActive}) => `px-3 py-2 rounded-md text-sm font-medium ${isActive ? 'bg-[var(--kenya-green)]/10 text-[var(--kenya-green)]' : 'text-gray-700 hover:text-[var(--kenya-green)]'}`}>
    {children}
  </NavLink>
)

export default function Navbar({ onOpenDonate }) {
  return (
    <header className="sticky top-0 z-40 backdrop-blur bg-white/60 border-b">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex items-center gap-4">
            <Link to="/" className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-full overflow-hidden shadow-sm">
                <svg viewBox="0 0 3 2" className="w-full h-full">
                  <rect width="3" height="2" fill="#006633"/>
                  <rect y="0.6" width="3" height="0.4" fill="#CE1126"/>
                  <rect y="0.9" width="3" height="0.2" fill="#000000"/>
                </svg>
              </div>
              <div>
                <div className="text-lg font-bold">New Kenya</div>
                <div className="text-xs text-gray-500">For a united, prosperous future</div>
              </div>
            </Link>
          </div>

          <nav className="hidden md:flex items-center gap-2" aria-label="Primary Navigation">
            <NavItem to="/">Home</NavItem>
            <NavItem to="/about">About</NavItem>
            <NavItem to="/issues">Issues</NavItem>
            <NavItem to="/events">Events</NavItem>
            <NavItem to="/get-involved">Get Involved</NavItem>
            {/* <LanguageSwitcher /> */}
            <button
              className="ml-4 px-4 py-2 rounded-md text-sm font-semibold text-white bg-[var(--kenya-red)] hover:opacity-95"
              onClick={() => onOpenDonate()}
              aria-label="Open donation modal"
            >
              Donate
            </button>
            {/* <RouterLink to="/shop" className="ml-2 px-3 py-2 rounded-md text-sm font-medium bg-gray-100">Shop</RouterLink> */}
          </nav>
        </div>
      </div>
    </header>
  );
}
