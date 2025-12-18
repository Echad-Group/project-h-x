import React, { useState } from 'react';
import { Link, NavLink } from 'react-router-dom'
import LanguageSwitcher from './LanguageSwitcher'
import { Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useAuth } from '../contexts/AuthContext'


const NavItem = ({to, children}) => (
  <NavLink to={to} className={({isActive}) => `px-3 py-2 rounded-md text-sm font-medium ${isActive ? 'bg-[var(--kenya-green)]/10 text-[var(--kenya-green)]' : 'text-gray-700 hover:text-[var(--kenya-green)]'}`}>
    {children}
  </NavLink>
)

export default function Navbar({ onOpenDonate }) {
  const { t } = useTranslation()
  const { user, isAuthenticated, logout } = useAuth()
  
  const handleLogout = async () => {
    await logout()
    window.location.href = '/'
  }
  
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
            
            {isAuthenticated ? (
              <>
                <Link to="/volunteer/dashboard" className="px-3 py-2 text-sm font-medium text-[var(--kenya-green)] hover:bg-[var(--kenya-green)]/10 rounded-md flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                  </svg>
                  Dashboard
                </Link>
                {user?.roles?.includes('Admin') && (
                  <Link to="/admin" className="px-3 py-2 text-sm font-medium text-purple-600 hover:bg-purple-100 rounded-md flex items-center gap-1">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    </svg>
                    Admin
                  </Link>
                )}
                <div className="relative group">
                  <button className="flex items-center gap-2 px-3 py-2 text-sm text-gray-700 hover:text-[var(--kenya-green)] rounded-md">
                    <div className="w-8 h-8 bg-[var(--kenya-green)] rounded-full flex items-center justify-center text-white text-xs font-bold">
                      {user?.firstName?.charAt(0)}{user?.lastName?.charAt(0)}
                    </div>
                    <span>{user?.firstName || user?.email}</span>
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                  </button>
                  
                  {/* Dropdown Menu */}
                  <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg border opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200">
                    <Link to="/profile" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 rounded-t-lg">
                      <span className="flex items-center gap-2">
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                        Profile & Settings
                      </span>
                    </Link>
                    <button
                      onClick={handleLogout}
                      className="w-full text-left block px-4 py-2 text-sm text-red-600 hover:bg-red-50 rounded-b-lg"
                    >
                      <span className="flex items-center gap-2">
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                        </svg>
                        {t('auth.logout') || 'Logout'}
                      </span>
                    </button>
                  </div>
                </div>
              </>
            ) : (
              <>
                <Link to="/login" className="fluent-btn fluent-btn-ghost">
                  {t('auth.login.title') || 'Login'}
                </Link>
                <Link to="/register" className="fluent-btn fluent-btn-ghost">
                  {t('auth.register.title') || 'Sign Up'}
                </Link>
              </>
            )}
            
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
