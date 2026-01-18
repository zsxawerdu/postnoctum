namespace PostNoctum.Cli;

internal static class Program
{
    static async Task<int> Main(string[] args) => (int)await EntryPoint.RunAsync(args);
}
