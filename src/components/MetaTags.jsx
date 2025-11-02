import React, { createContext, useContext } from 'react';
import { generateMetaTags } from '../utils/seoHelpers';

export const MetaContext = createContext();

export const MetaProvider = ({ children }) => {
  const updateMeta = (data) => {
  const { meta, schema } = generateMetaTags(data);
    
    // Update meta tags
    Object.entries(meta).forEach(([key, value]) => {
      if (!value) return;
      
      // Update standard meta tags
      let element = document.querySelector(`meta[name="${key}"]`);
      if (element) {
        element.setAttribute('content', value);
      }
      
      // Update Open Graph meta tags
      element = document.querySelector(`meta[property="og:${key}"]`);
      if (element) {
        element.setAttribute('content', value);
      }
      
      // Update Twitter meta tags
      element = document.querySelector(`meta[property="twitter:${key}"]`);
      if (element) {
        element.setAttribute('content', value);
      }
    });

    // Update title if provided
    if (meta.title) {
      document.title = meta.title;
    }

    // Update JSON-LD if provided
    if (meta.structuredData) {
      let script = document.querySelector('script[type="application/ld+json"]');
      if (script) {
        script.textContent = JSON.stringify(meta.structuredData);
      }
    }
  };

  return (
    <MetaContext.Provider value={{ updateMeta }}>
      {children}
    </MetaContext.Provider>
  );
};

export const useMeta = () => {
  const context = useContext(MetaContext);
  if (!context) {
    throw new Error('useMeta must be used within a MetaProvider');
  }
  return context;
};