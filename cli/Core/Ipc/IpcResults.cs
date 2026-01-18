namespace PostNoctum.Cli;

internal sealed record IpcEmptyResult;

internal sealed record StatusSnapshot(
    DaemonStatus Daemon,
    DbStatus Db,
    CollectorStatus Collectors,
    RuleStatus Rules,
    IncidentStatus Incidents,
    AlertStatus Alerts);

internal sealed record DaemonStatus(int Pid, string StartedUtc, long UptimeSeconds, string Version);
internal sealed record DbStatus(string Path, int SchemaVersion);
internal sealed record CollectorStatus(CollectorJournald Journald, CollectorDmesg Dmesg, CollectorProc Proc);
internal sealed record CollectorJournald(bool Enabled, string Mode, bool Healthy, string? LastEventUtc);
internal sealed record CollectorDmesg(bool Enabled, bool Healthy);
internal sealed record CollectorProc(bool Enabled, bool Healthy);
internal sealed record RuleStatus(int Enabled, int Disabled);
internal sealed record IncidentStatus(int Open, int Resolved);
internal sealed record AlertStatus(int Suppressed);

internal sealed record IncidentSummary(
    string IncidentId,
    int Severity,
    string State,
    string Title,
    string Fingerprint,
    string FirstSeenUtc,
    string LastUpdatedUtc,
    double Confidence,
    string Summary);

internal sealed record IncidentsListResult(IReadOnlyList<IncidentSummary> Incidents);

internal sealed record RuleSummary(string RuleId, string Title, string Description, bool Enabled, string UpdatedUtc);
internal sealed record RulesListResult(IReadOnlyList<RuleSummary> Rules);

internal sealed record AlertSuppression(string Fingerprint, string Reason, string CreatedUtc, string? ExpiresUtc);
internal sealed record AlertsListResult(IReadOnlyList<AlertSuppression> Suppressions);

internal sealed record AlertsHistoryResult(IReadOnlyList<AlertSuppression> History);

internal sealed record ExplainResult(
    int ExplainVersion,
    ExplainIncident Incident,
    ExplainReasoning Reasoning,
    ExplainEvidence Evidence,
    ExplainDebug Debug);

internal sealed record ExplainIncident(
    string IncidentId,
    int Severity,
    string State,
    string Title,
    string Fingerprint,
    string FirstSeenUtc,
    string LastUpdatedUtc,
    double Confidence);

internal sealed record ExplainReasoning(
    string Summary,
    string ConfidenceAssessment,
    IReadOnlyList<ExplainRuleTrigger> TriggeringRules,
    IReadOnlyList<string> SuggestedNextChecks);

internal sealed record ExplainRuleTrigger(string RuleId, string Title, bool Matched);

internal sealed record ExplainEvidence(IReadOnlyList<EvidenceEvent> Timeline);

internal sealed record EvidenceEvent(
    string EventId,
    string ObservedUtc,
    string Source,
    string EventType,
    object Payload);

internal sealed record ExplainDebug(ExplainInputs Inputs);
internal sealed record ExplainInputs(IReadOnlyList<CollectorInput> Collectors);
internal sealed record CollectorInput(string Source, bool Available, string? Notes);
