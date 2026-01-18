namespace PostNoctum.Cli;

internal sealed class RulesCommand(string[] args) : ICommand
{
    public async ValueTask<ExitCode> ExecuteAsync(CommandContext ctx)
    {
        if (!await RequireRunning(ctx)) return ExitCode.RuntimeFailure;
        var sub = args.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(sub)) return ExitCode.UserError;

        if (sub == "list")
        {
            var response = await ctx.Ipc.SendAsync<RulesListResult>(IpcRequests.RulesList(), CancellationToken.None);
            if (!response.Ok)
            {
                ctx.Output.WriteError(response.Error?.Code ?? IpcErrorCodes.Internal, "rules list failed");
                return ExitCode.RuntimeFailure;
            }
            if (ctx.Output.Format == OutputFormat.Json)
            {
                ctx.Output.WriteJson(response.Result ?? new RulesListResult([]));
                return ExitCode.Success;
            }

            ctx.Output.WritePlain($"rules {sub}: not implemented");
            return ExitCode.Success;
        }

        Func<string, IpcRequest>? exec = sub switch
        {
            "enable" => IpcRequests.RulesEnable,
            "disable" => IpcRequests.RulesDisable,
            "test" => IpcRequests.RulesTest,
            _ => null
        };

        if (exec is null)
        {
            ctx.Output.WriteError(IpcErrorCodes.BadRequest, "missing rule action");
            return ExitCode.UserError;
        }
        var result = await WithArg(ctx, exec, args.ElementAtOrDefault(1));
        if (!result.Ok)
        {
            ctx.Output.WriteError(result.Error?.Code ?? IpcErrorCodes.Internal, "rules update failed");
            return ExitCode.RuntimeFailure;
        }

        ctx.Output.WritePlain($"rules {sub}: not implemented");
        return ExitCode.Success;
    }

    static async ValueTask<IpcResponse<IpcEmptyResult>> WithArg(CommandContext ctx, Func<string, IpcRequest> factory, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            ctx.Output.WriteError(IpcErrorCodes.BadRequest, "missing rule id");
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
