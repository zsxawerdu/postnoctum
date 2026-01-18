namespace PostNoctum.Cli;

internal static class EntryPoint
{
    public static async ValueTask<ExitCode> RunAsync(string[] args)
    {
        var parse = ArgParser.Parse(args);
        if (parse.Error is { } error)
        {
            Console.Error.WriteLine(error);
            return ExitCode.UserError;
        }

        var output = new OutputWriter(parse.Options.Format);
        var resolved = OptionResolver.Resolve(parse.Options);
        if (!string.IsNullOrWhiteSpace(resolved.Error))
        {
            output.WriteError(IpcErrorCodes.ConfigError, resolved.Error);
            return ExitCode.ConfigError;
        }
        var ipc = new LocalIpcClient(resolved.Options);
        var ctx = new CommandContext(resolved.Options, output, ipc);

        var command = CommandRouter.Route(parse.CommandArgs);
        return await command.ExecuteAsync(ctx);
    }
}
