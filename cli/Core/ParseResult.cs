namespace PostNoctum.Cli;

internal sealed record ParseResult(GlobalOptions Options, string[] CommandArgs, string? Error)
{
    public static ParseResult Ok(GlobalOptions options, string[] commandArgs) => new(options, commandArgs, null);
    public static ParseResult Fail(string error) => new(GlobalOptions.Default, [], error);
}
