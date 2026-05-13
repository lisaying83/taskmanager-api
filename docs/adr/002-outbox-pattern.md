# ADR-002: Outbox Pattern for Domain Event Persistence

**Date:** 2026-05-12  
**Status:** Accepted

## Context

When a task is created, we need to record an audit event. The naive approach — calling a message broker directly inside the handler — creates a dual-write problem: the database write might succeed while the broker call fails, losing the event silently.

## Decision

Domain events raised by entities are converted to `OutboxMessage` records inside `AppDbContext.SaveChangesAsync`. Both the business data and the event record are written in the **same database transaction**.

```
Handler → entity.Complete() → TaskCompletedEvent raised
       → SaveChangesAsync() → INSERT outbox_messages (same tx)
```

A background processor (not yet implemented) will read unprocessed outbox messages and publish them to a message broker or process them directly.

## Consequences

**Positive:**
- Guaranteed at-least-once delivery: if the transaction commits, the event is not lost
- No distributed transaction needed
- Audit trail available even before a message broker is wired

**Negative:**
- Slightly higher write amplification (one extra row per event)
- Requires a background processor to actually dispatch events

## Alternatives Considered

- **Direct broker publish in handler**: simple but fragile — loses events on broker failure
- **Domain event dispatcher after SaveChanges**: still at risk if the process crashes between SaveChanges and dispatch
