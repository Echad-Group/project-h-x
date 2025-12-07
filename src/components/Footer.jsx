import React from 'react'
import { useTranslation } from 'react-i18next'

export default function Footer(){
  const { t } = useTranslation();

  return (
    <footer className="bg-[var(--kenya-black)] text-white py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 grid grid-cols-1 md:grid-cols-3 gap-6">
        <div>
          <h4 className="text-lg font-semibold">New Kenya</h4>
          <p className="text-sm text-gray-200 mt-2">{t('footer.tagline')}</p>
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-2 sm:gap-4">
          <div>
            <h5 className="font-semibold">{t('footer.quickLinksHeading')}</h5>
            <ul className="mt-2 text-sm text-gray-200 space-y-1">
              <li><a href="/about">{t('nav.about')}</a></li>
              <li><a href="/issues">{t('nav.issues')}</a></li>
              <li><a href="/events">{t('nav.events')}</a></li>
              <li><a href="/news">{t('nav.news')}</a></li>
              <li><a href="/get-involved">{t('nav.getInvolved')}</a></li>
            </ul>
          </div>
          <div>
            <h5 className="font-semibold h-0 sm:h-auto">&nbsp;</h5>
            <ul className="mt-0 sm:mt-2 text-sm text-gray-200 space-y-1">
              <li><a href="/contact">{t('nav.contact')}</a></li>
              <li><a href="/gallery">{t('gallery.title')}</a></li>
              <li><a href="/faq">{t('faq.title')}</a></li>
              <li><a href="/shop">{t('shop.title')}</a></li>
              <li><a href="/team">{t('team.title')}</a></li>
            </ul>
          </div>
        </div>
        <div>
          <h5 className="font-semibold">{t('footer.contactHeading')}</h5>
          <p className="text-sm text-gray-200 mt-2"><a href="mailto:info@newkenya.org" tagert="_blank">info@newkenya.org</a></p>
          <p className="text-sm text-gray-200"><a href="tel:+254700000000" target="_blank">+254 700 000 000</a></p>
        </div>
      </div>
      <div className="mt-8 text-center text-xs text-gray-400">© {new Date().getFullYear()} New Kenya — {t('footer.rights')}</div>
    </footer>
  )
}
