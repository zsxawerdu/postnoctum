namespace PostNoctum.Cli;

internal sealed record ConfigResult(string? IpcEndpoint, int? Version, string? Error);
