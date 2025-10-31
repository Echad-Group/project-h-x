import React, {useState} from 'react'
import { Routes, Route, Link } from 'react-router-dom'
import { motion } from 'framer-motion'
import Navbar from './components/Navbar'
import Footer from './components/Footer'
import Home from './pages/Home'
import About from './pages/About'
import Issues from './pages/Issues'
import Events from './pages/Events'
import GetInvolved from './pages/GetInvolved'

export default function App(){
  return (
    <div className="min-h-screen flex flex-col bg-[var(--kenya-white)] text-gray-900">
      <Navbar />
      <main className="flex-1">
        <Routes>
          <Route path="/" element={<Home/>} />
          <Route path="/about" element={<About/>} />
          <Route path="/issues" element={<Issues/>} />
          <Route path="/events" element={<Events/>} />
          <Route path="/get-involved" element={<GetInvolved/>} />
        </Routes>
      </main>
      <Footer />
    </div>
  )
}
