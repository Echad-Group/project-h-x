import React from 'react'
import { color, motion } from 'framer-motion'

export default function Hero(){
  return (
    <section className="relative overflow-hidden" style={{background: 'url(https://commonwealthchamber.com/wp-content/uploads/2021/07/kenya-1.jpg) no-repeat center', backgroundSize: 'cover'}}>
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
              <a href="#donate" className="px-6 py-3 border border-[var(--kenya-red)] text-[var(--kenya-red)] rounded-md font-semibold">Donate</a>
            </div>
          </div>

          <div className="flex justify-center">
            <div className="w-full max-w-md bg-white rounded-2xl p-6 card-shadow">
              <div className="h-48 rounded-lg bg-gradient-to-br from-[var(--kenya-green)] to-[var(--kenya-red)] flex items-center justify-center text-white font-bold text-xl">Join the movement</div>
              <div className="mt-4">
                <p className="text-sm text-gray-600">Subscribe for updates and events near you.</p>
                <div className="mt-3 flex gap-2">
                  <input aria-label="email" className="flex-1 px-3 py-2 border rounded-md" placeholder="Your email" />
                  <button className="px-4 py-2 bg-[var(--kenya-black)] text-white rounded-md">Subscribe</button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>

    </section>
  )
}
