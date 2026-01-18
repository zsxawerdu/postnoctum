namespace PostNoctum.Cli;

internal static class ArgParser
{
    public static ParseResult Parse(string[] args)
    {
        var options = GlobalOptions.Default;
        var command = new List<string>();

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                command.AddRange(args[i..]);
                break;
            }

            switch (arg)
            {
                case "--config":
                    options = options with { ConfigPath = NextValue(args, ref i) };
                    break;
                case "--state-dir":
                    options = options with { StateDir = NextValue(args, ref i) };
                    break;
                case "--log-level":
                    options = options with { LogLevel = NextValue(args, ref i) };
                    break;
                case "--format":
                    if (!TryParseFormat(NextValue(args, ref i), out var format))
                        return ParseResult.Fail("invalid --format (plain|json|markdown)");
                    options = options with { Format = format };
                    break;
                default:
                    return ParseResult.Fail($"unknown flag: {arg}");
            }
        }

        return ParseResult.Ok(options, command.ToArray());
    }

    static string NextValue(string[] args, ref int index)
    {
        if (index + 1 >= args.Length) return "";
        index++;
        return args[index];
    }

    static bool TryParseFormat(string value, out OutputFormat format)
    {
        format = value.ToLowerInvariant() switch
        {
            "plain" => OutputFormat.Plain,
            "json" => OutputFormat.Json,
            "markdown" => OutputFormat.Markdown,
            _ => OutputFormat.Plain
        };
        return value is "plain" or "json" or "markdown";
    }
}
