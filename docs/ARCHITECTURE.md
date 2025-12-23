# Architecture

## Layers
- Domain: business rules, workflow, invariants
- Application: use cases + interfaces (repositories)
- Infrastructure: EF Core, persistence, external implementations
- API: Controllers, auth, DTOs, request/response mapping

## Principle
Business rules live in Domain, not in controllers and not in EF entities directly.
