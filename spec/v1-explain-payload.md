# Post Noctum — v1 `explain` Payload (JSON)

This document specifies the JSON payload shape returned by:

- CLI: `postnoctum explain <incident-id> --format json`
- IPC: `Incidents.Explain` with `format=json`

## Principles

- Versioned (`explain_version`).
- Deterministic: the same stored evidence produces the same explanation text.
- Honest about uncertainty: missing inputs are stated explicitly.
- No raw telemetry leaves the host by default; JSON here is local.

## Top-Level Shape

```json
{
  "explain_version": 1,
  "incident": {
    "incident_id": "incident-8f32c",
    "severity": 3,
    "state": "open",
    "title": "API restart loop",
    "fingerprint": "restartloop+oom+deploy:api",
    "first_seen_utc": "2026-01-16T11:58:01Z",
    "last_updated_utc": "2026-01-16T12:33:47Z",
    "confidence": 0.82
  },
  "reasoning": {
    "summary": "...",
    "confidence_assessment": "...",
    "triggering_rules": [
      {"rule_id": "...", "title": "...", "matched": true}
    ],
    "suggested_next_checks": ["..."]
  },
  "evidence": {
    "timeline": [
      {
        "event_id": "01J...",
        "observed_utc": "2026-01-16T12:01:02Z",
        "source": "journald",
        "event_type": "log",
        "payload": {"version": 1, "...": "..."}
      }
    ]
  },
  "debug": {
    "inputs": {
      "collectors": [
        {"source": "journald", "available": true, "notes": null}
      ]
    }
  }
}
```

## Field Semantics

### `incident`

- `incident_id`: stable identifier.
- `fingerprint`: stable grouping key.
- `confidence`: 0..1.

### `reasoning`

- `summary`: short causal narrative in plain text.
- `confidence_assessment`: plain text; must mention missing evidence.
- `triggering_rules`: ordered list of rules considered triggering.
- `suggested_next_checks`: ordered list of concrete next steps.

### `evidence.timeline`

An ordered (ascending by `observed_utc`) timeline of evidence events.

- `event_type` is a small controlled vocabulary in v1:
  - `log`
  - `rule_eval`
  - `state_change`
  - `metric` (reserved)

- `payload` is versioned per event type and source.

## Event Payloads (v1)

### journald log payload (`source=journald`, `event_type=log`)

```json
{
  "version": 1,
  "unit": "api.service",
  "priority": 3,
  "message": "Main process exited...",
  "cursor": "s=..."
}
```

Notes:

- `cursor` is optional in v1 (journalctl mode), but reserved for future sd-journal cursor support.

### dmesg log payload (`source=dmesg`, `event_type=log`)

```json
{
  "version": 1,
  "message": "Out of memory: Killed process ..."
}
```

### proc-derived payload (`source=proc`, `event_type=metric`)

```json
{
  "version": 1,
  "metric": "memory.available_bytes",
  "value": 123456789,
  "unit": "bytes"
}
```

### rule evaluation payload (`event_type=rule_eval`)

```json
{
  "version": 1,
  "rule_id": "linux.kernel.oomkill",
  "matched": true,
  "notes": "matched 3 OOMKill signatures in last 5m"
}
```

## `debug`

`debug` exists to support the principle "the system must be debuggable by its users".

- `debug.inputs.collectors` indicates which collectors were available/healthy at the time of explanation generation.
- v1 keeps `debug` small.

## Compatibility

- New fields may be added.
- Existing fields must not change meaning.
- Breaking changes require incrementing `explain_version`.

## ✅ TODOs

### Phase 1
- 🧾 Define markdown/plain text explain formats
- 🧪 Add example payloads for missing evidence

### Phase 2
- 🧠 Implement deterministic narrative generator

### v2 Phase 1
- 📦 Add support for containerized evidence sources
