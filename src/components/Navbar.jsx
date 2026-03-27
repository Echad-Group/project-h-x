import React, { useEffect, useState } from 'react';
import { Link, NavLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { useAuth } from '../contexts/AuthContext'
import { volunteerService } from '../services/volunteerService'


const NavItem = ({to, children}) => (
  <NavLink to={to} className={({isActive}) => `px-3 py-2 rounded-md text-sm font-medium ${isActive ? 'bg-[var(--kenya-green)]/10 text-[var(--kenya-green)]' : 'text-gray-700 hover:text-[var(--kenya-green)]'}`}>
    {children}
  </NavLink>
)

export default function Navbar({ onOpenDonate }) {
  const { t } = useTranslation()
  const { user, isAuthenticated, logout } = useAuth()
  const [mobileOpen, setMobileOpen] = useState(false)
  const [hasVolunteered, setHasVolunteered] = useState(
    Array.isArray(user?.roles) && user.roles.includes('Volunteer')
  )

  useEffect(() => {
    let isMounted = true

    async function resolveVolunteerState() {
      if (!isAuthenticated) {
        if (isMounted) setHasVolunteered(false)
        return
      }

      const roleBasedVolunteer = Array.isArray(user?.roles) && user.roles.includes('Volunteer')

      try {
        const status = await volunteerService.checkStatus()
        if (!isMounted) return
        setHasVolunteered(Boolean(status?.isVolunteer) || roleBasedVolunteer)
      } catch {
        if (!isMounted) return
        setHasVolunteered(roleBasedVolunteer)
      }
    }

    resolveVolunteerState()

    return () => {
      isMounted = false
    }
  }, [isAuthenticated, user?.roles])

  const primaryLinks = [
    { to: '/', label: t('nav.home') },
    { to: '/about', label: t('nav.about') },
    { to: '/issues', label: t('nav.issues') },
    { to: '/events', label: 'Events' },
    ...((isAuthenticated && hasVolunteered) ? [] : [{ to: '/get-involved', label: t('nav.getInvolved') }])
  ]
  
  const handleLogout = async () => {
    await logout()
    window.location.href = '/'
  }
  
  return (
    <header className="sticky top-0 z-40 border-b border-gray-200 bg-white/95 backdrop-blur">
      <div className="w-full px-[5%]">
        <div className="flex justify-between h-16 items-center">
          <div className="flex items-center gap-8">
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
                <div className="hidden sm:block text-xs text-gray-500">For a united, prosperous future</div>
              </div>
            </Link>

            <nav className="hidden xl:flex items-center gap-1" aria-label="Primary Navigation">
              {primaryLinks.map((link) => (
                <NavItem key={link.to} to={link.to}>{link.label}</NavItem>
              ))}
            </nav>
          </div>

          <nav className="hidden xl:flex items-center gap-2" aria-label="Utility Navigation">
            {isAuthenticated ? (
              <>
                { hasVolunteered && ( <Link to="/volunteer/dashboard" className="px-2 py-2 text-sm font-medium text-[var(--kenya-green)] hover:bg-[var(--kenya-green)]/10 rounded-md flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                  </svg>
                  Dashboard
                </Link> ) }
                {/* hasVolunteered && (
                  <Link to="/get-involved" className="px-2 py-2 text-sm font-medium text-[var(--kenya-green)] hover:bg-[var(--kenya-green)]/10 rounded-md flex items-center gap-1">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                    </svg>
                    {t('nav.getInvolved')}
                  </Link>
                ) */}
                <Link to="/tasks" className="px-2 py-2 text-sm font-medium text-[var(--kenya-green)] hover:bg-[var(--kenya-green)]/10 rounded-md flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2" />
                  </svg>
                  Tasks
                </Link>
                <Link to="/leaderboard" className="px-2 py-2 text-sm font-medium text-[var(--kenya-green)] hover:bg-[var(--kenya-green)]/10 rounded-md flex items-center gap-1">
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17v-6m3 6V7m3 10v-4m4 8H5a2 2 0 01-2-2V5a2 2 0 012-2h14a2 2 0 012 2v14a2 2 0 01-2 2z" />
                  </svg>
                  Leaderboard
                </Link>
                {user?.roles?.includes('Admin') && (
                  <Link to="/admin" className="px-2 py-2 text-sm font-medium text-purple-600 hover:bg-purple-100 rounded-md flex items-center gap-1">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                    </svg>
                    Admin
                  </Link>
                )}
                <div className="relative group">
                  <button className="flex items-center gap-2 px-3 py-2 text-sm text-gray-700 hover:text-[var(--kenya-green)] rounded-md">
                    {user?.profilePhotoUrl ? (
                      <img 
                        src={`http://localhost:5065${user.profilePhotoUrl}`}
                        alt="Profile"
                        className="w-8 h-8 rounded-full object-cover border-2 border-[var(--kenya-green)]"
                      />
                    ) : (
                      <div className="w-8 h-8 bg-[var(--kenya-green)] rounded-full flex items-center justify-center text-white text-xs font-bold">
                        {user?.firstName?.charAt(0)}{user?.lastName?.charAt(0)}
                      </div>
                    )}
                    <span>{user?.firstName || user?.email}</span>
                    {/* <span className={`hidden 2xl:inline-flex items-center rounded-full px-2 py-0.5 text-xs font-semibold ${hasVolunteered ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'}`}>
                      {hasVolunteered ? 'Volunteer Active' : 'Status Pending'}
                    </span> */}
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                  </button>
                  
                  {/* Dropdown Menu */}
                  <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg border opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200">
                    <div className="px-4 py-2 border-b border-gray-100">
                      <p className="text-xs text-gray-500">Status</p>
                      <p className={`text-sm font-semibold ${hasVolunteered ? 'text-emerald-700' : 'text-amber-700'}`}>
                        {hasVolunteered ? 'Volunteer Active' : 'Not Volunteering Yet'}
                      </p>
                    </div>
                    { !hasVolunteered && (
                    <Link to="/volunteer/dashboard" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 rounded-t-lg">
                      <span className="flex items-center gap-2">
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                        </svg>
                        Dashboard
                      </span>
                    </Link> ) }
                    
                    <Link to="/profile" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 rounded-t-lg">
                      <span className="flex items-center gap-2">
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                        Profile & Settings
                      </span>
                    </Link>
                    <Link to="/notification-settings" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50">
                      <span className="flex items-center gap-2">
                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                        </svg>
                        Notification Settings
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
              className="fluent-btn fluent-btn-action ml-1"
              onClick={() => onOpenDonate()}
              aria-label="Open donation modal"
            >
              Donate
            </button>
          </nav>

          <button
            type="button"
            className="xl:hidden inline-flex items-center justify-center rounded-md p-2 text-gray-700 hover:bg-gray-100"
            onClick={() => setMobileOpen((prev) => !prev)}
            aria-label={mobileOpen ? 'Close menu' : 'Open menu'}
            aria-expanded={mobileOpen}
          >
            {mobileOpen ? (
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            ) : (
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            )}
          </button>
        </div>

        {mobileOpen && (
          <div className="xl:hidden flex flex-wrap border-t border-gray-200 py-4 space-y-3">
            {isAuthenticated && (
              <div className="w-[-webkit-fill-available] px-3 py-2 rounded-md bg-gray-50 border border-gray-200 flex items-center justify-between">
                <span className="text-sm text-gray-700">Status</span>
                <span className={`text-xs font-semibold rounded-full px-2 py-0.5 ${hasVolunteered ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'}`}>
                  {hasVolunteered ? 'Volunteer Active' : 'Pending'}
                </span>
              </div>
            )}

            <nav className="grid grid-cols-1 gap-1 " style={{flex: "0 0 50%"}} aria-label="Mobile Primary Navigation">
              {primaryLinks.map((link) => (
                <NavLink
                  key={link.to}
                  to={link.to}
                  onClick={() => setMobileOpen(false)}
                  className={({isActive}) => `px-3 py-2 rounded-md text-sm font-medium ${isActive ? 'bg-[var(--kenya-green)]/10 text-[var(--kenya-green)]' : 'text-gray-700 hover:text-[var(--kenya-green)] hover:bg-gray-50'}`}
                >
                  {link.label}
                </NavLink>
              ))}
            </nav>

            {isAuthenticated ? (
              <div className="grid grid-cols-2 gap-2 pt-2 " style={{flex: "0 0 50%"}}>
                <Link to="/volunteer/dashboard" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost hover:bg-green-50 justify-start">
                  <span className="flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                    </svg>
                    Dashboard
                  </span>
                </Link>
                {hasVolunteered && (
                  <Link to="/get-involved" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost hover:bg-green-50 justify-start">
                    <span className="flex items-center gap-2">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                      </svg>
                      {t('nav.getInvolved')}
                    </span>
                  </Link>
                )}
                <Link to="/tasks" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost hover:bg-green-50 justify-start">
                  <span className="flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2" />
                    </svg>
                    Tasks
                  </span>
                </Link>
                <Link to="/leaderboard" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost hover:bg-green-50 justify-start">
                  <span className="flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 17v-6m3 6V7m3 10v-4m4 8H5a2 2 0 01-2-2V5a2 2 0 012-2h14a2 2 0 012 2v14a2 2 0 01-2 2z" />
                    </svg>
                    Leaderboard
                  </span>
                </Link>
                <Link to="/profile" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost hover:bg-green-50 justify-start">
                  <span className="flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                    Profile
                  </span>
                </Link>
                <Link to="/notification-settings" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost hover:bg-green-50 justify-start">
                  <span className="flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
                    </svg>
                    Notifications
                  </span>
                </Link>
                {user?.roles?.includes('Admin') && (
                  <Link to="/admin" onClick={() => setMobileOpen(false)} className="fluent-btn fluent -btn-ghost text-purple-600 hover:bg-purple-100 justify-start text-purple-600">
                    <span className="flex items-center gap-2">
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      </svg>
                      Admin
                    </span>
                  </Link>
                )}
                <button onClick={handleLogout} className="fluent-btn fluent -btn-ghost justify-start text-red-600 hover:bg-red-50">
                  <span className="flex items-center gap-2">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                    </svg>
                    {t('auth.logout') || 'Logout'}
                  </span>
                </button>
              </div>
            ) : (
              <div className="grid grid-cols-2 gap-2 pt-2">
                <Link to="/login" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost justify-center">{t('auth.login.title') || 'Login'}</Link>
                <Link to="/register" onClick={() => setMobileOpen(false)} className="fluent-btn fluent-btn-ghost justify-center">{t('auth.register.title') || 'Sign Up'}</Link>
              </div>
            )}

            <div className="w-[-webkit-fill-available] grid grid-cols-2 gap-2 pt-2">
              <button
                className="fluent-btn fluent-btn-action justify-center"
                onClick={() => {
                  onOpenDonate()
                  setMobileOpen(false)
                }}
                aria-label="Open donation modal"
              >
                Donate
              </button>
            </div>
          </div>
        )}
        </div>
    </header>
  );
}
