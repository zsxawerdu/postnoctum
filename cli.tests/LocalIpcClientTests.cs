using PostNoctum.Cli;

namespace Cli.Tests;

public sealed class LocalIpcClientTests
{
    [Fact]
    public void ResolvesSocketPathFromEndpoint()
    {
        var path = LocalIpcClient.ResolveSocketPath(null, "/tmp/postnoctum.sock");
        Assert.Equal("/tmp/postnoctum.sock", path);
    }

    [Fact]
    public void ResolvesSocketPathFromStateDir()
    {
        var path = LocalIpcClient.ResolveSocketPath("/var/lib/postnoctum", null);
        Assert.Equal(Path.Combine("/var/lib/postnoctum", "postnoctum.sock"), path);
    }
}
