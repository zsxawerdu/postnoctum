namespace PostNoctum.Cli;

internal sealed class ServiceCommand(string[] args) : ICommand
{
    public ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        var sub = args.FirstOrDefault();
        if (sub != "install") return new(ExitCode.UserError);
        ctx.Output.WritePlain("service install: not implemented");
        return new(ExitCode.Success);
    }
}
