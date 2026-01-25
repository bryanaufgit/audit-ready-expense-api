# ADR 0001: Test Layout (Unit / Integration / Robot)

## Context
We want a repository that demonstrates QA engineering:
- clear test levels
- reproducible execution
- CI-friendly reporting (artifacts)
- interview-friendly structure

## Decision
We use three test levels:
1. Unit tests inside the .NET test project (`AuditReadyExpense.Tests`)
2. Integration tests (API + DB) using Testcontainers in the same test project
3. Robot Framework regression tests under `tests/robot` (outside the .NET solution)

## Consequences
- Unit + Integration tests run via `dotnet test`
- Robot tests run via a dedicated script and produce HTML reports
- CI can upload Robot reports as artifacts