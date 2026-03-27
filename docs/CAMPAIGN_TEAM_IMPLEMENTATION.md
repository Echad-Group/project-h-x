# Campaign Team Module

Last updated: March 27, 2026

## Purpose

The Campaign Team module manages the public leadership and staff profiles shown on the Team page and gives admins CRUD and ordering controls from the Admin Panel.

## Current Backend State

### Entity
`CampaignTeamMember` stores:
- name
- role
- email
- phone
- bio
- responsibilities
- photo URL
- Twitter, LinkedIn, Facebook links
- display order
- active flag
- joined date
- created and updated timestamps

### API surface
Base route: `/api/campaignteam`

Public endpoints:
- `GET /api/campaignteam`
- `GET /api/campaignteam/{id}`

Admin endpoints:
- `POST /api/campaignteam`
- `PUT /api/campaignteam/{id}`
- `DELETE /api/campaignteam/{id}`
- `PUT /api/campaignteam/{id}/reorder`

### Data behavior
- Public list returns active members
- Display order is respected before alphabetical fallback
- Admin writes are protected by role-based authorization

## Current Frontend State

### Public page
`src/pages/Team.jsx`

Current behavior:
- fetches live team member data through `campaignTeamService`
- renders member cards responsively
- supports loading, error, and empty states
- displays contact and social links when present

### Admin management
`src/components/admin/AdminCampaignTeam.jsx`

Current behavior:
- list existing members
- create and edit members through admin forms
- delete members
- manage display order
- visualize summary stats and active roster state

### Service layer
`src/services/campaignTeamService.js`
- `getMembers`
- `getMember`
- `createMember`
- `updateMember`
- `deleteMember`
- `reorderMember`

## UX Notes

- The Team page is public and part of the primary navigation
- Team member profiles are database-driven rather than hardcoded
- The module is mature compared with several other feature areas because both the public and admin flows are wired up end to end

## Operational Notes

- Seed data can be created with `backend/seed-campaign-team.ps1`
- Admin management happens inside the Admin Panel rather than on a standalone admin route
- Photo URLs are URL-based fields rather than binary media uploads handled by this module

## Current Constraints

- There is no dedicated media uploader for campaign team photos yet
- There is no moderation history or audit trail specific to team-profile edits beyond general platform audit features
- There is no bulk import or export flow for campaign team data
