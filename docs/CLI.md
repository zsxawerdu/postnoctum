# Post Noctum — CLI

If it cannot be used under pressure, it is broken.

This document defines the command-line interface for Post Noctum.

The CLI is the primary operator interface. Dashboards are intentionally absent.

The CLI must be:

- Predictable
- Discoverable
- Scriptable
- Boring

Surprises are failures.

## 1. Design Principles

- Few commands, many explanations
- Explicit over implicit
- Readable output by default
- Machine-readable output on demand
- No interactive prompts unless explicitly requested

The CLI must work:

- Over SSH
- In minimal terminals
- Without colors
- Without internet access

## 2. Command Structure

All commands follow this pattern:

```sh
postnoctum <command> [subcommand] [flags]
```

Flags must:

- Have long names by default
- Avoid single-letter shortcuts unless universally obvious
- Never change behavior silently

## 3. Core Commands (Non-Negotiable)

### `postnoctum init`

Initialize Post Noctum configuration and local state.

Creates:

- Default config file
- Directory structure
- Example rules
- System service template (not installed)

Does not start the service.

### `postnoctum run`

Run Post Noctum in the foreground.

Intended for:

- Debugging
- Testing
- Development

Must:

- Log verbosely
- Print rule evaluations
- Show reasoning steps

### `postnoctum service install`

Install Post Noctum as a system service.

Responsibilities:

- Register systemd service (or equivalent)
- Enable at boot
- Verify permissions

Must never:

- Start automatically without confirmation
- Modify firewall rules
- Open network ports

### `postnoctum status`

Display current system status.

Shows:

- Service health
- Active incidents
- Suppressed alerts
- Rule evaluation state

Output must be human-readable.

### `postnoctum incidents`

List known incidents.

Default output:

- Incident ID
- Severity
- Affected service/host
- Current state
- First seen / last updated

### `postnoctum explain <incident-id>`

Produce a full local explanation of an incident.

Must include:

- Triggering rules
- Evidence timeline
- Reasoning summary
- Confidence assessment
- Suggested next checks

Default output format: plain text

Optional formats: Markdown, JSON

### `postnoctum rules`

Inspect rule state.

Subcommands:

- `list`
- `enable <rule-id>`
- `disable <rule-id>`
- `test <rule-id>`

Rule changes must be explicit and logged.

### `postnoctum alerts`

Inspect alert state.

Subcommands:

- `list`
- `suppress <fingerprint>`
- `unsuppress <fingerprint>`
- `history`

Alert suppression must never affect reasoning.

## 4. Output Formats

Default output:

- Plain text
- Wrapped for 80 columns
- No color dependencies

Optional:

- `--format json`
- `--format markdown`

JSON output must be:

- Stable
- Versioned
- Backward-compatible

## 5. Exit Codes (Contract)

Exit codes must be consistent.

| Code | Meaning |
| ---: | ------- |
| 0 | Success |
| 1 | User error (invalid input) |
| 2 | Configuration error |
| 3 | Runtime failure |
| 4 | Partial success / degraded |

Scripts must be able to rely on these values.

## 6. Logging Behavior

CLI commands must:

- Log to stderr
- Respect log levels
- Never mix logs with structured output

- Human output → stdout
- Logs → stderr

Always.

## 7. Configuration Flags

Global flags:

- `--config <path>`
- `--state-dir <path>`
- `--log-level`
- `--format`

Flags must never:

- Override config silently
- Persist without explicit user action

## 8. No Hidden Behavior

The CLI must never:

- Auto-enable features
- Auto-update rules
- Auto-enable outbound communication

Every network action must be:

- Configured
- Visible
- Testable

## 9. Help Output Is Part of the Interface

`postnoctum help` must:

- Be concise
- Show examples
- Reference documentation paths

If the help output grows beyond one screen, the interface has failed.

## 10. Example Usage (Canonical)

```sh
postnoctum init
postnoctum service install
postnoctum status

postnoctum incidents
postnoctum explain incident-8f32c

postnoctum rules list
postnoctum alerts suppress restartloop+oom+deploy:api
```

## 11. CLI Backward Compatibility

Breaking CLI changes require:

- Major version bump
- Explicit migration notes
- Transitional compatibility where possible

Operators must never be surprised during an incident.

## Closing Rule

The CLI is not a developer convenience. It is a lifeline.

If a command cannot be executed confidently by a tired human, the interface is wrong.
