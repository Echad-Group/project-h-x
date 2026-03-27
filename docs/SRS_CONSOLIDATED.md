# Implementation-Aligned System Specification

Last updated: March 27, 2026

## Purpose

This document is not a purely aspirational requirements set. It is a current-state specification snapshot of what the codebase actually implements today, with explicit notes where the platform is partial rather than complete.

## System Scope

The current platform includes:
- a public campaign website
- account registration and authentication
- OTP verification support
- profile and verification workflows
- volunteer registration and organization features
- units, teams, assignments, and tasks
- campaign messaging and push notifications
- compliance and leaderboard features
- election results workflows
- an admin command portal
- a Mongo-backed War Room subsystem
- early mobile client scaffolds

## Current Technical Architecture

### Web frontend
- React 18
- Vite
- React Router 6
- Tailwind CSS
- i18next localization

### API backend
- ASP.NET Core 8
- ASP.NET Identity
- JWT bearer authentication
- Entity Framework Core
- SQLite for transactional storage
- MongoDB for War Room persistence

### Background processes
- message delivery worker
- compliance reminder scheduler
- weekly command reporting service
- hierarchy bootstrap at startup

## Roles and Access

The system supports a mix of public, authenticated, admin, and leadership-oriented workflows.

Current role-sensitive areas include:
- admin panel access
- verification review queue
- content publishing
- command and compliance surfaces
- war-room endpoints
- election result review and adjudication

## Functional Areas

### Public site
Implemented:
- home and informational pages
- issues and policy content
- events and RSVP
- news listing and detail
- campaign team page
- contact and donation intake

### Identity and account
Implemented:
- minimal registration
- login
- OTP send and verify endpoints
- password reset initiation and completion
- logout

Partial:
- onboarding policy is lighter than the broader command-platform vision because verification is deferred to profile completion rather than enforced during registration

### Profile and verification
Implemented:
- profile editing
- profile photo upload
- identity-number capture
- verification document uploads
- profile completeness progress
- reminder banner in app shell
- admin verification queue and decision endpoints

Partial:
- document-security hardening and full face-match automation remain incomplete compared with a production-grade identity platform

### Volunteer and organization
Implemented:
- volunteer sign-up and self-service update or leave flow
- units, teams, assignments, tasks, and organizational visual surfaces
- volunteer dashboard route and integration points

### Messaging and push
Implemented:
- broadcast and targeted messaging endpoints
- inbox retrieval and read acknowledgment
- push subscribe, unsubscribe, status, and send
- category-aware content-triggered push
- service-worker quiet hours and category filtering

Partial:
- advanced scheduling, queue introspection, and delivery analytics are still lighter than a dedicated messaging platform

### Compliance and leaderboard
Implemented:
- compliance summary and reminder queueing
- leaderboard retrieval and recalculation
- operations health and command summary surfaces

### Election and command
Implemented:
- result submission
- pending review
- review decisions
- conflict listing and adjudication
- aggregate and status views
- geolocation ingest and coverage

### War Room
Implemented:
- state snapshot
- command pods
- incidents and escalation
- battle rhythm tracking
- red-zone mode and decisions
- command grid updates
- coalition modules
- mobilization roles
- campaign phases
- legal case records
- Mongo persistence

## Persistence Model

### SQLite entities currently present
- users and identity data
- volunteers
- contacts
- donations
- events and RSVPs
- push subscriptions
- units, teams, assignments, tasks
- issues, initiatives, questions
- campaign team members
- articles
- campaign messages
- compliance reminders
- leaderboard scores
- OTP codes
- election results
- geolocation pings
- audit events

### MongoDB domain
- War Room state collections through `WarRoomMongoStore`

## Non-Functional Snapshot

### Strengths
- large implemented feature surface for a single codebase
- clear split between public content and operational modules
- working JWT and role-based auth model
- route-level protection in frontend
- development HTTP logging and rate limiting configuration present

### Weaknesses
- production hardening is incomplete
- automated tests are sparse
- bundle-size and dependency warnings remain
- local file storage is still used for uploaded assets
- mobile clients are scaffolds, not full clients

## Current Priority Risks

- secrets management
- document storage security
- delivery and background-job observability
- admin bootstrap hardening
- incomplete end-to-end automated coverage

## Current Conclusion

The system is already a hybrid campaign website plus command platform. It is not a minimal MVP anymore. Its main remaining work is operational hardening, deeper workflow polish, and completion of mobile and production infrastructure concerns.
