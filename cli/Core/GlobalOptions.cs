namespace PostNoctum.Cli;

internal sealed record GlobalOptions(
    string? ConfigPath,
    string? StateDir,
    string? LogLevel,
    OutputFormat Format,
    string? IpcEndpoint)
{
    public static GlobalOptions Default => new(null, null, null, OutputFormat.Plain, null);
}
