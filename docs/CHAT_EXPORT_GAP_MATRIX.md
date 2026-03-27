# Adaptation Matrix

Last updated: March 27, 2026

## Purpose

This document compares the broader campaign-command vision that informed the project with what is actually implemented in the current codebase.

Status legend:
- `Adapted`: implemented in a meaningful current form
- `Partial`: some implementation exists, but depth or polish is incomplete
- `Not Adapted`: concept is still absent or only implied

## Identity, Verification, and Onboarding

- Lightweight account registration: `Adapted`
- OTP login challenge flow: `Adapted`
- Deferred profile completion and reminder banner: `Adapted`
- Profile completeness checklist: `Adapted`
- Verification queue with decision endpoints: `Adapted`
- NIDA, voter card, and selfie upload support: `Adapted`
- Mandatory registration-time document pack: `Not Adapted`
- Strong encrypted-at-rest document storage strategy: `Partial`
- Full automated face-match pipeline as a mandatory flow: `Partial`

## Volunteer and Hierarchy Operations

- Volunteer signup and self-service update or leave flow: `Adapted`
- Units, teams, assignments, and task APIs: `Adapted`
- Downline add, remove, reassign, tree, and leader-capacity APIs: `Adapted`
- Rich end-user hierarchy visualization and manipulation UX: `Partial`
- Verified-only downline expansion rules: `Partial`

## Messaging, Push, and Compliance

- Broadcast and targeted campaign messaging APIs: `Adapted`
- Delivery worker with queued channel handling: `Adapted`
- Inbox and read acknowledgment: `Adapted`
- Push subscription, status, unsubscribe, and send APIs: `Adapted`
- Category-aware push dispatch from News and Events: `Adapted`
- Quiet hours and category filtering in service worker: `Adapted`
- Fully scheduled broadcast orchestration UI and queue controls: `Partial`
- Deep delivery analytics and observability: `Partial`
- Daily compliance reminder scheduler and escalation model: `Adapted`
- Personalized compliance messaging with nearest-office guidance: `Not Adapted`

## Content and Public Website

- News system with public listing, detail page, admin management, and featured content: `Adapted`
- Events system with slugs, detail pages, and RSVPs: `Adapted`
- Policy issues platform with nested initiatives and questions: `Adapted`
- Dynamic campaign team page and admin management: `Adapted`
- Donation and contact intake APIs: `Adapted`
- Live payment gateway integration: `Not Adapted`

## Command, Analytics, and Election Features

- Command dashboard summary: `Adapted`
- Leaderboard and my-rank APIs: `Adapted`
- Geolocation ingest and coverage endpoints: `Adapted`
- Election result submission, review, conflict handling, aggregate, and status APIs: `Adapted`
- Admin command center and election dashboard UI: `Adapted`
- Weekly command reporting hosted service: `Adapted`
- Full deep-drill operational analytics across every hierarchy dimension: `Partial`

## War Room

- War Room controller and service surface: `Adapted`
- Mongo-backed War Room persistence: `Adapted`
- Command pods: `Adapted`
- Incidents and escalation actions: `Adapted`
- Battle rhythm checkpoints: `Adapted`
- Red-zone mode and decision log: `Adapted`
- Command grid state: `Adapted`
- Coalition modules and mobilization roles: `Adapted`
- Campaign phase switching: `Adapted`
- Legal case tracking: `Adapted`
- Mature real-time collaborative war-room UX with external integrations: `Partial`

## Mobile Clients

- Expo mobile starter aligned to campaign platform: `Partial`
- .NET MAUI scaffold aligned to field-operations architecture: `Partial`
- Production-ready native mobile experience: `Not Adapted`

## Summary

The current codebase has moved well beyond a public brochure site and already implements meaningful identity, profile, volunteer, command, election, messaging, and war-room capabilities. The main remaining gaps are production hardening, deeper analytics and workflow polish, a stronger document-security model, and completion of the mobile clients.
