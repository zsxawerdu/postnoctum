namespace PostNoctum.Cli;

internal sealed record IpcRequest(
    int ProtocolVersion,
    string RequestId,
    string Method,
    object Params,
    string ClientVersion);
