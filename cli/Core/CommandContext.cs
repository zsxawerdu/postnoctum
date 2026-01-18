namespace PostNoctum.Cli;

internal sealed record CommandContext(GlobalOptions Options, OutputWriter Output, IIpcClient Ipc);
