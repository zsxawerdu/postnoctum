#

- CLI: .NET 10 console app in `cli/`
- Tests: xUnit in `cli.tests/` (run: `dotnet test` in `cli.tests/`)
- IPC: NDJSON over UDS; socket path resolves from `[ipc].endpoint` in config or `--state-dir`
- Config: TOML via Tomlyn; requires `version = 1`
- Specs: TODO lists tracked in `spec/*.md`
