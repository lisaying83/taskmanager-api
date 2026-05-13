# ADR-001: Clean Architecture over Layered MVC

**Date:** 2026-05-12  
**Status:** Accepted

## Context

The project needed a structural pattern that allows testing business logic without infrastructure dependencies and makes it clear where each type of concern belongs.

## Decision

We adopted Clean Architecture with four explicit layers: Domain, Application, Infrastructure, and API. The dependency rule is enforced: inner layers never reference outer layers.

## Consequences

**Positive:**
- Domain logic is tested without spinning up a database or HTTP server
- Infrastructure can be swapped (e.g., replace PostgreSQL with another DB) without touching business rules
- Each handler is small and has a single reason to change

**Negative:**
- More files and folders than a simple MVC project
- Requires discipline to avoid leaking infrastructure concerns into Application or Domain

## Alternatives Considered

- **Traditional layered MVC**: faster to start, but business logic ends up coupled to EF Core entities and becomes hard to test
- **Vertical Slice Architecture**: valid for larger teams, but overkill for a focused API of this scope
