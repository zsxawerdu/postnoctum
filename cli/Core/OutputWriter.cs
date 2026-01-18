namespace PostNoctum.Cli;

internal sealed class OutputWriter(OutputFormat format)
{
    public OutputFormat Format => format;

    public void WritePlain(string text)
    {
        if (format != OutputFormat.Json) Console.Out.WriteLine(text);
    }

    public void WriteMarkdown(string text)
    {
        if (format == OutputFormat.Markdown) Console.Out.WriteLine(text);
    }

    public void WriteJson(object payload)
    {
        if (format == OutputFormat.Json)
            Console.Out.WriteLine(System.Text.Json.JsonSerializer.Serialize(payload));
    }

    public void WriteError(string code, string message)
    {
        if (format == OutputFormat.Json)
            WriteJson(new { ok = false, error = new { code, message } });
        else
            Console.Error.WriteLine(message);
    }
}
