namespace PostNoctum.Cli;

internal sealed class ExplainCommand(string? incidentId) : ICommand
{
    public async ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        if (string.IsNullOrWhiteSpace(incidentId))
            return ExitCode.UserError;
        if (!await RequireRunning(ctx)) return ExitCode.RuntimeFailure;

        var response = await ctx.Ipc.SendAsync<ExplainResult>(IpcRequests.IncidentsExplain(incidentId), CancellationToken.None);
        if (!response.Ok)
        {
            ctx.Output.WriteError(response.Error?.Code ?? IpcErrorCodes.Internal, "explain failed");
            return ExitCode.RuntimeFailure;
        }

        if (ctx.Output.Format == OutputFormat.Json)
        {
            var fallback = new ExplainResult(
                1,
                new ExplainIncident(incidentId, 0, "open", "", "", "", "", 0),
                new ExplainReasoning("", "", [], []),
                new ExplainEvidence([]),
                new ExplainDebug(new ExplainInputs([])));
            ctx.Output.WriteJson(response.Result ?? fallback);
            return ExitCode.Success;
        }

        ctx.Output.WritePlain($"explain: {incidentId} (not implemented)");
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
