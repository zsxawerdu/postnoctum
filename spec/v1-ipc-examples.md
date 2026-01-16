# Post Noctum — v1 IPC Examples (NDJSON)

This document provides concrete v1 examples for the local IPC protocol described in `spec/v1-ipc.md`.

All examples are single-line JSON objects intended to be sent/received as NDJSON over the local transport.

## Conventions

- `request_id` should be a UUID or ULID.
- `protocol_version` is `1` in v1.
- `client_version` and `server_version` are semantic versions.

## `Status.Get`

### Request

```json
{"protocol_version":1,"request_id":"01J3J9ZJ5V4F1JH0K2E7W2D8G7","method":"Status.Get","params":{},"client_version":"0.1.0"}
```

### Response (OK)

```json
{"protocol_version":1,"request_id":"01J3J9ZJ5V4F1JH0K2E7W2D8G7","ok":true,"result":{"daemon":{"pid":1234,"started_utc":"2026-01-16T12:00:00Z","uptime_seconds":2100,"version":"0.1.0"},"db":{"path":"/var/lib/postnoctum/postnoctum.db","schema_version":1},"collectors":{"journald":{"enabled":true,"mode":"journalctl","healthy":true,"last_event_utc":"2026-01-16T12:34:12Z"},"dmesg":{"enabled":true,"healthy":true},"proc":{"enabled":true,"healthy":true}},"rules":{"enabled":12,"disabled":0},"incidents":{"open":1,"resolved":0},"alerts":{"suppressed":2}},"server_version":"0.1.0"}
```

### Response (Error)

Example: internal error.

```json
{"protocol_version":1,"request_id":"01J3J9ZJ5V4F1JH0K2E7W2D8G7","ok":false,"error":{"code":"Internal","message":"failed to query database","details":{"operation":"Status.Get"}},"server_version":"0.1.0"}
```

## `Incidents.List`

### Request

```json
{"protocol_version":1,"request_id":"01J3JA0Q40E0FNR2K2YQ7M6T6T","method":"Incidents.List","params":{"limit":50,"include_resolved":false},"client_version":"0.1.0"}
```

### Response (OK)

```json
{"protocol_version":1,"request_id":"01J3JA0Q40E0FNR2K2YQ7M6T6T","ok":true,"result":{"incidents":[{"incident_id":"incident-8f32c","severity":3,"state":"open","title":"API restart loop","fingerprint":"restartloop+oom+deploy:api","first_seen_utc":"2026-01-16T11:58:01Z","last_updated_utc":"2026-01-16T12:33:47Z","confidence":0.82,"summary":"api restarted 6 times after deploy; OOM signatures observed"}]},"server_version":"0.1.0"}
```

## `Incidents.Explain`

### Request

```json
{"protocol_version":1,"request_id":"01J3JA2K3T1M02XH0J4P1JZ4Z8","method":"Incidents.Explain","params":{"incident_id":"incident-8f32c","format":"json"},"client_version":"0.1.0"}
```

### Response (OK)

The full `result` shape is defined by `spec/v1-explain-payload.md`.

```json
{"protocol_version":1,"request_id":"01J3JA2K3T1M02XH0J4P1JZ4Z8","ok":true,"result":{"explain_version":1,"incident":{"incident_id":"incident-8f32c","severity":3,"state":"open","title":"API restart loop","fingerprint":"restartloop+oom+deploy:api","first_seen_utc":"2026-01-16T11:58:01Z","last_updated_utc":"2026-01-16T12:33:47Z","confidence":0.82},"reasoning":{"summary":"After a deploy, the api service entered a restart loop. Journald shows repeated OOMKills shortly after process start.","confidence_assessment":"High confidence: multiple independent OOMKill indicators across journald and kernel messages.","suggested_next_checks":["Check memory limits for api service (cgroup / container)","Inspect recent deploy diff for memory regression","Confirm node memory pressure and other workloads"],"triggering_rules":[{"rule_id":"linux.service.restartloop","title":"Service restart loop","matched":true},{"rule_id":"linux.kernel.oomkill","title":"OOMKill detected","matched":true}]},"evidence":{"timeline":[{"event_id":"01J3JA2YQJ3V2Q4A6J0H2R5D8T","observed_utc":"2026-01-16T12:01:02Z","source":"journald","event_type":"log","payload":{"version":1,"unit":"api.service","priority":3,"message":"Main process exited, code=killed, status=9/KILL"}},{"event_id":"01J3JA3A0Q8X9P6G8G3N8Q7D5M","observed_utc":"2026-01-16T12:01:03Z","source":"dmesg","event_type":"log","payload":{"version":1,"message":"Out of memory: Killed process 12345 (api) total-vm:..."}}]}},"server_version":"0.1.0"}
```

## Unsupported Protocol Version

### Request

```json
{"protocol_version":99,"request_id":"01J3JA4B7M9R4X1W1FZ0R2C7E1","method":"Status.Get","params":{},"client_version":"0.1.0"}
```

### Response (Error)

```json
{"protocol_version":1,"request_id":"01J3JA4B7M9R4X1W1FZ0R2C7E1","ok":false,"error":{"code":"UnsupportedProtocol","message":"unsupported protocol_version 99; supported: 1","details":{"supported":[1]}},"server_version":"0.1.0"}
```
