using Tomlyn;
using Tomlyn.Model;

namespace PostNoctum.Cli;

internal static class ConfigLoader
{
    public static ConfigResult Load(string? configPath)
    {
        if (string.IsNullOrWhiteSpace(configPath)) return new(null, null, null);
        if (!File.Exists(configPath)) return new(null, null, "config file not found");
        var text = File.ReadAllText(configPath);
        var model = Toml.ToModel(text) as TomlTable;
        if (model is null) return new(null, null, "invalid config");

        var version = model.TryGetValue("version", out var versionValue) ? versionValue as long? : null;
        var endpoint = ResolveIpcEndpoint(model);
        return new(endpoint, version is null ? null : (int)version, null);
    }

    static string? ResolveIpcEndpoint(TomlTable model)
    {
        if (!model.TryGetValue("ipc", out var ipc)) return null;
        if (ipc is not TomlTable ipcTable) return null;
        if (!ipcTable.TryGetValue("endpoint", out var endpoint)) return null;
        return endpoint as string;
    }
}
