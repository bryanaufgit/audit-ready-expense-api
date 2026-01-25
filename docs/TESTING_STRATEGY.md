# Testing Strategy

This repository follows a pragmatic test pyramid:

## 1) Unit Tests (fast, isolated)
**Purpose:** Validate domain rules and invariants without infrastructure.
- Location: `AuditReadyExpense.Tests` (Unit)
- Examples:
  - status transitions allowed/forbidden
  - validation (title required, amount > 0)
  - creator cannot approve

## 2) Integration Tests (real components, real DB)
**Purpose:** Validate the system end-to-end at the API boundary with real persistence.
- Location: `AuditReadyExpense.Tests/Integration`
- Uses: Testcontainers (PostgreSQL), EF Core migrations
- Verifies:
  - HTTP endpoints work
  - DB persistence is correct
  - audit events are written

## 3) Blackbox Regression (Robot Framework)
**Purpose:** Treat the API as a black box and validate behavior from the outside.
- Location: `tests/robot`
- Produces: `report.html`, `log.html`, `output.xml`
- Later published as CI artifacts

## Conventions
- Tests must be deterministic (no sleeps, no shared state).
- Prefer stable identifiers over brittle assumptions.
- Assertions should verify observable behavior (status + persisted audit trail).