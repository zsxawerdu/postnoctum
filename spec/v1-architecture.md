# Post Noctum — v1 Architecture Spec

This spec captures the agreed v1 architecture decisions for Post Noctum.

Post Noctum is local-first. Telemetry stays on-host. Only meaning may leave the host, and only through explicit, operator-reviewed outbound integrations (out of scope for v1).

## Goals (v1)

- Provide a lifeline-grade CLI that is predictable, scriptable, and boring.
- Run fully offline and in minimal terminals (no color dependency, SSH-safe).
- Operate primarily on Linux first, with a cross-platform architecture that can extend to macOS/Windows.
- Capture enough evidence + history to produce a high-quality `postnoctum explain <incident-id>` output.

## Non-Goals (v1)

- Centralized telemetry, remote dashboards, or streaming raw signals off-host.
- Automatic outbound network actions.
- Kubernetes and Docker ingestion (planned v2).
- `sd-journal` native bindings (planned v2; v1 uses `journalctl`).

## Language + Packaging

- Implementation language: C# (.NET).
- Distribution: a single self-contained executable named `postnoctum` per target OS/arch.
- The executable uses a "kubectl pattern": one file on disk, multiple subcommands.

## Process Model (Hybrid Daemon + CLI)

One binary supports both a long-running daemon role and a short-lived CLI client role.

- `postnoctum run`
  - Runs the daemon in the foreground.
  - Intended for debugging, development, and system service execution.
  - Evaluates rules continuously and persists state.

- Other commands (client mode)
  - `postnoctum status`
  - `postnoctum incidents`
  - `postnoctum explain <incident-id>`
  - `postnoctum rules ...`
  - `postnoctum alerts ...`

These client commands connect to a local daemon over a local-only IPC transport (see `spec/v1-ipc.md`).

### Daemon Requirement

- Client commands **assume the daemon is running**.
- When the daemon is unreachable:
  - `postnoctum status` prints a clear "not running" status and exits with code `4` (degraded).
  - Other client commands exit with code `3` (runtime failure) and print a short instruction to start the daemon (or install the service).

## Privileges

- v1: daemon runs as root (Linux-first).
- v1 does not implement privilege dropping.
- v2: privilege dropping and hardening are planned.

## Directory Layout (Linux defaults)

These defaults can be overridden by global flags (`--config`, `--state-dir`).

- Config:
  - `/etc/postnoctum/config.toml`
- State directory:
  - `/var/lib/postnoctum/`
- SQLite database:
  - `/var/lib/postnoctum/postnoctum.db`
- Runtime/IPC directory:
  - `/run/postnoctum/`
- IPC endpoint:
  - `/run/postnoctum/postnoctum.sock`

macOS/Windows paths will be specified later, but the architecture assumes OS-appropriate defaults.

## Telemetry Sources (v1)

Linux-first sources:

- journald: `journalctl -o json` line-delimited JSON
- kernel ring buffer: `dmesg` (exact collection approach to be specified in the collector)
- `/proc`: process and host signals

Telemetry ingestion is implemented behind interfaces so sources can be swapped later.

Planned v2:

- journald native ingestion via `sd-journal` (`libsystemd` bindings)
- Docker and Kubernetes sources

## Data Flow

1. Collectors ingest local telemetry into normalized events.
2. Rule engine evaluates events and derived signals.
3. Incidents are created/updated based on rule evaluations.
4. Evidence timeline is recorded to SQLite.
5. CLI requests query daemon for:
   - current status
   - incidents list
   - full explanation narrative with evidence timeline

## Observability (of Post Noctum itself)

- CLI output is written to stdout.
- Logs are written to stderr.
- `--format json` returns stable, versioned JSON and does not mix logs with JSON.

## Versioning

- CLI output JSON must be stable and versioned.
- IPC protocol includes version negotiation (see `spec/v1-ipc.md`).
- SQLite schema uses a schema version/migration strategy (see `spec/v1-sqlite-schema.md`).

## Service Installation (Linux-first)

`postnoctum service install`:

- Generates and/or installs a systemd unit file.
- Enables at boot.
- Must not start automatically without explicit confirmation.
- Must not modify firewall rules or open network ports.

(Exact UX and confirmation mechanics are specified in CLI behavior and may evolve.)

## ✅ TODOs

### Phase 1
- 🔧 CLI: human output formatting for status/incidents/explain
- 🧭 CLI: `init` + `service install` workflows
- 🧾 CLI: JSON errors for invalid input (explain, unknown subcommands)
- 🔌 IPC: daemon server + version negotiation

### Phase 2
- 🧠 Daemon: collectors (journald/dmesg/proc) + rule engine
- 🗄️ Storage: SQLite schema + migrations + retention job
- 🧾 Explain: narrative generation + evidence timeline assembly

### v2 Phase 1
- 🔐 Privilege dropping and hardening
- 📦 sd-journal ingestion + Docker/Kubernetes sources
- 🪟 Windows named pipe IPC
