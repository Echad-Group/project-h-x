# MAUI Mobile App Audit - 2026-03-29

## Scope

Reviewed the MAUI mobile app for UI/UX consistency, separation of concerns, logic correctness, and crash/fault tolerance across authentication, navigation, tasks, result submission, profile management, and public-content flows.

## Findings

### 1. Session handling was optimistic rather than authoritative

- `SessionService` treated any stored token as an active session.
- Expired JWTs were not detected before navigation guards ran.
- `AuthTokenHandler` cleared credentials on `401` but the app did not actively redirect to a safe route.

Impact: users could appear signed in until a later API failure, then become stranded on protected UI.

### 2. ViewModels owned Shell navigation directly

- Auth, profile, and content viewmodels called `Shell.Current.GoToAsync(...)` directly.
- This coupled presentation logic to shell routing details and made redirection behavior harder to harden or test.

Impact: navigation behavior was duplicated and more brittle during auth/session transitions.

### 3. Result submission had avoidable failure paths

- `SubmitResultViewModel.LoadAsync()` did not guard outbox initialization/status refresh failures.
- The viewmodel subscribed to the singleton outbox `StatusChanged` event and never unsubscribed.
- Submission validation did not reject impossible totals such as votes exceeding registered voters.

Impact: page activation could throw, repeated visits could leak handlers, and invalid field combinations could be queued or submitted.

### 4. One visible tasks UX bug existed in the page markup

- The empty-state label on the Tasks page rendered unconditionally, even when tasks were present.

Impact: contradictory UI reduced trust in task state.

### 5. Error handling was mostly per-viewmodel and string-based

- Many viewmodels caught `Exception` and surfaced raw `ex.Message`.
- API error normalization existed in several services but was duplicated.

Impact: messaging remained inconsistent and recovery behavior stayed distributed.

## Remediation Implemented In This Pass

1. Make session state authoritative by validating JWT expiry and broadcasting session changes.
2. Redirect to login automatically after unauthorized or expired sessions.
3. Introduce an app navigation service and move high-traffic viewmodels off direct Shell calls.
4. Harden result submission load/status/event lifecycles and add cross-field validation.
5. Fix the Tasks empty-state visibility bug.
6. Extend tests around session expiry/navigation-related behavior where feasible.

## Deferred Follow-up

1. Break `ProfileViewModel` into focused sub-viewmodels/sections.
2. Consolidate API response parsing into a shared base service/helper.
3. Add more UI-state properties such as explicit empty/loading/partial-failure states for content-heavy pages.