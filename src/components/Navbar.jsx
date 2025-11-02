import React, { useState } from 'react';
import { Link, NavLink } from 'react-router-dom'
import LanguageSwitcher from './LanguageSwitcher'
import { Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'


const NavItem = ({to, children}) => (
  <NavLink to={to} className={({isActive}) => `px-3 py-2 rounded-md text-sm font-medium ${isActive ? 'bg-[var(--kenya-green)]/10 text-[var(--kenya-green)]' : 'text-gray-700 hover:text-[var(--kenya-green)]'}`}>
    {children}
  </NavLink>
)

export default function Navbar({ onOpenDonate }) {
  const { t } = useTranslation()
  return (
    <header className="sticky top-0 z-40 backdrop-blur bg-white/60 border-b">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex items-center gap-4">
            <Link to="/" className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-full overflow-hidden shadow-sm">
                {/* <svg viewBox="0 0 3 2" className="w-full h-full">
                  <rect width="3" height="2" fill="#006633"/>
                  <rect y="0.6" width="3" height="0.4" fill="#CE1126"/>
                  <rect y="0.9" width="3" height="0.2" fill="#000000"/>
                </svg> */}
                <img src="/src/assets/new-kenya.2.0.png" alt="New Kenya Flag" className="w-full h-full object-cover" />
              </div>
              <div>
                <div className="text-lg font-bold">New Kenya</div>
                <div className="text-xs text-gray-500 ">For a united, prosperous future</div>
              </div>
            </Link>
          </div>

          <nav className="hidden md:flex items-center gap-3" aria-label="Primary Navigation">
            <NavItem to="/">{t('nav.home')}</NavItem>
            <NavItem to="/about">{t('nav.about')}</NavItem>
            <NavItem to="/issues">{t('nav.issues')}</NavItem>
            <NavItem to="/events">Events</NavItem>
            <NavItem to="/get-involved">{t('nav.getInvolved')}</NavItem>
            <LanguageSwitcher />
            <button
              className="fluent-btn fluent-btn-action ml-2"
              onClick={() => onOpenDonate()}
              aria-label="Open donation modal"
            >
              Donate
            </button>
            <RouterLink to="/shop" className="fluent-btn fluent-btn-ghost">Shop</RouterLink>
          </nav>
        </div>
      </div>
    </header>
  );
}
