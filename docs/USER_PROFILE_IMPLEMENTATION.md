# User Profile and Settings

Last updated: March 27, 2026

## Purpose

The Profile module is the primary place where authenticated users manage personal details, account security, and deferred verification data that is no longer required at signup.

## Current Frontend Experience

### Route
- `/profile`
- Protected by `ProtectedRoute`

### Main areas in `src/pages/Profile.jsx`
- Profile information tab
- Security tab
- Account settings tab
- Profile completeness progress card
- Photo upload and delete controls
- Identity and verification section

### Profile completeness
The page now includes:
- a completion percentage
- a progress bar
- a missing-items checklist

The checklist currently evaluates completion for:
- first name
- last name
- phone number
- location
- national ID number
- voter card number
- NIDA document upload
- voter card upload
- selfie upload

### Deferred identity completion
The profile form now includes fields and upload controls for items removed from the signup page:
- national ID number
- voter card number
- NIDA document upload
- voter card document upload
- selfie upload

## Current Backend API

Base route: `/api/userprofile`

Endpoints:
- `GET /api/userprofile`
- `PUT /api/userprofile`
- `POST /api/userprofile/upload-verification-document`
- `PUT /api/userprofile/password`
- `PUT /api/userprofile/email`
- `DELETE /api/userprofile`
- `POST /api/userprofile/upload-photo`
- `DELETE /api/userprofile/photo`

## Current Data Returned by Profile API

The profile response includes:
- identity basics
- contact fields
- bio and social links
- profile photo URL
- role list
- verification status
- voter card status
- national ID number
- voter card number
- uploaded document URLs for NIDA, voter card, and selfie
- account timestamps and verification flags

## Current Validation and Behavior

### Profile update
- first name and last name are required
- website is normalized to null when blank
- national ID and voter card numbers are checked for duplicates against other users before save

### Verification document upload
Supported upload types:
- `nida`
- `voter-card`
- `selfie`

Behavior:
- files are stored under `wwwroot/uploads/verification-docs`
- replacing a document removes the previously stored file for that slot
- uploading a voter card marks voter card status as pending
- uploading any verification document leaves the user in a pending verification state pending review workflows

### Profile photo upload
- separate endpoint and storage path from verification uploads
- files stored under `wwwroot/uploads/profile-photos`

## Related App-Shell Behavior

`src/App.jsx` now includes a profile completion reminder banner that:
- appears for authenticated users
- skips the `/profile` route itself
- checks profile completeness via the profile API
- supports local dismiss cooldown behavior

## Current Constraints

- There is no dedicated per-item inline validation summary beyond the checklist and API error messages
- Verification documents are stored on local disk and not in a hardened object-storage pipeline
- Face-match processing is not triggered from profile uploads alone unless the relevant downstream workflow consumes the uploaded document set
