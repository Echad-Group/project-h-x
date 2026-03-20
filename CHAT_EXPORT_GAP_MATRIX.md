# Exported Chat Gap Matrix

## Scope
This document maps requirements from the exported presidential campaign chat against the current project implementation and lists all missing or partially adapted features.

Status legend:
- Not Adapted: requirement not implemented in current codebase
- Partial: some implementation exists, but requirement is incomplete
- Adapted: fully implemented (none in this gap-only document)

## Source Baseline
Primary requirement source: exported chat attachment (presidential campaign architecture + Kenya campaign networking app + war-room model + compliance/WhatsApp/verification workflows).

## Comprehensive Gap List

### 1) War-Room and Command Doctrine

#### 1. Dedicated war-room command module
- Status: Not Adapted
- Evidence: [src/components/admin/AdminCommandCenter.jsx](src/components/admin/AdminCommandCenter.jsx), [src/components/admin/AdminElectionDashboard.jsx](src/components/admin/AdminElectionDashboard.jsx)
- Missing implementation:
  - War-room domain model (commander, units, battle boards, command posts)
  - War-room APIs and UI module for live command orchestration

#### 2. Daily battle-rhythm system (06:00, 09:00, 14:00, 19:00 cycles)
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Program.cs](backend/NewKenyaAPI/Program.cs), [backend/NewKenyaAPI/Services](backend/NewKenyaAPI/Services)
- Missing implementation:
  - Recurring schedule engine for command meetings
  - Workflow tasks and attendance/completion tracking per cycle

#### 3. Crisis escalation protocol (Level 1-4)
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Controllers](backend/NewKenyaAPI/Controllers)
- Missing implementation:
  - Incident entity with severity and escalation path
  - Escalation actions, ownership transitions, response SLAs

#### 4. Legal rapid response and vote protection workflow
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Controllers/ResultsController.cs](backend/NewKenyaAPI/Controllers/ResultsController.cs)
- Missing implementation:
  - Legal rapid-response endpoints
  - Vote-protection case intake and incident handling lifecycle

#### 5. Integrated command matrix (cross-functional pods)
- Status: Not Adapted
- Evidence: [src/components/admin](src/components/admin), [backend/NewKenyaAPI/Services](backend/NewKenyaAPI/Services)
- Missing implementation:
  - Pod composition model (data + field + digital + comms)
  - Assignment to battleground regions/demographic blocs/opponent lanes

#### 6. Election-week red-zone hourly decision mode
- Status: Not Adapted
- Evidence: [src/components/admin/AdminElectionDashboard.jsx](src/components/admin/AdminElectionDashboard.jsx)
- Missing implementation:
  - Election-week mode toggle
  - Hourly decision log and operational checkpoints

#### 7. National/regional/county command grid operations
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/ResultsController.cs](backend/NewKenyaAPI/Controllers/ResultsController.cs), [src/components/admin/AdminElectionDashboard.jsx](src/components/admin/AdminElectionDashboard.jsx)
- Missing implementation:
  - Command-post readiness tracking by command tier
  - Live operational status across national, regional, county, polling units

### 2) Organizational and Coalition Operations

#### 8. Coalition and special-groups command layer
- Status: Not Adapted
- Evidence: [src/components/admin/AdminPanel.jsx](src/components/admin/AdminPanel.jsx), [backend/NewKenyaAPI/Models/UserRoles.cs](backend/NewKenyaAPI/Models/UserRoles.cs)
- Missing implementation:
  - Coalition director workflows
  - Youth/women/faith/diaspora/business outreach modules

#### 9. Kenya-specific mobilization roles from exported chat
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Models/UserRoles.cs](backend/NewKenyaAPI/Models/UserRoles.cs)
- Missing implementation:
  - Party liaison, tribal/regional mobilization, religious advisory, village elder coordinator roles and features

#### 10. Campaign phase management (Exploratory/Launch/Persuasion/GOTV)
- Status: Not Adapted
- Evidence: [src/components/admin](src/components/admin), [backend/NewKenyaAPI/Controllers](backend/NewKenyaAPI/Controllers)
- Missing implementation:
  - Phase entity and lifecycle
  - Phase-based targets, playbooks, and KPI switching

