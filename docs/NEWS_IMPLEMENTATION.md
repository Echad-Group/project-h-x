# News Module

Last updated: March 27, 2026

## Purpose

The News module provides a dynamic article system for campaign updates, policy announcements, event coverage, and editorial publishing. It powers both public browsing and admin news management.

## Backend

### Core entity
`Article`

### API surface
Base route: `/api/news`

Public endpoints:
- `GET /api/news`
- `GET /api/news/featured`
- `GET /api/news/{slug}`
- `GET /api/news/by-id/{id}`
- `POST /api/news/{id}/increment-view`
- `GET /api/news/categories`
- `GET /api/news/tags`

Admin endpoints:
- `POST /api/news`
- `PUT /api/news/{id}`
- `DELETE /api/news/{id}`

### Current implementation details
- filtering supports category, search, status, sort, and pagination
- articles can be featured and include multiple image URLs
- view tracking is handled by a dedicated increment endpoint
- publishing and updates can dispatch category-tagged push notifications

## Frontend

### Public pages
- `src/pages/News.jsx`
- `src/pages/NewsArticle.jsx`

Current behavior:
- article listing supports search, filtering, sort, and pagination
- detail pages load by slug and increment article views
- related-content and sharing patterns are supported in the article experience

### Shared sections and admin tools
- `src/sections/NewsCarousel.jsx`
- `src/components/admin/AdminNewsManagement.jsx`
- `src/components/admin/AdminNewsEditor.jsx`

### Service layer
- `src/services/newsService.js`

## Content workflow

- articles can exist in draft, published, or archived-like lifecycle states depending on backend status usage
- featured articles are used in homepage and promotional surfaces
- admin authorship is tracked through authenticated API access

## Current Constraints

- there is no separate editorial workflow engine with approvals
- image handling remains URL-based rather than managed through an upload pipeline owned by the news module
