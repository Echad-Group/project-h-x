import React, { useState, useEffect } from 'react'

const LOCALES = ['en', 'sw']

export default function LanguageSwitcher(){
  const [lang, setLang] = useState(localStorage.getItem('nk_lang') || 'en');

  useEffect(()=>{
    document.documentElement.lang = lang;
    localStorage.setItem('nk_lang', lang);
  },[lang])

  return (
    <div className="flex items-center gap-2">
      <label className="text-sm text-gray-600">Language</label>
      <select value={lang} onChange={e=> setLang(e.target.value)} className="p-1 border rounded">
        {LOCALES.map(l=> <option key={l} value={l}>{l.toUpperCase()}</option>)}
      </select>
    </div>
  )
}