### 3) Identity, Verification, and Compliance

#### 11. Mandatory registration upload pack (NIDA + voter card + selfie)
- Status: Not Adapted
- Evidence: [src/pages/Register.jsx](src/pages/Register.jsx), [backend/NewKenyaAPI/Controllers/AuthController.cs](backend/NewKenyaAPI/Controllers/AuthController.cs), [backend/NewKenyaAPI/Models/ApplicationUser.cs](backend/NewKenyaAPI/Models/ApplicationUser.cs)
- Missing implementation:
  - File upload fields in registration UI/API
  - Validation and secure storage for all three required artifacts

#### 12. Registration-stage OTP enforcement
- Status: Partial
- Evidence: [src/pages/Login.jsx](src/pages/Login.jsx), [backend/NewKenyaAPI/Controllers/AuthController.cs](backend/NewKenyaAPI/Controllers/AuthController.cs)
- Missing implementation:
  - Mandatory OTP during onboarding/registration (not just login challenge)

#### 13. Admin verification review queue and decisions
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Models/ApplicationUser.cs](backend/NewKenyaAPI/Models/ApplicationUser.cs), [src/components/admin](src/components/admin)
- Missing implementation:
  - Verification queue, approve/reject endpoints, reviewer notes, timeline UI

#### 14. AI face-match verification pipeline
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Models/ApplicationUser.cs](backend/NewKenyaAPI/Models/ApplicationUser.cs), [backend/NewKenyaAPI/Services](backend/NewKenyaAPI/Services)
- Missing implementation:
  - Face match service integration and threshold policy
  - Failure routing to admin review

#### 15. Verified-only recruitment rule
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Services/CampaignHierarchyService.cs](backend/NewKenyaAPI/Services/CampaignHierarchyService.cs), [backend/NewKenyaAPI/Controllers/DownlinesController.cs](backend/NewKenyaAPI/Controllers/DownlinesController.cs)
- Missing implementation:
  - Explicit guard that blocks unverified users from adding/recruiting downlines

#### 16. Identity duplicate detection beyond phone/email
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Data/ApplicationDbContext.cs](backend/NewKenyaAPI/Data/ApplicationDbContext.cs)
- Missing implementation:
  - Dedup checks for NIDA, selfie similarity, voter-card correlation

#### 17. Encrypted ID document handling controls
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Controllers/UserProfileController.cs](backend/NewKenyaAPI/Controllers/UserProfileController.cs)
- Missing implementation:
  - Secure-at-rest encryption strategy and secrets-managed keying for identity files

### 4) Downline Network and Field Execution

#### 18. Full downline management UI (not just tree picker)
- Status: Partial
- Evidence: [src/components/admin/AdminMessageComposer.jsx](src/components/admin/AdminMessageComposer.jsx), [backend/NewKenyaAPI/Controllers/DownlinesController.cs](backend/NewKenyaAPI/Controllers/DownlinesController.cs)
- Missing implementation:
  - Dedicated hierarchy screen for add/reassign/remove/conflict resolution and cap visibility

#### 19. Interactive hierarchy tree UX from exported chat
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/DownlinesController.cs](backend/NewKenyaAPI/Controllers/DownlinesController.cs)
- Missing implementation:
  - Full-screen expandable/zoomable tree
  - Color-coded verification/compliance indicators
  - Direct-only vs full-tree toggle in user-facing app

#### 20. Task management end-user surfaces
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/TasksController.cs](backend/NewKenyaAPI/Controllers/TasksController.cs), [src/services/campaignCommandService.js](src/services/campaignCommandService.js), [src/App.jsx](src/App.jsx)
- Missing implementation:
  - Task pages for assign/execute/update/complete in user and leader journeys

#### 21. Combined tasks + compliance reminders screen
- Status: Not Adapted
- Evidence: [src/components/admin/AdminCommandCenter.jsx](src/components/admin/AdminCommandCenter.jsx), [src/components/VolunteerDashboard.jsx](src/components/VolunteerDashboard.jsx)
- Missing implementation:
  - Unified daily execution screen with task actions + voter-card reminder actions

