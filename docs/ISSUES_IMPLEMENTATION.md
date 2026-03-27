# Issues and Policy Platform

Last updated: March 27, 2026

## Purpose

The Issues module presents campaign policy positions publicly and supports nested initiatives and FAQ-style questions through backend-managed content.

## Backend

### Core entities
- `Issue`
- `IssueInitiative`
- `IssueQuestion`

### API surface
Base route: `/api/issues`

Public endpoints:
- `GET /api/issues`
- `GET /api/issues/{id}`
- `GET /api/issues/slug/{slug}`

Admin endpoints:
- `POST /api/issues`
- `PUT /api/issues/{id}`
- `DELETE /api/issues/{id}`
- `POST /api/issues/{issueId}/initiatives`
- `PUT /api/issues/initiatives/{id}`
- `DELETE /api/issues/initiatives/{id}`
- `POST /api/issues/{issueId}/questions`
- `PUT /api/issues/questions/{id}`
- `DELETE /api/issues/questions/{id}`
- `POST /api/issues/seed`

### Current behavior
- public reads return published issues for presentation use
- issue detail payloads include initiatives and questions
- admin paths support nested content maintenance instead of forcing whole-record replacement for every sub-item change

## Frontend

### Public page
`src/pages/Issues.jsx`

Current behavior:
- loads issue inventory from the API
- renders issue summaries, initiatives, and Q&A content
- supports fallback rendering when the backend is unavailable

### Home integration
`src/sections/IssuesPreview.jsx`
- surfaces a subset of issue content on the home page

## Content model notes

- This module acts as a structured policy-content system rather than a blog
- Slugs exist for deeper linking even though the frontend currently centers the aggregate issues page rather than a mature issue-detail route hierarchy

## Operational Notes

- Seed data can be created with `backend/seed-issues.ps1`
- SQLite stores issues and nested children with ordering fields for display control

## Current Constraints

- The frontend does not yet expose a robust standalone issue-detail experience equivalent to the news detail page
- There is no rich text editor or media upload flow in the current admin UX for policy content
