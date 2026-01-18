namespace PostNoctum.Cli;

internal sealed record IpcResponse<T>(
    int ProtocolVersion,
    string RequestId,
    bool Ok,
    T? Result,
    IpcError? Error,
    string ServerVersion)
{
    public static IpcResponse<T> Success(string requestId, T result) =>
        new(IpcProtocol.ProtocolVersion, requestId, true, result, null, IpcProtocol.ServerVersion);

    public static IpcResponse<T> Fail(string requestId, IpcError error) =>
        new(IpcProtocol.ProtocolVersion, requestId, false, default, error, IpcProtocol.ServerVersion);
}
