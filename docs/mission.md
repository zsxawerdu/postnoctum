# Post Noctum — Philosophy

All telemetry stays local. Only meaning leaves the host.

This document exists to prevent Post Noctum from becoming just another observability product. If a future feature, integration, or refactor violates the principles below, it is a bug, not a debate.

## 1. Post Noctum Is Local-First or It Is Nothing

Post Noctum evaluates systems where they run.

Logs, metrics, configuration, certificates, and runtime state remain on the host at all times. Post Noctum does not centralize telemetry, replicate data, or stream raw signals elsewhere.

If a feature requires shipping raw telemetry off-host, it does not belong in Post Noctum.

## 2. Meaning Is More Valuable Than Data

Modern systems do not fail because they lack data. They fail because humans are forced to interpret data under stress.

Post Noctum prioritizes:

- Explanations over dashboards
- Causal narratives over time series
- Judgement over aggregation

Data exists to support reasoning. Reasoning is the product.

## 3. Outbound Communication Must Be Intentional

The only information that may leave the host is:

- Alert summaries
- Incident fingerprints
- Human-readable conclusions

Outbound data must be:

- Small
- Explicit
- Reviewable by the operator

Any outbound data path must be opt-in, inspectable, and reversible.

## 4. Assume Decay, Drift, and Entropy

Systems do not remain correct. They degrade quietly until they fail loudly.

Post Noctum assumes:

- Configuration drifts
- Certificates expire at inconvenient times
- Rollouts succeed but change behavior
- Defaults mutate across environments

Detecting entropy is a first-class responsibility, not an add-on.

## 5. Optimize for 2 AM, Not 2 PM

Post Noctum is designed for when:

- Context is missing
- People are tired
- Mistakes are expensive

Features that require:

- Extended visual analysis
- Multi-step dashboards
- Manual correlation across tools

are suspect by default.

If it does not help during an on-call incident, it does not ship.

## 6. Determinism Before Intelligence

Post Noctum prefers:

- Explicit rules
- Predictable behavior
- Stable outputs

over:

- Heuristics that cannot be explained
- Non-reproducible results
- “Trust the model” reasoning

When intelligent reasoning is used, it must be:

- Bounded
- Explainable
- Secondary to deterministic signal detection

## 7. The System Must Be Debuggable by Its Users

Post Noctum itself must never become a black box.

Operators must be able to:

- Inspect inputs
- Reproduce decisions
- Understand why an alert fired
- Disable or override behavior

If Post Noctum cannot explain itself, it has failed.

## 8. Boring Is a Feature

Post Noctum intentionally avoids:

- Clever abstractions
- Hidden automation
- Implicit behavior

Configuration must be:

- Explicit
- Versionable
- Readable without documentation

Predictability is more valuable than novelty.

## 9. Trust Is Earned by Restraint

Post Noctum earns trust by:

- Doing less
- Being quiet when nothing is wrong
- Alerting only when confident
- Refusing to speculate beyond evidence

False confidence is more dangerous than uncertainty.

When information is missing, Post Noctum must say so.

## 10. Humans Are the Authority

Post Noctum does not replace operators.

It exists to:

- Reduce cognitive load
- Preserve context
- Surface likely causes
- Suggest next steps

Final judgement always belongs to the human on-call.

## 11. Security Is a Default, Not a Mode

Post Noctum is designed for:

- Regulated environments
- Air-gapped systems
- Zero-trust assumptions

Security is not enabled later. Security is assumed from the first line of code.

## 12. No Feature Without Scar Tissue

Features are justified by:

- Real incidents
- Real failures
- Real operator pain

If a feature cannot be traced back to a failure someone has lived through, it is likely unnecessary.

## Closing Rule

When in doubt, choose:

- Less data
- Fewer alerts
- More explanation
- More operator control

Post Noctum exists to make the night survivable — and the morning understandable.