### 5) Messaging, WhatsApp, and Delivery Intelligence

#### 22. Scheduled broadcast (send now vs schedule)
- Status: Not Adapted
- Evidence: [src/components/admin/AdminMessageComposer.jsx](src/components/admin/AdminMessageComposer.jsx), [backend/NewKenyaAPI/Controllers/MessagesController.cs](backend/NewKenyaAPI/Controllers/MessagesController.cs)
- Missing implementation:
  - Schedule timestamp on message requests
  - Delayed dispatch queue and scheduler UI

#### 23. Delivery intelligence dashboard (sent/delivered/read/failed)
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Services/MessageDeliveryWorker.cs](backend/NewKenyaAPI/Services/MessageDeliveryWorker.cs), [src/components/admin/AdminMessageComposer.jsx](src/components/admin/AdminMessageComposer.jsx)
- Missing implementation:
  - Historical delivery analytics UI and APIs
  - Channel breakdowns and retry/dead-letter observability

#### 24. Read-status lifecycle and tracking
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Models/CampaignDomain.cs](backend/NewKenyaAPI/Models/CampaignDomain.cs), [backend/NewKenyaAPI/Services/MessageDeliveryWorker.cs](backend/NewKenyaAPI/Services/MessageDeliveryWorker.cs)
- Missing implementation:
  - Read acknowledgment API/event and reporting

#### 25. WhatsApp compliance reminders to members
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/ComplianceController.cs](backend/NewKenyaAPI/Controllers/ComplianceController.cs), [backend/NewKenyaAPI/Services/MessageDeliveryWorker.cs](backend/NewKenyaAPI/Services/MessageDeliveryWorker.cs)
- Missing implementation:
  - Daily reminders routed via both app and WhatsApp with explicit templates

### 6) Reminder Automation and Escalation

#### 26. Automated daily reminder scheduler
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Controllers/ComplianceController.cs](backend/NewKenyaAPI/Controllers/ComplianceController.cs), [backend/NewKenyaAPI/Program.cs](backend/NewKenyaAPI/Program.cs)
- Missing implementation:
  - Recurring job for daily compliance reminder generation

#### 27. Day1/day3/day7 operational escalation actions
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/ComplianceController.cs](backend/NewKenyaAPI/Controllers/ComplianceController.cs)
- Missing implementation:
  - Action automation for each level
  - Regional leader follow-up notifications at escalation threshold

#### 28. Personalized reminder content and nearest IEC office guidance
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Controllers/ComplianceController.cs](backend/NewKenyaAPI/Controllers/ComplianceController.cs)
- Missing implementation:
  - Personalized message templates and geo-linked nearest office helper

### 7) Election Command and Integrity

#### 29. Post-submission validation workflow for election anomalies
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/ResultsController.cs](backend/NewKenyaAPI/Controllers/ResultsController.cs)
- Missing implementation:
  - Reviewer actions to validate/reject pending results and track reviewer identity

#### 30. Geotag/device integrity for election submissions
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Models/CampaignDomain.cs](backend/NewKenyaAPI/Models/CampaignDomain.cs)
- Missing implementation:
  - Submission metadata: latitude, longitude, device fingerprint, confidence/tamper checks

#### 31. Duplicate detection across submitters for same polling station/window
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/ResultsController.cs](backend/NewKenyaAPI/Controllers/ResultsController.cs)
- Missing implementation:
  - Multi-submitter conflict handling and adjudication path

### 8) Analytics, Leaderboards, and Performance Management

#### 32. Full hierarchy analytics (region/county/sub-county/ward)
- Status: Partial
- Evidence: [src/components/admin/AdminCommandCenter.jsx](src/components/admin/AdminCommandCenter.jsx), [backend/NewKenyaAPI/Controllers/CommandDashboardController.cs](backend/NewKenyaAPI/Controllers/CommandDashboardController.cs)
- Missing implementation:
  - Deep drilldown analytics across all hierarchy levels and dimensions

