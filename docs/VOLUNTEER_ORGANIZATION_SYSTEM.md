# Volunteer Organization System

Last updated: March 27, 2026

## Purpose

The volunteer organization system manages units, teams, volunteer assignments, task flows, and hierarchy-aware campaign operations.

## Backend Modules

### Units
`UnitsController`
- `GET /api/units`
- `GET /api/units/{id}`
- `POST /api/units`
- `PUT /api/units/{id}`
- `DELETE /api/units/{id}`
- `GET /api/units/{id}/volunteers`

### Teams
`TeamsController`
- `GET /api/teams`
- `GET /api/teams/{id}`
- `POST /api/teams`
- `PUT /api/teams/{id}`
- `DELETE /api/teams/{id}`

### Assignments
`AssignmentsController`
- `GET /api/assignments/volunteer/{volunteerId}`
- `POST /api/assignments`
- `PUT /api/assignments/{id}`
- `DELETE /api/assignments/{id}`
- `POST /api/assignments/bulk`

### Tasks
`TasksController`
- `POST /api/tasks/create`
- `POST /api/tasks/assign`
- `GET /api/tasks/my-tasks`
- `GET /api/tasks/manage`
- `PUT /api/tasks/{taskId}`
- `DELETE /api/tasks/{taskId}`
- `POST /api/tasks/status`
- `POST /api/tasks/complete`

## Current Frontend Surface

### Public and volunteer-facing
- `/get-involved`
- `/volunteer/dashboard`
- `/organization`
- `/tasks`

### Admin-facing
- `AdminTeams`
- `AdminVolunteers`
- `AdminHierarchyManager`
- `AdminProjects`
- `AdminCommandCenter`

### Services
- `src/services/organizationService.js`
- `src/services/volunteerService.js`
- `src/services/campaignCommandService.js`

## Current Functionality

- unit and team data can be managed through backend CRUD endpoints
- volunteer assignments support create, update, delete, and bulk operations
- tasks support creation, assignment, status change, completion, update, and deletion
- hierarchy and command-oriented screens consume this data for admin workflows
- volunteers can see their involvement state through the volunteer experience

## Current Role in the Platform

This subsystem is one of the bridges between the public-facing campaign site and the operational campaign network. It overlaps with:
- hierarchy and downline management
- task execution
- compliance follow-up
- leaderboard scoring
- volunteer status and engagement

## Current Constraints

- not every backend capability has a fully equivalent polished end-user workflow yet
- some of the deepest operational controls remain stronger in admin surfaces than in volunteer-facing screens
- assignment and task analytics remain more operational than narrative in the current UI
