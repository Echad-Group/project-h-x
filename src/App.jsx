import React, {useState, useEffect} from 'react'
import { Routes, Route, Link, useLocation } from 'react-router-dom'
import { motion } from 'framer-motion'
import Navbar from './components/Navbar'
import Footer from './components/Footer'
import { MetaProvider } from './components/MetaTags'
import { AuthProvider } from './contexts/AuthContext'
import PWADebugPanel from './components/PWADebugPanel'
import NotificationButton from './components/NotificationButton'
import InstallPrompt from './components/InstallPrompt'
import DonateModal from './components/DonateModal';
import Home from './pages/Home'
import About from './pages/About'
import Issues from './pages/Issues'
import Events from './pages/Events'
import GetInvolved from './pages/GetInvolved'
import Contact from './pages/Contact'
import FAQ from './pages/FAQ'
import News from './pages/News'
import NewsArticle from './pages/NewsArticle'
import Team from './pages/Team'
import Gallery from './pages/Gallery'
import EventDetail from './pages/EventDetail'
import Login from './pages/Login'
import Register from './pages/Register'


export default function App() {
  const [donateOpen, setDonateOpen] = useState(false);
  const appConfig = {
    siteName: 'Project H',
    theme: 'light',
    user: { name: 'Rodern', role: 'member' },
  };

  return (
    <AuthProvider>
      <MetaProvider>
        <div className="min-h-screen flex flex-col bg-[var(--kenya-white)] text-gray-900">
          {/* Skip link for accessibility */}
          <a href="#main" className="sr-only focus:not-sr-only focus:absolute focus:top-4 focus:left-4 bg-white p-2 rounded">Skip to main content</a>
          <Navbar onOpenDonate={() => { setDonateOpen(true); }} />
          <main className="flex-1" id="main">
            <Routes>
              <Route path="/" element={<Home onOpenDonate={() => setDonateOpen(true)} />} />
              <Route path="/about" element={<About onOpenDonate={() => setDonateOpen(true)} />} />
              <Route path="/issues" element={<Issues onOpenDonate={() => setDonateOpen(true)} />} />
              <Route path="/events" element={<Events onOpenDonate={() => setDonateOpen(true)} />} />
              <Route path="/events/:id" element={<EventDetail/>} />
              <Route path="/get-involved" element={<GetInvolved onOpenDonate={() => setDonateOpen(true)} />} />
              <Route path="/contact" element={<Contact/>} />
              <Route path="/faq" element={<FAQ/>} />
              <Route path="/news" element={<News/>} />
              <Route path="/news/:id" element={<NewsArticle/>} />
              <Route path="/team" element={<Team/>} />
              <Route path="/gallery" element={<Gallery/>} />
              <Route path="/login" element={<Login/>} />
              <Route path="/register" element={<Register/>} />
            </Routes>
          </main>
          <Footer />
          <PWADebugPanel />
          <div className="fixed top-20 right-4 z-40">
            <NotificationButton />
          </div>
          <InstallPrompt />
          <DonateModal open={donateOpen} setDonateOpen={setDonateOpen} onClose={() => setDonateOpen(false)} />
        </div>
      </MetaProvider>
    </AuthProvider>
  )
}
