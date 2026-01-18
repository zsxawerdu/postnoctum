using PostNoctum.Cli;

namespace Cli.Tests;

public sealed class ArgParserTests
{
    [Fact]
    public void ParsesGlobalFlagsAndCommandArgs()
    {
        var result = ArgParser.Parse(["--config", "/etc/postnoctum/config.toml", "--state-dir", "/var/lib/postnoctum", "--log-level", "info", "--format", "json", "status"]);
        Assert.Null(result.Error);
        Assert.Equal("/etc/postnoctum/config.toml", result.Options.ConfigPath);
        Assert.Equal("/var/lib/postnoctum", result.Options.StateDir);
        Assert.Equal("info", result.Options.LogLevel);
        Assert.Equal(OutputFormat.Json, result.Options.Format);
        Assert.Equal(["status"], result.CommandArgs);
    }

    [Fact]
    public void FailsOnUnknownFlag()
    {
        var result = ArgParser.Parse(["--nope"]);
        Assert.NotNull(result.Error);
    }
}
