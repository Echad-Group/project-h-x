import React, { useState, useEffect } from 'react'
import { useTranslation } from 'react-i18next'

const LOCALES = [
  { code: 'en', labelKey: 'language.english' },
  { code: 'sw', labelKey: 'language.swahili' }
]

export default function LanguageSwitcher(){
  const { t, i18n } = useTranslation()
  const [lang, setLang] = useState(localStorage.getItem('locale') || i18n.language || 'en');

  useEffect(()=>{
    document.documentElement.lang = lang;
    localStorage.setItem('locale', lang);
    i18n.changeLanguage(lang);
  },[lang, i18n])

  return (
    <div className="flex items-center gap-2">
      <label className="text-sm text-gray-600">{/* {t('language.label')} */}</label>
      <select value={lang} onChange={e=> setLang(e.target.value)} className="p-1 border rounded">
        {LOCALES.map(l=> <option key={l.code} value={l.code}>{t(l.labelKey)}</option>)}
      </select>
    </div>
  )
}
