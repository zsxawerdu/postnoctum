namespace PostNoctum.Cli;

internal sealed class InitCommand : ICommand
{
    public ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        ctx.Output.WritePlain("init: not implemented");
        return new(ExitCode.Success);
    }
}
