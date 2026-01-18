namespace PostNoctum.Cli;

internal static class CommandRouter
{
    public static ICommand Route(string[] args) => args.Length == 0 ? new HelpCommand() : args[0] switch
    {
        "init" => new InitCommand(),
        "run" => new RunCommand(),
        "status" => new StatusCommand(),
        "incidents" => new IncidentsCommand(),
        "explain" => new ExplainCommand(args.Skip(1).FirstOrDefault()),
        "rules" => new RulesCommand(args.Skip(1).ToArray()),
        "alerts" => new AlertsCommand(args.Skip(1).ToArray()),
        "service" => new ServiceCommand(args.Skip(1).ToArray()),
        _ => new HelpCommand()
    };
}
