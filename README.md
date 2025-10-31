# New Kenya — Campaign UI (Demo)

This repository contains a modern, responsive React + Vite + Tailwind UI scaffold for a presidential campaign website themed "New Kenya". It uses Kenyan national colors and a clean, modern aesthetic.

Requirements
- Node.js 16+ and npm (or yarn)

Install (PowerShell)

```powershell
# from repository root
npm install

# start dev server
npm run dev
```

Available scripts
- `npm run dev` — start Vite dev server (opens at http://localhost:5173)
- `npm run build` — build production static assets
- `npm run preview` — preview production build

Notes
- This is a UI demo scaffold. Donation and subscription flows are client-side placeholders. Replace alert-based flows with secure, server-backed integrations for production.
- Update the candidate name and copy in `index.html`, `src/components/Navbar.jsx`, and `src/pages/About.jsx`.

Next steps
- Add backend endpoints for newsletter signups, RSVPs, and donations.
- Add translations and accessibility improvements.
- Wire analytics and A/B experimentation for messaging.
