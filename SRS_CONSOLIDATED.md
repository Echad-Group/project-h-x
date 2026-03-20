# SOFTWARE REQUIREMENTS SPECIFICATION (SRS)
## Presidential Campaign Networking & Command Platform (Kenya)
(v1.0 + v2.0 + v3.0 consolidated)

---

## 1. INTRODUCTION

### 1.1 Purpose
Define a unified, mission-critical specification for a national-scale hierarchical campaign networking system for Kenya.
Covers full functional and non-functional requirements, architecture, security, workflows, roles, API, database, monitoring, and operational readiness.

### 1.2 Scope
Platform supports:
- Nationwide hierarchical mobilization
- Identity verification (NIDA + selfie, voter card)
- Strict 10-downline max recruiting and geo hierarchy
- Task execution & completion tracking
- WhatsApp + push alerts + broadcast systems
- Analytics, leaderboard, compliance, command and election-day operations

### 1.3 Definitions
- Downline: direct subordinate user
- Leader: user with downlines
- Verification: NIDA + selfie + Admin review
- Compliance: verified voter card
- Node: hierarchical user position (region/county/etc.)

---

## 2. SYSTEM OVERVIEW

### 2.1 Product Perspective
- Mobile App: .NET MAUI (or equivalent cross-platform .NET framework) for Android/iOS/Desktop
- Admin Web Portal: React.js
- API Backend: .NET Core (C#) or Node.js; .NET-focused stack preferred for in-house services
- DB: initially SQLite (local and lightweight), longer-term MySQL + MongoDB hybrid for structured and document workloads; optionally SQL Server for transactional services
- Cache: .NET in-memory cache (MemoryCache) / Redis alternative (e.g., Microsoft.Extensions.Caching.Memory or 3rd-party no-redis-delivery for .NET)
- Message queue: RabbitMQ/Kafka (v3), with local queue fallback for VPS (e.g., MSMQ or JetStream)
- Storage: file system / network file share on VPS (no cloud/object store dependency)
- External: Twilio WhatsApp + AI face recognition (hosted service APIs)
- Hosting: VPS (self-managed Linux/Windows), no AWS/Azure/GCP managed PaaS
- Optional: NGINX/Caddy gateway, reverse proxy, TLS on VPS

### 2.2 Product Functions
- Registration & identity verification
- Downline/leadership tree management
- Tasks/quota/points/gamification
- Messaging (WhatsApp + push + in-app)
- Voter card compliance reminder engine
- Leaderboard tracking
- Admin analytics dashboard
- Election-day reporting & PVT
- Logging/monitoring + disaster recovery

### 2.3 Operating Environment
- Mobile: Android & iOS (React Native)
- Backend: self-hosted VPS (Linux/Windows) using Docker containers for portability; optional on-prem deployment
- DB: SQLite for initial MVP (local file), with migration path to MySQL (OLTP) + MongoDB (analytics/document data) and optional SQL Server; no PostgreSQL dependency
- Cache: .NET in-memory or lightweight distributed cache suitable on VPS
- On-prem/hybrid in v3 using microservices
- Client fallback/offline experiences targeted

---

## 3. ROLES & ACCESS CONTROL

### 3.1 Roles
- Super Admin (HQ)
- Regional Leader
- County Leader
- Sub-County Leader
- Constituency Leader
- Ward Leader
- Polling Station Agent (v3)
- Volunteer

### 3.2 RBAC Matrix (basics)
- Broadcast: Admin yes, Region limited, others no
- Assign tasks: Admin/region/county/ward yes, volunteer no
- View analytics: Admin/region partial; others restricted

### 3.3 Constraints
- 10 direct downlines per user
- No circular parent-child links
- All nodes within same geographic hierarchy for assignment

---

## 4. ARCHITECTURE

### 4.1 High-level layered (v2/v3)
- API Gateway (NGINX/Kong)
- Auth & RBAC service (JWT + OTP)
- User service
- Network service (downline tree engine)
- Task service
- Messaging service
- Compliance/leaderboard service
- Election service
- Analytics + command dashboard
- Monitoring + logging pipeline (Prometheus/Grafana/ELK)

### 4.2 Data flow
1. User action → API gateway
2. microservice → DB write
3. event → queue
4. worker processor → notifications + analytics
5. dashboard updates

### 4.3 External interfaces
- WhatsApp API (Twilio, per-message with send/delivered/read tracking)
- Cloud storage for media (AWS S3)
- AI verification service (face match, fraud detection)
- API gateway (NGINX/Kong)
- SMS/voice OTP gateway
- optional polling station devices

---

## 5. FUNCTIONAL REQUIREMENTS

### 5.1 Registration & Verification
- Fields: name, phone, NIDA upload, optional voter card upload, selfie
- OTP phone verification mandatory
- AI face matching (≥80% threshold)
- Admin review + status: Pending/Verified/Rejected
- Duplicate detection (face + identity + phone)
- Enforce unique phone/user
- Max image size <= 5MB

### 5.2 Downline Hierarchy
- Legal structure:
  - National > Region > County > Sub-County > Constituency > Ward > Polling > Volunteer
- Rules:
  - Max 10 direct children
  - No loops
  - Geographic membership validation
  - Expandable tree UI with real-time counts
- Algorithms:
  - BFS for propagation
  - Depth-limited expansion
  - Mild caching for hierarchy reads

### 5.3 Task Management
- API models:
  - create, assign, get my-tasks, update status
- States:
  - Pending → In Progress → Completed
- Validation:
  - deadline required
  - assigned user must be in downline hierarchy
  - no undocumented complete transitions

### 5.4 Messaging System
- Channels: WhatsApp + push + in-app
- Modes:
  - Broadcast all
  - Targeted by region/role
  - Cascading chain
- Requirements:
  - per-user delivery (no spam groups)
  - statuses: sent/delivered/read/fail
  - rate-limit 100 msg/s server
  - retry queue + dead-letter path

### 5.5 Voter Compliance
- Logic:
  - if voter_card missing → daily reminder
  - escalations:
    - day 3 warning
    - day 7 notify leader
- Cron scheduled 09:00 daily
- audit trail for messages and escalation steps

### 5.6 Leaderboard & Gamification
- point values:
  - +10 per downline
  - +5 per task completed
  - +20 upon voter card verified
- rank levels:
  - national/regional/county
- anti-gaming:
  - duplicate downlines check
  - suspicious activity detection

### 5.7 Admin Dashboard
- counts: total users, new users, verifications
- graph: downline growth timeline
- controls: send broadcast, compliance status, analytics drilldown

### 5.8 Elections command (v3)
- command structure:
  - national war-room, regional centers, county nodes, polling agents
- real-time reporting:
  - geo-tag results, timestamp verify
- parallel vote tabulation:
  - submit --> validate -> county aggregation -> national tally
- integrity:
  - duplicate result detection
  - outlier analysis

---

## 6. API SPECIFICATION

### Auth
- `POST /auth/register`
- `POST /auth/login`
- `POST /auth/verify` (OTP, level)

### Network
- `POST /downlines/add`
- `GET /downlines/:id`
- `GET /downlines/tree`

### Tasks
- `POST /tasks/create`
- `GET /tasks/my-tasks`
- `POST /tasks/assign`
- `POST /tasks/complete`

### Messaging
- `POST /messages/broadcast`
- `POST /messages/target`

### Compliance
- `GET /compliance/summary`
- `POST /compliance/reminder`

### Electorate Reporting (v3)
- `POST /results/submit`
- `GET /results/aggregate`
- `GET /results/status`

---

## 7. DATABASE DESIGN

### 7.1 Data platform strategy
- Phase 1: SQLite (local file-based, minimal ops, easy setup on VPS)
- Phase 2: MySQL (ORM migrations, better concurrency, structured relational data)
- Phase 3: MongoDB for document/analytics-heavy features (logs, event streams, extended audit, geo voters)
- Optional: SQL Server for mission-critical components where T-SQL or enterprise support needed
- No PostgreSQL requirement in this codebase; vendor-neutral SQL layer via ORM (Entity Framework/EF Core) with migration support

### Tables (core)
- `users`
- `downlines`
- `tasks`
- `messages`
- `compliance_reminders`
- `leaderboard_scores`
- `audit_logs`
- `results` (election)
- `roles` / `permissions`

### Key columns

`users`:
- id, name, phone, role, parent_id, location fields, downline_count, verification_status, voter_card_status, id_image_url, selfie_url

`downlines`:
- leader_id, downline_id, added_at

`tasks`:
- id, assigned_to, assigned_by, status, deadline, created_at, updated_at

`messages`:
- id, sender_id, receiver_id, channel, direction, status, event_id, created_at

`results`:
- id, polling_agent_id, constituency_id, candidate_A, candidate_B, status

### Constraints
- PK on id
- FK user parent and downlines
- unique phone
- downline_count <= 10
- proper indexing on role+location+status
- partitioning by date for events and logs (v3)

---

## 8. NON-FUNCTIONAL REQUIREMENTS

### Performance
- 1M+ users, 5M+ concurrent targeted (when scaled to MySQL/MongoDB cluster)
- API <300ms p95 reachable with VPS vertical scaling and microservices
- message latency <5s
- query index for tree traversal
- SQLite for MVP with single instance performance expectations; scale to MySQL and MongoDB for higher concurrency and analytics

### Deployment and hosting
- Self-managed VPS with monitoring and maintenance responsibility
- Backup + restore for SQLite file and MySQL/Mongo DB data
- Automated deployment scripts (Docker Compose / shell scripts)
- TLS via certbot or managed certs on nginx/caddy

### Security
- JWT tokens
- OTP + RBAC
- AES-256 at rest
- HTTPS TLS 1.2+
- role-based access checks
- DDoS mitigation

### Reliability
- 99.9% uptime
- health checks + circuit breaker
- failover DB replicas
- retry for queue/failures

### Scalability
- horizontal auto-scaling
- microservices with stateless apps
- cache layer (Redis)

### Compliance
- Kenya Data Protection Act
- PII encryption
- audit logging

---

## 9. SECURITY ARCHITECTURE

- app layer auth/authorization
- network security groups + WAF
- DB encryption + backup encryption
- intrusion detection
- fraud analytics (duplicate identity, device fingerprinting, behavior)

---

## 10. MONITORING & LOGGING

- logs to ELK stack
- metrics via Prometheus + Grafana
- alerts on critical thresholds
- audit trail for authorization events, data changes

---

## 11. DISASTER RECOVERY

- daily backups
- multi-region replication
- restore test <1 hour
- incremental snapshots and failover process

---

## 12. UI/UX DESIGN

### Screens
- Registration + verification
- Login/OTP
- Profile + downline tree
- Task list/assign
- Leaderboard
- Compliance and messaging
- Admin/analytics dashboard
- Election command view

### Principles
- mobile-first
- offline mode / low bandwidth optimizations
- load deferred resources
- accessible (WCAG basics)

---

## 13. EXTERNAL INTERFACES

- WhatsApp API (Twilio)
- Cloud storage (AWS S3)
- AI face recognition service
- API gateway (NGINX/Kong)
- SMS/voice OTP gateway
- optional polling station devices

---

## 14. FUTURE ENHANCEMENTS

- offline campaign sync
- AI voter behavior prediction
- GIS mapping + heatmaps
- advanced anti-gaming analytics
- machine learning for reports
- full CLI and admin workflow generator

---

## 15. APPENDICES

- Data dictionary (to be detailed)
- sequence diagrams:
  - registration: User→App→API→DB→AI→Admin→User
  - broadcast: Admin→API→Queue→WhatsApp→User
  - result submission: Poll Agent→API→Validation→Aggregate
- migration plan + release checklist (existing project docs)
