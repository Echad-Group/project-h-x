import React from 'react'
import { motion } from 'framer-motion'

import HeroCarousel from '../components/HeroCarousel'

export default function Hero({onOpenDonate}) {

  return (
    <section className="relative overflow-hidden" style={{ background: 'url(/src/assets/fluent-style-nk-bg.png) no-repeat center', backgroundSize: 'cover'}}>
      <div className="absolute inset-0 flex">
        <div className="w-1/3 bg-[var(--kenya-green)]/8" />
        <div className="w-1/3 bg-[var(--kenya-black)]/8" />
        <div className="w-1/3 bg-[var(--kenya-red)]/8" />
      </div>

      <div className="relative max-w-7xl mx-auto px-4 py-28 sm:py-36">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-center">
          <div style={{backgroundColor: 'rgba(0,0,0,0.5)', backdropFilter: 'blur(10px)', padding: '1.5rem', borderRadius: '0.5rem'}}>
            <motion.h1 initial={{y:20, opacity:0}} animate={{y:0, opacity:1}} transition={{duration:0.6}} className="text-4xl text-white sm:text-5xl font-extrabold leading-tight">
              New Kenya
              <span className="block text-2xl font-light mt-2 text-gray-700" style={{color: 'whitesmoke'}}>A modern vision for unity, jobs, and opportunity</span>
            </motion.h1>

            <p className="mt-6 text-lg text-gray-700 max-w-xl" style={{color: 'wheat'}}>We are building a New Kenya — one that invests in youth, strengthens communities, and modernizes institutions. Join us to make it happen.</p>

            <div className="mt-8 flex gap-3">
              <a href="#get-involved" className="px-6 py-3 bg-[var(--kenya-green)] text-white rounded-md font-semibold shadow-sm hover:opacity-95">Get Involved</a>
              <a href="#donate" className="px-6 py-3 border border-[var(--kenya-red)] text-[var(--kenya-red)] rounded-md font-semibold" onClick={(e) => {e.preventDefault(); onOpenDonate(); }}>Donate</a>
            </div>
          </div>

          <div className="flex justify-center">
            {/* Carousel component */}
            <div className="w-full max-w-md">
              <HeroCarousel />
            </div>
          </div>

        </div>
      </div>

    </section>
  )
}
