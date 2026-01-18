namespace PostNoctum.Cli;

internal interface ICommand
{
    ValueTask<ExitCode> ExecuteAsync(CommandContext ctx);
}
