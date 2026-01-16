# Post Noctum — v1 Message Types (JSON DTOs)

This document defines shared JSON message/DTO shapes used across:

- IPC responses (`spec/v1-ipc.md`)
- CLI `--format json` output

The goal is to keep payloads consistent, versioned, and backward-compatible.

## Conventions

- All timestamps are UTC ISO-8601 strings (e.g. `2026-01-16T12:33:47Z`).
- Identifiers are opaque strings (UUID/ULID encouraged).
- New fields may be added at any time.
- Existing fields must not change meaning.

## Error Codes

Used in IPC error responses (`error.code`). CLI JSON errors should reuse the same values.

- `DaemonUnavailable`: cannot connect to local daemon
- `UnsupportedProtocol`: protocol version not supported
- `BadRequest`: invalid params, missing required fields
- `NotFound`: requested resource does not exist
- `Conflict`: state conflict (e.g. already enabled/disabled)
- `ConfigError`: configuration invalid
- `Internal`: unexpected server failure

## `IncidentSeverity`

Integer (v1):

- `0`: unknown
- `1`: info
- `2`: warning
- `3`: critical
- `4`: emergency

(Exact mapping can evolve, but integers remain stable.)

## `IncidentState`

String enum (v1):

- `open`
- `mitigated`
- `resolved`

## `IncidentSummary`

Used by:

- IPC: `Incidents.List` results
- CLI: `postnoctum incidents --format json`

```json
{
  "incident_id": "incident-8f32c",
  "severity": 3,
  "state": "open",
  "title": "API restart loop",
  "fingerprint": "restartloop+oom+deploy:api",
  "first_seen_utc": "2026-01-16T11:58:01Z",
  "last_updated_utc": "2026-01-16T12:33:47Z",
  "confidence": 0.82,
  "summary": "api restarted 6 times after deploy; OOM signatures observed"
}
```

## `RuleSummary`

Used by:

- IPC: `Rules.List` results
- CLI: `postnoctum rules list --format json`

```json
{
  "rule_id": "linux.kernel.oomkill",
  "title": "OOMKill detected",
  "description": "Detects kernel OOM kill signatures.",
  "enabled": true,
  "updated_utc": "2026-01-16T12:00:00Z"
}
```

## `AlertSuppression`

Used by:

- IPC: `Alerts.List` results
- CLI: `postnoctum alerts list --format json`

```json
{
  "fingerprint": "restartloop+oom+deploy:api",
  "reason": "known during load test",
  "created_utc": "2026-01-16T12:10:00Z",
  "expires_utc": null
}
```

## `EvidenceEvent`

Used by:

- IPC: `Incidents.Explain` (`evidence.timeline[]`)
- Storage: `evidence_events.payload_json` stores versioned payloads

```json
{
  "event_id": "01J3JA2YQJ3V2Q4A6J0H2R5D8T",
  "observed_utc": "2026-01-16T12:01:02Z",
  "source": "journald",
  "event_type": "log",
  "payload": {
    "version": 1,
    "unit": "api.service",
    "priority": 3,
    "message": "Main process exited...",
    "cursor": null
  }
}
```

### `event_type` values (v1)

- `log`
- `rule_eval`
- `state_change`
- `metric` (reserved)

## `StatusSnapshot`

Used by:

- IPC: `Status.Get` result
- CLI: `postnoctum status --format json`

```json
{
  "daemon": {
    "pid": 1234,
    "started_utc": "2026-01-16T12:00:00Z",
    "uptime_seconds": 2100,
    "version": "0.1.0"
  },
  "db": {
    "path": "/var/lib/postnoctum/postnoctum.db",
    "schema_version": 1
  },
  "collectors": {
    "journald": {"enabled": true, "mode": "journalctl", "healthy": true, "last_event_utc": "2026-01-16T12:34:12Z"},
    "dmesg": {"enabled": true, "healthy": true},
    "proc": {"enabled": true, "healthy": true}
  },
  "rules": {"enabled": 12, "disabled": 0},
  "incidents": {"open": 1, "resolved": 0},
  "alerts": {"suppressed": 2}
}
```

## Versioning Notes

- IPC has `protocol_version` (transport-level) and method result shapes that must remain stable.
- `explain` has `explain_version` (payload-level).
- Stored payloads inside SQLite must include a payload `version` field.