#### 33. Weekly automated reporting
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Services](backend/NewKenyaAPI/Services)
- Missing implementation:
  - Scheduled report generation and distribution pipeline

#### 34. Gamification depth (badges/titles/incentives)
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/LeaderboardController.cs](backend/NewKenyaAPI/Controllers/LeaderboardController.cs), [backend/NewKenyaAPI/Services/LeaderboardService.cs](backend/NewKenyaAPI/Services/LeaderboardService.cs)
- Missing implementation:
  - Badge engine, progression tiers, award rules, and recognition surfaces

#### 35. Verification-aware points model
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Services/LeaderboardService.cs](backend/NewKenyaAPI/Services/LeaderboardService.cs)
- Missing implementation:
  - Scoring dimensions for identity compliance and downline quality integrity

#### 36. User-facing leaderboard and rank journey
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/LeaderboardController.cs](backend/NewKenyaAPI/Controllers/LeaderboardController.cs), [src/App.jsx](src/App.jsx)
- Missing implementation:
  - Frontend leaderboard route/views, user rank context, regional/county views

### 9) Geolocation and Mapping

#### 37. Geolocation entity and ingestion pipeline
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Models](backend/NewKenyaAPI/Models), [backend/NewKenyaAPI/Controllers](backend/NewKenyaAPI/Controllers)
- Missing implementation:
  - Geo table(s), APIs, ingestion cadence, and privacy controls

#### 38. Field deployment and coverage maps
- Status: Not Adapted
- Evidence: [src/components/admin](src/components/admin)
- Missing implementation:
  - Ward/constituency/sub-county/county/region map overlays with coverage KPIs

### 10) Security, Audit, and Operations

#### 39. Verification/task/messaging audit log system
- Status: Not Adapted
- Evidence: [backend/NewKenyaAPI/Data/ApplicationDbContext.cs](backend/NewKenyaAPI/Data/ApplicationDbContext.cs)
- Missing implementation:
  - Audit log entity and immutable event writes for critical actions

#### 40. Extended auth controls (beyond current OTP/JWT baseline)
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Controllers/AuthController.cs](backend/NewKenyaAPI/Controllers/AuthController.cs), [backend/NewKenyaAPI/Program.cs](backend/NewKenyaAPI/Program.cs)
- Missing implementation:
  - Full 2FA policy coverage and stronger auth-control surfaces per role risk

#### 41. Command analytics observability and operational health
- Status: Partial
- Evidence: [backend/NewKenyaAPI/Program.cs](backend/NewKenyaAPI/Program.cs)
- Missing implementation:
  - Dedicated operational metrics and dashboard health checks for command workflows

### 11) Product Surface and Platform Alignment

#### 42. Mobile-first app layer from exported chat
- Status: Not Adapted
- Evidence: [src/App.jsx](src/App.jsx), [package.json](package.json)
- Missing implementation:
  - Native mobile client implementation (if strict alignment to exported chat blueprint is required)

#### 43. Broadcast admin UX parity with blueprint
- Status: Partial
- Evidence: [src/components/admin/AdminMessageComposer.jsx](src/components/admin/AdminMessageComposer.jsx)
- Missing implementation:
  - Full compose/audience/schedule/reporting dashboard with historical campaign message performance

#### 44. Candidate-level org chart and strategic command role management
- Status: Not Adapted
- Evidence: [src/components/admin/AdminCampaignTeam.jsx](src/components/admin/AdminCampaignTeam.jsx)
- Missing implementation:
  - Structured command org chart management (chairperson, strategists, counsel chains, unit command ownership)

## Notes
- Existing implementation already covers major foundations: OTP challenge flow, hierarchy constraints (max 10 downlines), messaging queueing with channel support, compliance reminders (manual queueing), leaderboard recalculation, and election submission/aggregation.
- This document intentionally captures only remaining requirement gaps from the exported chat blueprint.

## Suggested Next Deliverable
Create an implementation tracker with columns:
1. Gap ID
2. Priority (P0/P1/P2)
3. Backend tasks
4. Frontend tasks
5. Data model changes
6. Acceptance criteria
7. Test coverage requirements
