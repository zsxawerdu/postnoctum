MODE: Planning, Design

ROLE: Senior Enterprise Architect

OBJECTIVE: Produce a complete feature implementation plan in `specs/<feature>.md` using the defined `PLAN FORMAT`.

CONSTRAINTS:

- Use the `PLAN FORMAT` below verbatim (section names + ordering).
- Follow existing patterns and conventions in the codebase.
- Design for extensibility and maintainability.
- Restrict research + references to files listed in `RELEVANT FILES`; ignore all other paths.
- Ensure Phase 1 (Foundation) explicitly includes repo research steps (start with `README.md`, then survey relevant server/client patterns).
- If a new dependency is required, add it via `uv add` and document it in the `Notes` section.

SUCCESS:

- A new `specs/<feature>.md` plan is produced for the provided Feature.
- Every required section in `PLAN FORMAT` is present and non-empty (placeholders, example <placeholder of content>, replaced with real content).
- `Relevant Files` lists specific existing files (and `New Files` if needed) with brief bullet rationales.
- `Step by Step Tasks` are ordered, actionable, include tests throughout, and end with `Validation Commands`
- `Validation Commands` include `cd app/server && uv run pytest` plus any additional commands needed for end-to-end confidence.

RELEVANT FILES:

- `README.md` - Project overview and instructions.
- `app/server/**` - Server codebase.
- `app/client/**` - Client codebase.
- `scripts/**` - Scripts to start/stop server + client.

PLAN FORMAT:

```md
MODE: Coding, Concise
ROLE: Senior Software Engineer
OBJECTIVE: <describe the feature>
RESULTS: 
# Feature: <feature name>

## Feature Description
<describe the feature in detail, including its purpose and value to users>

## User Story
As a <type of user>
I want to <action/goal>
So that <benefit/value>

## Problem Statement
<clearly define the specific problem or opportunity this feature addresses>

## Solution Statement
<describe the proposed solution approach and how it solves the problem>

## Relevant Files
Use these files to implement the feature:

<find and list the files that are relevant to the feature describe why they are relevant in bullet points. If there are new files that need to be created to implement the feature, list them in an h3 'New Files' section.>

## Implementation Plan
### Phase 1: Foundation
<describe the foundational work needed before implementing the main feature>

### Phase 2: Core Implementation
<describe the main implementation work for the feature>

### Phase 3: Integration
<describe how the feature will integrate with existing functionality>

## Step by Step Tasks
IMPORTANT: Execute every step in order, top to bottom.

<list step by step tasks as h3 headers plus bullet points. use as many h3 headers as needed to implement the feature. Order matters, start with the foundational shared changes required then move on to the specific implementation. Include creating tests throughout the implementation process. Your last step should be running the `Validation Commands` to validate the feature works correctly with zero regressions.>

## Testing Strategy
### Unit Tests
<describe unit tests needed for the feature>

### Integration Tests
<describe integration tests needed for the feature>

### Edge Cases
<list edge cases that need to be tested>

## Acceptance Criteria
<list specific, measurable criteria that must be met for the feature to be considered complete>

## Validation Commands
Execute every command to validate the feature works correctly with zero regressions.

<list commands you'll use to validate with 100% confidence the feature is implemented correctly with zero regressions. every command must execute without errors so be specific about what you want to run to validate the feature works as expected. Include commands to test the feature end-to-end.>
- `cd app/server && uv run pytest` - Run server tests to validate the feature works with zero regressions

## Notes
<optionally list any additional notes, future considerations, or context that are relevant to the feature that will be helpful to the developer>

