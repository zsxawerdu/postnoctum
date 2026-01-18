namespace PostNoctum.Cli;

internal sealed record IpcError(string Code, string Message, object? Details);
