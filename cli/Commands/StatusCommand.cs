namespace PostNoctum.Cli;

internal sealed class StatusCommand : ICommand
{
    public async ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        var response = await ctx.Ipc.SendAsync<StatusSnapshot>(IpcRequests.StatusGet(), CancellationToken.None);
        if (!response.Ok && response.Error?.Code == IpcErrorCodes.DaemonUnavailable)
        {
            ctx.Output.WriteError(IpcErrorCodes.DaemonUnavailable, "service not running");
            return ExitCode.Degraded;
        }

        if (!response.Ok)
        {
            ctx.Output.WriteError(response.Error?.Code ?? IpcErrorCodes.Internal, "status failed");
            return ExitCode.RuntimeFailure;
        }

        if (ctx.Output.Format == OutputFormat.Json)
        {
            ctx.Output.WriteJson(response.Result ?? new StatusSnapshot(
                new DaemonStatus(0, "", 0, ""),
                new DbStatus("", 0),
                new CollectorStatus(new(true, "", true, null), new(true, true), new(true, true)),
                new RuleStatus(0, 0),
                new IncidentStatus(0, 0),
                new AlertStatus(0)));
            return ExitCode.Success;
        }

        ctx.Output.WritePlain("ok");
        return ExitCode.Success;
    }
}
