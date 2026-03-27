# Project H X MAUI Client

Last updated: March 27, 2026

## Purpose

This folder contains a .NET MAUI scaffold aligned with the platform's field-operations and campaign-command direction.

## Current Status

The MAUI client is an architectural starter rather than a deployed product.

## Included Today

- shell navigation with mobile-first tabs
- login and OTP challenge pages and view models
- API auth service contracts and baseline implementation
- secure storage session service
- offline outbox contracts and in-memory placeholder processor
- placeholder pages for tasks, submission, inbox, leaderboard, and profile

## Intended Direction

The MAUI client is positioned for eventual support of:
- native authentication
- offline-first submission patterns
- task execution and message inbox workflows
- push registration and deep links
- geolocation and camera-assisted evidence capture

## Notes

- base API reference in the scaffold points at the local backend API
- emulator-specific localhost mapping still needs to be handled per platform
- persistent offline storage and production-grade sync behavior are not yet complete

## Current Constraints

- this folder should be treated as a scaffold
- the web app remains the primary implemented client surface in the repository today
