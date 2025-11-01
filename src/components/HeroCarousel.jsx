import React, {useState, useEffect, useRef} from 'react'
import slide1 from '../assets/slide-1.svg'
import slide2 from '../assets/slide-2.svg'
import slide3 from '../assets/slide-3.svg'

const slides = [
  {src: slide1, alt: 'Join the movement'},
  {src: slide2, alt: 'Community Dialogues'},
  {src: slide3, alt: 'Digital Connectivity'}
]

export default function HeroCarousel({className}){
  const [index, setIndex] = useState(0)
  const timeoutRef = useRef(null)
  const delay = 4000

  useEffect(()=>{
    resetTimeout()
    timeoutRef.current = setTimeout(()=>{
      setIndex((prev)=> (prev + 1) % slides.length)
    }, delay)
    return () => resetTimeout()
  }, [index])

  function resetTimeout(){
    if(timeoutRef.current) clearTimeout(timeoutRef.current)
  }

  function goTo(i){
    setIndex(i % slides.length)
  }

  function prev(){
    setIndex((i)=> (i - 1 + slides.length) % slides.length)
  }

  function next(){
    setIndex((i)=> (i + 1) % slides.length)
  }

  return (
    <div className={`w-full max-w-md ${className ?? ''}`}>
      <div className="relative rounded-2xl overflow-hidden bg-white card-shadow">
        <div className="h-48 md:h-56 w-full bg-gray-200 relative">
          {slides.map((s, i)=> (
            <img
              key={i}
              src={s.src}
              alt={s.alt}
              className={`w-full h-full object-cover transition-transform duration-700 ease-in-out absolute inset-0 ${i === index ? 'translate-x-0 opacity-100' : 'translate-x-full opacity-0'}`}
              style={{transform: i === index ? 'translateX(0)' : 'translateX(100%)'}}
            />
          ))}
        </div>

        <div className="p-4">
          <div className="flex items-center justify-between">
            <div>
              <h4 className="font-bold">Featured</h4>
              <p className="text-sm text-gray-600">Highlights and updates</p>
            </div>
            <div className="flex items-center gap-2">
              <button onClick={prev} aria-label="Previous" className="p-2 rounded-md bg-gray-100 hover:bg-gray-200">
                ‹
              </button>
              <button onClick={next} aria-label="Next" className="p-2 rounded-md bg-gray-100 hover:bg-gray-200">
                ›
              </button>
            </div>
          </div>

          <div className="mt-3 flex items-center gap-2 justify-center">
            {slides.map((_, i)=> (
              <button key={i} onClick={()=>goTo(i)} aria-label={`Go to slide ${i+1}`} className={`w-2 h-2 rounded-full ${i === index ? 'bg-[var(--kenya-green)]' : 'bg-gray-300'}`} />
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}
