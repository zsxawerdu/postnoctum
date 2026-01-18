# Post Noctum — v1 CLI Contract

This document refines `docs/CLI.md` into a v1 implementation contract.

## Principles

- Predictable, discoverable, scriptable, boring.
- No hidden behavior.
- No interactive prompts unless explicitly requested.
- Must work over SSH, in minimal terminals, offline.

## Global Flags

- `--config <path>`: path to TOML config file.
- `--state-dir <path>`: overrides default state directory.
- `--log-level <level>`: logging verbosity (logs to stderr).
- `--format <plain|json|markdown>`: output format for human output.

## Output Streams

- Human output and structured output: stdout
- Logs: stderr

Never mix logs with `--format json` output.

## Exit Codes

| Code | Meaning |
| ---: | ------- |
| 0 | Success |
| 1 | User error (invalid input) |
| 2 | Configuration error |
| 3 | Runtime failure |
| 4 | Partial success / degraded |

### Daemon Unreachable Behavior

- `postnoctum status`
  - Prints: service not running / cannot connect
  - Exit: `4` (degraded)

- Other commands (`incidents`, `explain`, `rules`, `alerts`)
  - Print a short message describing the failure and how to start/install
  - Exit: `3` (runtime failure)

## Commands (v1)

### `postnoctum init`

- Creates default config file and directory structure.
- Creates example rules.
- Creates system service template (not installed).
- Does not start the service.

### `postnoctum run`

- Runs daemon in foreground.
- Logs verbosely.
- Prints rule evaluations and reasoning steps to logs.

### `postnoctum service install`

Linux-first systemd behavior:

- Installs systemd unit.
- Enables at boot.
- Verifies permissions.

Must never:

- Start automatically without explicit confirmation.
- Modify firewall rules.
- Open network ports.

### `postnoctum status`

- Displays service health.
- Displays active incidents count.
- Displays suppressed alerts count.
- Displays rule evaluation state.

Default output is plain text.

### `postnoctum incidents`

Lists known incidents.

Default output fields:

- Incident ID
- Severity
- Affected service/host
- Current state
- First seen / last updated

### `postnoctum explain <incident-id>`

Outputs a full local explanation including:

- Triggering rules
- Evidence timeline
- Reasoning summary
- Confidence assessment
- Suggested next checks

Formats:

- Default: plain text
- Optional: `--format markdown`, `--format json`

### `postnoctum rules`

- `list`
- `enable <rule-id>`
- `disable <rule-id>`
- `test <rule-id>`

Rule changes must be explicit and logged.

### `postnoctum alerts`

- `list`
- `suppress <fingerprint>`
- `unsuppress <fingerprint>`
- `history`

Alert suppression must never affect reasoning (only notification/alerting behavior).

## Compatibility

Breaking CLI changes require major version bump and migration notes.

## ✅ TODOs

### Phase 1
- 🔧 CLI: human output formatting for status/incidents/explain
- 🧭 CLI: `init` + `service install` workflows
- 🧾 CLI: JSON errors for invalid input (explain, unknown subcommands)

### Phase 2
- 🔌 IPC: daemon server + version negotiation
- 🧠 Daemon: collectors (journald/dmesg/proc) + rule engine

### v2 Phase 1
- 🪟 Windows named pipe IPC
