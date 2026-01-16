# Post Noctum — v1 IPC Protocol (Local-Only)

Post Noctum CLI client commands communicate with the local daemon over a local-only IPC transport.

## Goals

- Local-only (no TCP listening sockets).
- Boring and inspectable (JSON messages).
- Versioned and backward-compatible.

## Transports

- Linux/macOS: Unix domain socket (UDS)
  - Default: `/run/postnoctum/postnoctum.sock`
- Windows (planned): named pipe

## Framing

- Line-delimited JSON (NDJSON): one JSON object per line.
- Requests and responses are single-line JSON.
- Server may return multiple JSON lines for streaming responses (planned), but v1 assumes request/response pairs.

## Version Negotiation

All requests include:

- `protocol_version`: integer
- `client_version`: string

Server responds with:

- `protocol_version`: integer
- `server_version`: string

The server rejects unsupported protocol versions with an error response.

## Message Shapes

### Request

```json
{
  "protocol_version": 1,
  "request_id": "uuid-or-ulid",
  "method": "Status.Get",
  "params": {},
  "client_version": "0.1.0"
}
```

### Success Response

```json
{
  "protocol_version": 1,
  "request_id": "uuid-or-ulid",
  "ok": true,
  "result": {"...": "..."},
  "server_version": "0.1.0"
}
```

### Error Response

```json
{
  "protocol_version": 1,
  "request_id": "uuid-or-ulid",
  "ok": false,
  "error": {
    "code": "DaemonUnavailable|BadRequest|NotFound|Internal|UnsupportedProtocol",
    "message": "human readable",
    "details": {"...": "optional"}
  },
  "server_version": "0.1.0"
}
```

## Methods (v1)

- `Status.Get`
- `Incidents.List`
- `Incidents.Explain` (params: `incident_id`)
- `Rules.List`
- `Rules.Enable` (params: `rule_id`)
- `Rules.Disable` (params: `rule_id`)
- `Rules.Test` (params: `rule_id`)
- `Alerts.List`
- `Alerts.Suppress` (params: `fingerprint`)
- `Alerts.Unsuppress` (params: `fingerprint`)
- `Alerts.History`

Exact payload schemas should be versioned and treated as compatibility-sensitive.

## Security

- Socket permissions should restrict access to appropriate local users/groups.
- No network ports are opened.
- No outbound communication is performed through IPC.
