namespace PostNoctum.Cli;

internal sealed class AlertsCommand(string[] args) : ICommand
{
    public async ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        if (!await RequireRunning(ctx)) return ExitCode.RuntimeFailure;
        var sub = args.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(sub)) return ExitCode.UserError;

        if (sub == "list")
        {
            var response = await ctx.Ipc.SendAsync<AlertsListResult>(IpcRequests.AlertsList(), CancellationToken.None);
            if (!response.Ok)
            {
                ctx.Output.WriteError(response.Error?.Code ?? IpcErrorCodes.Internal, "alerts list failed");
                return ExitCode.RuntimeFailure;
            }
            if (ctx.Output.Format == OutputFormat.Json)
            {
                ctx.Output.WriteJson(response.Result ?? new AlertsListResult([]));
                return ExitCode.Success;
            }

            ctx.Output.WritePlain($"alerts {sub}: not implemented");
            return ExitCode.Success;
        }

        if (sub == "history")
        {
            var response = await ctx.Ipc.SendAsync<AlertsHistoryResult>(IpcRequests.AlertsHistory(), CancellationToken.None);
            if (!response.Ok)
            {
                ctx.Output.WriteError(response.Error?.Code ?? IpcErrorCodes.Internal, "alerts history failed");
                return ExitCode.RuntimeFailure;
            }
            if (ctx.Output.Format == OutputFormat.Json)
            {
                ctx.Output.WriteJson(response.Result ?? new AlertsHistoryResult([]));
                return ExitCode.Success;
            }

            ctx.Output.WritePlain($"alerts {sub}: not implemented");
            return ExitCode.Success;
        }

        Func<string, IpcRequest>? exec = sub switch
        {
            "suppress" => IpcRequests.AlertsSuppress,
            "unsuppress" => IpcRequests.AlertsUnsuppress,
            _ => null
        };

        if (exec is null)
        {
            ctx.Output.WriteError(IpcErrorCodes.BadRequest, "missing alert action");
            return ExitCode.UserError;
        }
        var result = await WithArg(ctx, exec, args.ElementAtOrDefault(1));
        if (!result.Ok)
        {
            ctx.Output.WriteError(result.Error?.Code ?? IpcErrorCodes.Internal, "alerts update failed");
            return ExitCode.RuntimeFailure;
        }

        if (ctx.Output.Format == OutputFormat.Json)
        {
            ctx.Output.WriteJson(new IpcEmptyResult());
            return ExitCode.Success;
        }

        ctx.Output.WritePlain($"alerts {sub}: not implemented");
        return ExitCode.Success;
    }

    static async ValueTask<IpcResponse<IpcEmptyResult>> WithArg(CommandContext ctx, Func<string, IpcRequest> factory, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ctx.Output.WriteError(IpcErrorCodes.BadRequest, "missing fingerprint");
            return IpcResponse<IpcEmptyResult>.Fail("", new(IpcErrorCodes.BadRequest, "missing id", null));
        }
        return await ctx.Ipc.SendAsync<IpcEmptyResult>(factory(value), CancellationToken.None);
    }

    static async ValueTask<bool> RequireRunning(CommandContext ctx)
    {
        var status = await ctx.Ipc.SendAsync<IpcEmptyResult>(IpcRequests.StatusGet(), CancellationToken.None);
        if (status.Ok) return true;
        ctx.Output.WritePlain("daemon not running; run `postnoctum run` or `postnoctum service install`");
        return false;
    }
}
