namespace PostNoctum.Cli;

internal enum ExitCode
{
    Success = 0,
    UserError = 1,
    ConfigError = 2,
    RuntimeFailure = 3,
    Degraded = 4
}
