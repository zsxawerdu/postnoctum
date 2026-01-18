using System.Net.Sockets;
using System.Text;

namespace PostNoctum.Cli;

internal sealed class LocalIpcClient(GlobalOptions options) : IIpcClient
{
    public const string DefaultSocketPath = "/run/postnoctum/postnoctum.sock";

    public async ValueTask<IpcResponse<T>> SendAsync<T>(IpcRequest request, CancellationToken ct)
    {
        try
        {
            var socketPath = ResolveSocketPath(options.StateDir, options.IpcEndpoint);
            using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
            var endpoint = new UnixDomainSocketEndPoint(socketPath);
            await socket.ConnectAsync(endpoint, ct);
            await using var stream = new NetworkStream(socket, ownsSocket: true);
            await using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);

            await writer.WriteLineAsync(SerializeRequest(request));
            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrWhiteSpace(line))
                return IpcResponse<T>.Fail(request.RequestId, new(IpcErrorCodes.Internal, "empty response", null));

            return DeserializeResponse<T>(line);
        }
        catch (SocketException)
        {
            var error = new IpcError(IpcErrorCodes.DaemonUnavailable, "daemon not running", null);
            return IpcResponse<T>.Fail(request.RequestId, error);
        }
        catch (Exception ex)
        {
            var error = new IpcError(IpcErrorCodes.Internal, ex.Message, null);
            return IpcResponse<T>.Fail(request.RequestId, error);
        }
    }

    public static string SerializeRequest(IpcRequest request) =>
        System.Text.Json.JsonSerializer.Serialize(request);

    public static IpcResponse<T> DeserializeResponse<T>(string json) =>
        System.Text.Json.JsonSerializer.Deserialize<IpcResponse<T>>(json) ??
        IpcResponse<T>.Fail("", new(IpcErrorCodes.Internal, "invalid response", null));

    public static string ResolveSocketPath(string? stateDir = null, string? endpoint = null)
    {
        if (!string.IsNullOrWhiteSpace(endpoint))
            return endpoint;
        if (!string.IsNullOrWhiteSpace(stateDir))
            return Path.Combine(stateDir, "postnoctum.sock");
        return DefaultSocketPath;
    }
}
