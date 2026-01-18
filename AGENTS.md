# AGENTS

## TL;DR
- 🧱 Post Noctum CLI v1 scaffold: args -> options -> command routing + IPC stubs in `cli/`
- 🧪 Tests: xUnit in `cli.tests/` (references `cli/` via `InternalsVisibleTo`)
- 🎯 Target framework: `net10.0` (no `.sln`; always pass explicit `.csproj`)
- 🔌 IPC: NDJSON over Unix domain socket; path from config `[ipc].endpoint` or `--state-dir`

## Repo Shape
- `cli/` (console app)
  - `Program.cs` -> `EntryPoint.RunAsync`
  - `EntryPoint.cs`: `ArgParser` -> `OptionResolver` -> `OutputWriter` -> `LocalIpcClient` -> `CommandRouter` -> `ICommand`
  - `Core/`: options, parsing, output, config, exit codes, IPC protocol types
  - `Commands/`: each command implements `ICommand`
- `cli.tests/` (xUnit)
- Docs/specs:
  - CLI contract: `docs/CLI.md`
  - Protocol/config specs: `spec/*.md`

## Build / Lint / Test

### Prereqs
- `dotnet --info` should show SDK 10.x.
- No `global.json` pin.

### Restore
- `dotnet restore ./cli/cli.csproj`
- `dotnet restore ./cli.tests/cli.tests.csproj`

### Build
- Dev: `dotnet build ./cli/cli.csproj -c Debug`
- Release: `dotnet build ./cli/cli.csproj -c Release`

### Run CLI
- `dotnet run --project ./cli/cli.csproj -- status`
- Pass global flags (they must come before the command token):
  - `dotnet run --project ./cli/cli.csproj -- --config ./config.toml --state-dir /tmp/postnoctum --format json status`

### Test (xUnit)
- Run all tests:
  - `dotnet test ./cli.tests/cli.tests.csproj`
- List tests:
  - `dotnet test ./cli.tests/cli.tests.csproj --list-tests`
- Run a single test (exact fully-qualified name):
  - `dotnet test ./cli.tests/cli.tests.csproj --filter FullyQualifiedName=Cli.Tests.ArgParserTests.ParsesGlobalFlagsAndCommandArgs`
- Run a subset (contains match):
  - `dotnet test ./cli.tests/cli.tests.csproj --filter FullyQualifiedName~ArgParserTests`
- Tight loop (avoid rebuild):
  - `dotnet test ./cli.tests/cli.tests.csproj --filter FullyQualifiedName~ArgParserTests --no-build`

### Coverage
- Coverlet collector is referenced:
  - `dotnet test ./cli.tests/cli.tests.csproj --collect:"XPlat Code Coverage"`

### Formatting (“lint”)
- No committed `.editorconfig` / StyleCop / analyzers.
- Use `dotnet format`.
- Important: repo root has no `.sln`, so always pass a `.csproj`:
  - Verify: `dotnet format ./cli/cli.csproj --verify-no-changes`
  - Verify: `dotnet format ./cli.tests/cli.tests.csproj --verify-no-changes`
  - Apply: `dotnet format ./cli/cli.csproj`
  - Apply: `dotnet format ./cli.tests/cli.tests.csproj`

## CLI Contract Rules (Don’t Break)
- Output formats:
  - default: human text (stdout)
  - `--format json`: JSON only (stdout)
  - `--format markdown`: markdown only (stdout)
- Errors:
  - when JSON: emit `{ ok: false, error: { code, message } }` to stdout
  - otherwise: human error to stderr
- Never mix human output with JSON output.
- Exit codes are a stable contract; keep `ExitCode` values stable.

## C# Code Style

### Language / Project Settings
- `ImplicitUsings=enable`, `Nullable=enable`, `TargetFramework=net10.0`.
- Default visibility: `internal` (public only when truly part of the CLI API surface).
- Prefer file-scoped namespaces (`namespace PostNoctum.Cli;`).

### Imports
- Prefer implicit usings; add explicit `using` only when needed.
- If you add usings, keep order:
  - `System.*`
  - third-party (`Tomlyn`)
  - project (`PostNoctum.*`)

### Formatting / Patterns
- Keep code compact (this repo leans expression-first).
- Prefer:
  - switch expressions for routing/dispatch (`CommandRouter.Route`)
  - primary constructors for simple command wrappers (`RulesCommand(string[] args)`)
  - `record` types for DTOs/results
- Use `var` for locals when obvious; keep explicit types for clarity at boundaries.

### Naming
- Types/methods/properties: `PascalCase`.
- Locals/params: `camelCase`.
- Consts: `PascalCase` (e.g. `DefaultSocketPath`).
- Tests: sentence-style exists; match the file’s style.

### Types / Nullability
- Be intentional with `T?`; avoid “null means many things” APIs.
- Prefer returning explicit result objects / `(value, error)` tuples over throwing.

### Async
- Command handlers return `ValueTask<ExitCode>`; keep that pattern.
- Avoid `async` if you can return a completed `ValueTask`.

### Error Handling
- Expected failures: write an error + return an `ExitCode`.
  - `ctx.Output.WriteError(IpcErrorCodes.BadRequest, "...")`
  - `return ExitCode.UserError;`
- Exceptions are only caught at boundaries:
  - `LocalIpcClient` maps `SocketException` -> `DaemonUnavailable`
  - unexpected exceptions -> `Internal` (message only)
- Keep error messages short and operator-oriented.

### Output
- Commands should route all printing through `OutputWriter`.
- If `ctx.Output.Format == OutputFormat.Json`, emit a structured payload (or `IpcEmptyResult`).
- Placeholder human output pattern: `"<cmd> <sub>: not implemented"`.

### IPC / Protocol
- Transport: Unix domain socket (`AddressFamily.Unix`).
- Encoding: NDJSON (one JSON object per line).
- Serialization: `System.Text.Json`.
- Socket path resolution (priority):
  - config `[ipc].endpoint` (via `OptionResolver`)
  - `--state-dir` + `postnoctum.sock`
  - default `/run/postnoctum/postnoctum.sock`
- IPC DTO shapes are part of a contract; don’t rename fields casually.

## Adding/Changing Commands
- Add new command: implement `ICommand` in `cli/Commands/`.
- Route it in `cli/Commands/CommandRouter.cs`.
- Missing/invalid args: `WriteError(...BadRequest...)` + `ExitCode.UserError`.
- Add at least one xUnit test in `cli.tests/` for routing/behavior.
- If you change CLI behavior/output/exit codes, update `docs/CLI.md`.

## Cursor / Copilot Rules
- No `.cursor/rules/`, `.cursorrules`, or `.github/copilot-instructions.md` in this repo.
