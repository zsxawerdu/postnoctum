using PostNoctum.Cli;

namespace Cli.Tests;

public sealed class OptionResolverTests
{
    [Fact]
    public void AppliesConfigEndpoint()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "version = 1\n[ipc]\nendpoint = \"/run/postnoctum/postnoctum.sock\"\n");
        var options = new GlobalOptions(path, null, null, OutputFormat.Plain, null);
        var resolved = OptionResolver.Resolve(options);
        File.Delete(path);

        Assert.Null(resolved.Error);
        Assert.Equal("/run/postnoctum/postnoctum.sock", resolved.Options.IpcEndpoint);
    }

    [Fact]
    public void RejectsUnsupportedConfigVersion()
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "version = 2\n");
        var options = new GlobalOptions(path, null, null, OutputFormat.Plain, null);
        var resolved = OptionResolver.Resolve(options);
        File.Delete(path);

        Assert.Equal("config version unsupported", resolved.Error);
    }
}
