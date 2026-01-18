using PostNoctum.Cli;

namespace Cli.Tests;

public sealed class ConfigLoaderTests
{
    [Fact]
    public void ReadsIpcEndpointAndVersion()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "version = 1\n[ipc]\nendpoint = \"/run/postnoctum/postnoctum.sock\"\n");
        var result = ConfigLoader.Load(path);
        File.Delete(path);

        Assert.Null(result.Error);
        Assert.Equal(1, result.Version);
        Assert.Equal("/run/postnoctum/postnoctum.sock", result.IpcEndpoint);
    }

    [Fact]
    public void ReturnsErrorWhenMissing()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));
        var result = ConfigLoader.Load(path);
        Assert.NotNull(result.Error);
    }
}
