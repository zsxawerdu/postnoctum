namespace PostNoctum.Cli;

internal sealed class HelpCommand : ICommand
{
    public ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        ctx.Output.WritePlain("postnoctum <command>");
        return new(ExitCode.UserError);
    }
}
