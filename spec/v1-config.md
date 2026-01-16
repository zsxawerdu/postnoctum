# Post Noctum — v1 Configuration (TOML)

TOML is the canonical configuration format for v1.

## Default Locations (Linux)

- Config: `/etc/postnoctum/config.toml`
- State dir: `/var/lib/postnoctum/`

Both may be overridden with CLI global flags.

## Design Principles

- Explicit, versionable, inspectable.
- No implicit env var merging.
- No silent overrides.

## Proposed Config Shape (v1)

```toml
version = 1

[state]
# Where the SQLite DB and local state live.
dir = "/var/lib/postnoctum"

[ipc]
# Linux/macOS: unix socket path
# Windows: named pipe name (future)
endpoint = "/run/postnoctum/postnoctum.sock"

[retention]
# Retain raw ingested events for explanation timelines.
# Exact behavior is implemented by periodic pruning.
raw_events_days = 14
incident_days = 90

[collectors.journald]
enabled = true
# v1 collection path: journalctl
mode = "journalctl"
# Optional journalctl arguments (kept explicit)
# args = ["--since", "-1h"]

[collectors.dmesg]
enabled = true

[collectors.proc]
enabled = true

[rules]
# Directory of built-in and user rules
# v1 expects mostly built-in rules; user DSL is planned.
dir = "/etc/postnoctum/rules"

[logging]
level = "info"

```

## Notes

- `version` is required and is used to version config parsing.
- `retention` exists in v1 because the system stores full evidence timelines in SQLite.
- Rule/DSL configuration will evolve; v1 prioritizes built-in rules with explicit enable/disable controls.
