# Volunteer Account and Signup Flow

Last updated: March 27, 2026

## Purpose

Volunteer onboarding is split between account creation and volunteer registration so the platform can support both lightweight signup and richer volunteer data capture.

## Current Account Creation Flow

### Registration
The main account registration route is `/register`.

Current required fields:
- first name
- last name
- email
- password
- confirm password

This is intentionally lighter than earlier versions of the flow. Identity and verification details are now completed later through the Profile page.

### Authentication endpoints
`AuthController` currently exposes:
- `POST /api/auth/register`
- `POST /api/auth/login`
- `POST /api/auth/send-otp`
- `POST /api/auth/verify`
- `POST /api/auth/logout`
- `POST /api/auth/reset-password`
- `POST /api/auth/forgot-password`

### Current auth behavior
- registration uses multipart form handling but no longer requires document uploads
- login can trigger OTP verification workflow
- forgot-password and reset-password flows exist
- JWT and serialized user state are stored in local storage by the frontend auth service

## Current Volunteer Flow

### Public volunteer registration
The volunteer interest flow is surfaced through `/get-involved` and `VolunteerSignup.jsx`.

Current backend support in `VolunteersController` includes:
- `GET /api/volunteers`
- `GET /api/volunteers/admin`
- `GET /api/volunteers/{id}`
- `GET /api/volunteers/check-status`
- `POST /api/volunteers`
- `PUT /api/volunteers/me`
- `DELETE /api/volunteers/me`
- `DELETE /api/volunteers/{id}`

### Current volunteer experience
- authenticated users can check if they already have a volunteer record
- existing volunteer records can be edited through self-service update flow
- volunteers can leave through self-service delete flow
- volunteer status is surfaced in the dashboard and get-involved experience

### Volunteer dashboard
Route: `/volunteer/dashboard`

The dashboard is intended to give volunteers a view into their status, tasks, and involvement state. It is present in the route map and integrated with the rest of the authenticated member experience.

## Relationship Between Account and Volunteer Records

- A platform account can exist before a volunteer record exists
- Volunteer registration links the user identity to volunteer-specific campaign participation data
- The volunteer system is no longer the only way for a user to enter the platform, because registration is now its own lightweight flow

## Current Constraints

- Email delivery and password-recovery workflows depend on runtime email configuration
- Volunteer dashboard depth is still lighter than the admin-side command and management surface
- Legacy documentation that described account creation as tightly coupled to volunteer creation is no longer accurate
