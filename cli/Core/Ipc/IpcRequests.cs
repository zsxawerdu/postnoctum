namespace PostNoctum.Cli;

internal static class IpcRequests
{
    static string NewId() => Guid.NewGuid().ToString("D");

    public static IpcRequest StatusGet() => New(IpcMethods.StatusGet, new IpcEmptyParams());
    public static IpcRequest IncidentsList() => New(IpcMethods.IncidentsList, new IpcEmptyParams());
    public static IpcRequest IncidentsExplain(string incidentId) => New(IpcMethods.IncidentsExplain, new IncidentsExplainParams(incidentId));
    public static IpcRequest RulesList() => New(IpcMethods.RulesList, new IpcEmptyParams());
    public static IpcRequest RulesEnable(string ruleId) => New(IpcMethods.RulesEnable, new RuleIdParams(ruleId));
    public static IpcRequest RulesDisable(string ruleId) => New(IpcMethods.RulesDisable, new RuleIdParams(ruleId));
    public static IpcRequest RulesTest(string ruleId) => New(IpcMethods.RulesTest, new RuleIdParams(ruleId));
    public static IpcRequest AlertsList() => New(IpcMethods.AlertsList, new IpcEmptyParams());
    public static IpcRequest AlertsSuppress(string fingerprint) => New(IpcMethods.AlertsSuppress, new AlertFingerprintParams(fingerprint));
    public static IpcRequest AlertsUnsuppress(string fingerprint) => New(IpcMethods.AlertsUnsuppress, new AlertFingerprintParams(fingerprint));
    public static IpcRequest AlertsHistory() => New(IpcMethods.AlertsHistory, new IpcEmptyParams());

    static IpcRequest New(string method, object payload) =>
        new(IpcProtocol.ProtocolVersion, NewId(), method, payload, IpcProtocol.ClientVersion);
}
