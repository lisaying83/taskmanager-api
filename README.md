# TaskManager API

A production-grade task management REST API demonstrating **Clean Architecture**, **CQRS with MediatR**, and the **Outbox Pattern** — built with ASP.NET Core 8, PostgreSQL, and Docker.

## Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                    API Layer                         │
│         Minimal APIs · JWT Auth · Swagger            │
└──────────────────────┬──────────────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────────────┐
│                Application Layer                     │
│   CQRS (Commands/Queries) · MediatR · FluentValidation│
│   ValidationBehavior Pipeline · DTOs                │
└──────────────────────┬──────────────────────────────┘
                       │ depends on
┌──────────────────────▼──────────────────────────────┐
│                  Domain Layer                        │
│   Entities · Value Objects · Domain Events           │
│   Interfaces · Business Rules                        │
└─────────────────────────────────────────────────────┘
                       ▲ implements
┌──────────────────────┴──────────────────────────────┐
│              Infrastructure Layer                    │
│   EF Core + PostgreSQL · JWT Service · BCrypt        │
│   Outbox Pattern · Repository Pattern                │
└─────────────────────────────────────────────────────┘
```

**Dependency rule**: each layer only knows about layers below it. Domain has zero external dependencies.

## Key Design Decisions

### Clean Architecture
Domain and Application layers have no infrastructure dependencies. The `ITaskRepository`, `IUserRepository`, and `IUnitOfWork` interfaces are defined in Domain and implemented in Infrastructure — making the core logic testable without a real database.

### CQRS with MediatR
Commands mutate state; Queries read state. This separation makes each handler small, focused, and independently testable. `ValidationBehavior` runs FluentValidation automatically via the MediatR pipeline before any handler executes.

### Outbox Pattern
Domain events (e.g., `TaskCreatedEvent`) are not published directly to a message broker. Instead, `AppDbContext.SaveChangesAsync` converts them into `OutboxMessage` records persisted in the same database transaction. This guarantees that if the transaction commits, the event is not lost — solving the dual-write problem.

### Value Objects
`TaskTitle` and `TaskDescription` encapsulate validation rules at the type level. You cannot create a `TaskItem` with an invalid title — the domain enforces it, not the API layer.

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core 8 Minimal APIs |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL 16 |
| Auth | JWT Bearer + Refresh Token Rotation |
| Password | BCrypt (work factor 12) |
| Testing | xUnit + FluentAssertions + NSubstitute |
| Containerization | Docker + Docker Compose |

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Run with Docker Compose

```bash
docker-compose up --build
```

API available at: `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

### Run locally (requires PostgreSQL)

```bash
# Update connection string in appsettings.Development.json, then:
dotnet run --project src/TaskManager.Api
```

### Run tests

```bash
dotnet test
```

## API Endpoints

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | No | Authenticate and receive JWT tokens |
| POST | `/api/auth/refresh` | No | Rotate refresh token |
| POST | `/api/tasks` | Yes | Create a new task |
| GET | `/api/tasks` | Yes | List tasks (filterable by status) |
| GET | `/api/tasks/{id}` | Yes | Get task details |
| POST | `/api/tasks/{id}/complete` | Yes | Mark task as done |
| POST | `/api/tasks/{id}/assign` | Yes | Assign task to user |
| GET | `/health` | No | Health check |

## Project Structure

```
TaskManager/
├── src/
│   ├── TaskManager.Domain/          # Entities, Value Objects, Domain Events, Interfaces
│   ├── TaskManager.Application/     # CQRS Handlers, Validators, DTOs
│   ├── TaskManager.Infrastructure/  # EF Core, Repositories, JWT, BCrypt
│   └── TaskManager.Api/             # Minimal API endpoints, Middleware, Program.cs
├── tests/
│   ├── TaskManager.Domain.Tests/    # Pure domain logic tests (no mocks needed)
│   └── TaskManager.Application.Tests/ # Handler tests with NSubstitute mocks
├── docs/
│   └── adr/                         # Architecture Decision Records
└── docker-compose.yml
```

## Test Coverage

21 tests covering:
- `TaskItem` entity: creation, completion, assignment, cancellation, domain event emission
- `User` entity: creation, token management, email normalization
- `CreateTaskHandler`: happy path + user-not-found error path

## Architecture Decision Records

- [ADR-001](docs/adr/001-clean-architecture.md) — Why Clean Architecture over layered MVC
- [ADR-002](docs/adr/002-outbox-pattern.md) — Why Outbox Pattern for domain events
- [ADR-003](docs/adr/003-value-objects.md) — Why Value Objects for title/description

---

*Built as part of a backend engineering portfolio — Week 1 of a 6-week series.*
