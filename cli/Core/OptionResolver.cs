namespace PostNoctum.Cli;

internal static class OptionResolver
{
    public static (GlobalOptions Options, string? Error) Resolve(GlobalOptions options)
    {
        var config = ConfigLoader.Load(options.ConfigPath);
        if (!string.IsNullOrWhiteSpace(config.Error)) return (options, config.Error);
        if (config.Version is not null and not 1) return (options, "config version unsupported");
        var resolved = string.IsNullOrWhiteSpace(config.IpcEndpoint)
            ? options
            : options with { IpcEndpoint = config.IpcEndpoint };
        return (resolved, null);
    }
}
