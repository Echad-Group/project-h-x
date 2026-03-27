# Project H X Mobile App

Last updated: March 27, 2026

## Purpose

This folder contains the Expo React Native starter for a mobile companion to the campaign platform.

## Current Status

The mobile app is a scaffold, not a production-complete client.

Current intent:
- provide a native shell aligned with campaign operations
- support future task, inbox, geolocation, and election result workflows
- connect to the existing backend API rather than invent a separate backend model

## Current Scope

- starter mobile command shell
- early status integration concepts
- foundation for adding authentication, task execution, messaging, and geolocation flows

## Run

```powershell
npm install
npm run start
```

Then open via:
- Expo Go
- Android emulator
- iOS simulator where supported by the local setup

## Current Constraints

- this client is not yet feature-complete relative to the web app
- auth, offline behavior, and campaign operations are not yet fully implemented here
- the mobile folder should be treated as an expansion track, not the primary production surface today
