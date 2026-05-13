# ADR-003: Value Objects for Task Title and Description

**Date:** 2026-05-12  
**Status:** Accepted

## Context

Validation of task fields (title length, empty checks) could live in the API layer, the Application layer, or the Domain. The choice affects how easy it is to enforce rules consistently.

## Decision

`TaskTitle` and `TaskDescription` are Value Objects in the Domain layer. They are immutable, created via a static `Create` factory that enforces rules, and throw `DomainException` on violation.

```csharp
// Invalid title cannot exist at the type level
var title = TaskTitle.Create(""); // throws DomainException
```

## Consequences

**Positive:**
- Rules are enforced regardless of which layer creates the entity
- Tests for validation logic live in `Domain.Tests` with zero dependencies
- `TaskItem.Title` is never null or invalid — the type system guarantees it

**Negative:**
- EF Core requires `OwnsOne` configuration to map Value Objects to columns
- Slightly more boilerplate than simple string properties

## Alternatives Considered

- **Validation in FluentValidation only**: catches errors at the API boundary but allows invalid state to be constructed in tests or non-API code paths
- **Data Annotations**: tightly coupled to ASP.NET, not portable to Domain layer
