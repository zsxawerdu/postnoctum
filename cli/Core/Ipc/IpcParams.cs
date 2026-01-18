namespace PostNoctum.Cli;

internal sealed record IpcEmptyParams;
internal sealed record IncidentsExplainParams(string IncidentId);
internal sealed record RuleIdParams(string RuleId);
internal sealed record AlertFingerprintParams(string Fingerprint);
