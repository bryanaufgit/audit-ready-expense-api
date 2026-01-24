# Audit Ready Expense API

![CI](../../actions/workflows/ci.yml/badge.svg)

A backend-focused expense workflow API built with **ASP.NET Core (.NET 10)**, designed to demonstrate **clean software engineering**, **domain-driven design**, and **production-grade testing**.

The project intentionally focuses on **business logic, architecture, and reliability** rather than UI.

---

## Project Goal

This project demonstrates how a real-world **enterprise expense approval workflow** can be implemented in a clean, testable, and auditable way.

Key design goals:
- Clear separation of concerns (Domain / Application / Infrastructure / API)
- Explicit business rules and invariants
- Full audit trail of all workflow actions
- Real integration tests against a real database
- CI pipeline that validates the system end-to-end

---

## Architecture Overview

The solution follows a **Clean Architecture** approach:

AuditReadyExpense.Domain
└─ Core business rules (entities, value objects, domain exceptions)

AuditReadyExpense.Application
└─ Use cases / workflow orchestration

AuditReadyExpense.Infrastructure
└─ Persistence (EF Core, PostgreSQL, repositories)

AuditReadyExpense.Api
└─ HTTP API (controllers, middleware, Swagger)

AuditReadyExpense.Tests
└─ Unit tests (Domain)
└─ Integration tests (API + PostgreSQL via Testcontainers)

**Key principle:**  
The Domain layer has **no dependencies** on infrastructure, HTTP, or frameworks.

---

## Expense Workflow

An expense follows a strict lifecycle:

Draft → Submitted → Approved
                ↘
                Rejected

### Rules enforced by the Domain:
- Amount must be greater than zero
- Title is required
- Only submitted expenses can be approved or rejected
- The creator **cannot approve their own expense**
- Every state transition produces an **immutable audit event**

---

## Audit Trail

Every workflow action creates an audit entry containing:
- FromStatus → ToStatus
- Actor (user id)
- Timestamp
- Optional reason (for rejection)

Audit events are persisted and can be queried via the API.

---

## Getting Started (Local)

### Prerequisites
- .NET SDK 10
- Docker (Docker Desktop)

### Start the database
```bash
docker compose up -d

Run the API

dotnet run --project AuditReadyExpense.Api
```
Swagger UI will be available at the URL shown in the console.


## Testing
Run all tests

dotnet test

Test setup
	•	Unit tests validate domain rules
	•	Integration tests:
	•	Start a real PostgreSQL container using Testcontainers
	•	Apply EF Core migrations automatically
	•	Exercise real HTTP endpoints end-to-end

No external services are required.

⸻

## API Usage (Examples)

During development, a temporary header is used instead of real authentication.

Required header

X-Actor-UserId: <guid>


⸻

# Create draft expense

curl -X POST http://localhost:5000/api/expenses \
  -H "Content-Type: application/json" \
  -H "X-Actor-UserId: 11111111-1111-1111-1111-111111111111" \
  -d '{
    "title": "Taxi to customer site",
    "amount": 12.50
  }'

# Submit expense

curl -X POST http://localhost:5000/api/expenses/{id}/submit \
  -H "X-Actor-UserId: 11111111-1111-1111-1111-111111111111"

# Approve expense (different user)

curl -X POST http://localhost:5000/api/expenses/{id}/approve \
  -H "X-Actor-UserId: 22222222-2222-2222-2222-222222222222"

# View audit trail

curl http://localhost:5000/api/expenses/{id}/audit


⸻

## Engineering Highlights
	•	Clean Architecture with strict dependency boundaries
	•	Explicit domain exceptions instead of generic runtime errors
	•	Workflow modeled as a state machine
	•	Global exception handling middleware
	•	PostgreSQL via EF Core with migrations
	•	Integration tests using Testcontainers
	•	CI pipeline validating build and tests on every push

⸻

## Notes
	•	Authentication is intentionally simplified for local development.
	•	The API is designed to be extended with JWT + role-based access control.
	•	Swagger is used only as a developer aid, not as a core dependency.

⸻

## License

MIT – for demonstration and educational purposes.