namespace PostNoctum.Cli;

internal interface IIpcClient
{
    ValueTask<IpcResponse<T>> SendAsync<T>(IpcRequest request, CancellationToken ct);
}
