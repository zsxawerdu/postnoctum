using PostNoctum.Cli;

namespace Cli.Tests;

public sealed class OutputWriterTests
{
    [Fact]
    public void WritesJsonErrorsToStdout()
    {
        var output = new OutputWriter(OutputFormat.Json);
        using var writer = new StringWriter();
        Console.SetOut(writer);

        output.WriteError("BadRequest", "nope");
        var text = writer.ToString();

        Assert.Contains("\"ok\":false", text);
        Assert.Contains("\"code\":\"BadRequest\"", text);
    }
}
