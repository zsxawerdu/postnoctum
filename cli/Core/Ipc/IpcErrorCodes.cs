namespace PostNoctum.Cli;

internal static class IpcErrorCodes
{
    public const string DaemonUnavailable = "DaemonUnavailable";
    public const string BadRequest = "BadRequest";
    public const string NotFound = "NotFound";
    public const string Conflict = "Conflict";
    public const string ConfigError = "ConfigError";
    public const string Internal = "Internal";
    public const string UnsupportedProtocol = "UnsupportedProtocol";
}
