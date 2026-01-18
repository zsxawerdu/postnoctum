namespace PostNoctum.Cli;

internal sealed class IncidentsCommand : ICommand
{
    public async ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        var status = await RequireRunning(ctx);
        if (!status) return ExitCode.RuntimeFailure;

        var response = await ctx.Ipc.SendAsync<IncidentsListResult>(IpcRequests.IncidentsList(), CancellationToken.None);
        if (!response.Ok)
        {
            ctx.Output.WriteError(response.Error?.Code ?? IpcErrorCodes.Internal, "incidents failed");
            return ExitCode.RuntimeFailure;
        }

        if (ctx.Output.Format == OutputFormat.Json)
        {
            ctx.Output.WriteJson(response.Result ?? new IncidentsListResult([]));
            return ExitCode.Success;
        }

        ctx.Output.WritePlain("incidents: not implemented");
        return ExitCode.Success;
    }

    static async ValueTask<bool> RequireRunning(CommandContext ctx)
    {
        var status = await ctx.Ipc.SendAsync<IpcEmptyResult>(IpcRequests.StatusGet(), CancellationToken.None);
        if (status.Ok) return true;
        ctx.Output.WritePlain("daemon not running; run `postnoctum run` or `postnoctum service install`");
        return false;
    }
}
