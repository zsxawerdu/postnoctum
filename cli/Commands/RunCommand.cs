namespace PostNoctum.Cli;

internal sealed class RunCommand : ICommand
{
    public ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        ctx.Output.WritePlain("run: not implemented");
        return new(ExitCode.Success);
    }
}
