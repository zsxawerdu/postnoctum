# Post Noctum — v1 SQLite Schema

Post Noctum stores local state and history in SQLite to support:

- Active incident tracking
- Alert suppression
- Evidence timelines for `postnoctum explain`

The schema is designed to be inspectable, versioned, and migratable.

## Database Location (Linux default)

- `/var/lib/postnoctum/postnoctum.db`

## Migrations

- A `meta` table stores the current schema version.
- v1 starts at schema version `1`.
- Future migrations are applied by the daemon at startup.

## Tables (v1)

### `meta`

- `key TEXT PRIMARY KEY`
- `value TEXT NOT NULL`

Keys:

- `schema_version`: integer stored as text
- `created_utc`: ISO-8601 string

### `incidents`

Represents the current known incident state.

- `incident_id TEXT PRIMARY KEY`
- `severity INTEGER NOT NULL`
- `state TEXT NOT NULL` (e.g. `open`, `mitigated`, `resolved`)
- `title TEXT NOT NULL`
- `fingerprint TEXT NOT NULL`
- `first_seen_utc TEXT NOT NULL`
- `last_updated_utc TEXT NOT NULL`
- `confidence REAL NOT NULL` (0..1)
- `summary TEXT NOT NULL` (short human summary)
- `explanation_markdown TEXT NULL` (optional cached narrative)

Indexes:

- `incidents_fingerprint_idx` on `fingerprint`
- `incidents_last_updated_idx` on `last_updated_utc`

### `evidence_events`

Append-only event store for explanation timelines.

- `event_id TEXT PRIMARY KEY` (uuid/ulid)
- `incident_id TEXT NOT NULL`
- `event_type TEXT NOT NULL` (e.g. `log`, `metric`, `rule_eval`, `state_change`)
- `source TEXT NOT NULL` (e.g. `journald`, `dmesg`, `proc`)
- `observed_utc TEXT NOT NULL`
- `payload_json TEXT NOT NULL` (versioned payload)

Foreign keys:

- `incident_id` references `incidents(incident_id)`

Indexes:

- `evidence_events_incident_time_idx` on `(incident_id, observed_utc)`

### `rules`

Tracks rule enable/disable state and metadata.

- `rule_id TEXT PRIMARY KEY`
- `enabled INTEGER NOT NULL` (0/1)
- `title TEXT NOT NULL`
- `description TEXT NOT NULL`
- `updated_utc TEXT NOT NULL`

### `alert_suppressions`

Tracks alert suppression by fingerprint.

- `fingerprint TEXT PRIMARY KEY`
- `reason TEXT NULL`
- `created_utc TEXT NOT NULL`
- `expires_utc TEXT NULL`

### `alerts_history`

Audit trail of emitted alerts (summaries only; no raw telemetry shipped).

- `alert_id TEXT PRIMARY KEY`
- `fingerprint TEXT NOT NULL`
- `incident_id TEXT NULL`
- `severity INTEGER NOT NULL`
- `created_utc TEXT NOT NULL`
- `payload_json TEXT NOT NULL`

Indexes:

- `alerts_history_fingerprint_time_idx` on `(fingerprint, created_utc)`

## Retention

A background retention job prunes:

- `evidence_events` older than `retention.raw_events_days`
- resolved incidents older than `retention.incident_days` (and their linked evidence)

Retention must be explicit, logged, and configurable.

## Notes

- All timestamps are stored as UTC ISO-8601 strings for inspectability.
- Payload JSON must include a `version` field to allow evolution.
- The schema intentionally favors boring inspectability over maximal normalization.
