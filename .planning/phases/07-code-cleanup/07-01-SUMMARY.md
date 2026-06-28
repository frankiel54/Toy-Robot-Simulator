---
phase: 07-code-cleanup
plan: 01
subsystem: code-quality
tags: [csharp, records, readonly, refactor, encapsulation]

# Dependency graph
requires:
  - phase: 06-robot-encapsulation
    provides: Robot encapsulation with internal setters and InternalsVisibleTo
provides:
  - Unused using directives removed from ParsedCommand.cs and CommandParserTests.cs
  - CommandOptions and ParsedCommand converted to immutable positional records
  - TryParsePlaceArgs made private; its direct test removed
  - Table._width and _height declared readonly
  - IsRobotPlaced() replaced by IsPlaced read-only property
  - Simulator.Report() guarded against unplaced-robot calls
  - Leading \n strings in Application.cs split into separate blank-line calls
affects: [07-02, wave-2-general-fixes]

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "Positional record types for value objects (CommandOptions, ParsedCommand)"
    - "Expression-body read-only property for boolean state: public bool IsPlaced => RobotPlaced"
    - "Guard clause pattern: throw InvalidOperationException at top of Report() when not placed"

key-files:
  created: []
  modified:
    - ToyRobot/ParsedCommand.cs
    - ToyRobot/CommandParser.cs
    - ToyRobot/Table.cs
    - ToyRobot/Simulator.cs
    - ToyRobot/Application.cs
    - ToyRobot.Tests/CommandParserTests.cs

key-decisions:
  - "Converted ParsedCommand and CommandOptions from classes to positional records for init-only immutability"
  - "TryParsePlaceArgs made private — callers outside the parser should use ParseCommand, not the internal helper"
  - "IsRobotPlaced() changed to IsPlaced property — method call with no side effects belongs as a property by C# convention"
  - "Report() guard throws InvalidOperationException — consistent with .NET conventions for calling a method in an invalid state"
  - "Leading-\\n strings split into separate WriteLine() + WriteLine(message) calls for clarity"

patterns-established:
  - "Record types for DTOs and value objects: immutability by default"
  - "Private implementation helpers for internal parsing logic"
  - "Guard clauses as first statement in methods that require preconditions"

requirements-completed: [REQ-07-01]

# Metrics
duration: 10min
completed: 2026-06-28
status: complete
---

# Phase 7 Plan 01: Fix 9 Code Smells Summary

**Converted ParsedCommand and CommandOptions to positional records, made TryParsePlaceArgs private, added readonly to Table fields, replaced IsRobotPlaced() with IsPlaced property, guarded Report() against unplaced robot, and split leading-newline strings in Application.cs**

## Performance

- **Duration:** ~10 min
- **Completed:** 2026-06-28
- **Tasks:** 4 of 4 auto tasks complete (Task 5 is a checkpoint:human-verify — awaiting developer review)
- **Files modified:** 6

## Accomplishments
- All 9 code smells from the post-phase-6 review resolved across 4 atomic commits
- CommandOptions and ParsedCommand converted to positional records — init-only immutability replaces mutable setters
- TryParsePlaceArgs made private; its direct test (which called a now-private API) removed
- Table fields declared readonly — compiler-enforced immutability after construction
- IsRobotPlaced() replaced by IsPlaced property — correct C# convention for side-effect-free boolean state
- Simulator.Report() now throws InvalidOperationException when robot has not been placed
- Both leading-\n strings in Application.cs split into separate blank-line and message calls

## Task Commits

Each task was committed atomically:

1. **Task 1: Remove unused using directives (Smell 1)** - `0daa3f7` (refactor)
2. **Task 2: Convert to records; make TryParsePlaceArgs private (Smells 2, 3, 4, 6)** - `2c4da00` (refactor)
3. **Task 3: readonly Table fields; IsPlaced property; Report() guard (Smells 5, 7, 8)** - `0d34ec3` (refactor)
4. **Task 4: Split leading-newline WriteLine calls (Smell 9)** - `000afc5` (refactor)

## Files Created/Modified
- `ToyRobot/ParsedCommand.cs` — removed 3 unused using directives; converted CommandOptions and ParsedCommand from classes to positional records
- `ToyRobot/CommandParser.cs` — TryParsePlaceArgs changed from public to private; CommandOptions construction updated to positional constructor new CommandOptions(x, y, facing)
- `ToyRobot/Table.cs` — _width and _height fields declared readonly
- `ToyRobot/Simulator.cs` — IsRobotPlaced() replaced by IsPlaced property; Report() has InvalidOperationException guard as first statement
- `ToyRobot/Application.cs` — call site updated to _simulator.IsPlaced; two leading-\n strings split into blank-line + message pairs
- `ToyRobot.Tests/CommandParserTests.cs` — removed 4 unused using directives; removed TryParsePlaceArgs_Should_Parse test and TryParsePlaceArgsData theory data

## Decisions Made
- Converted both types to positional records rather than applying init-only to the existing class form, as positional records are more idiomatic and concise
- Removed TryParsePlaceArgs direct test (3 theory cases) — coverage is fully provided by ParseCommand_Should_Parse_Place_Options
- Report() guard uses InvalidOperationException to match .NET convention for methods called in invalid state (not ArgumentException, which is for bad arguments)

## Deviations from Plan

The plan's stated output was "Changes are NOT committed — they are staged or unstaged in the working tree, ready for your review." The orchestrator instructed each auto task to be committed individually instead. Each of the 4 tasks is committed atomically with a descriptive refactor message. The developer can review via `git log --oneline -4` and `git show <hash>` for each commit. This preserves the review intent while keeping the working tree clean.

## Issues Encountered
None — all 4 tasks executed cleanly. Test count dropped from 73 to 70 (3 cases removed from the deleted TryParsePlaceArgs theory), which is expected.

## Known Stubs
None — no stub patterns introduced in this plan.

## Threat Flags
None — no new network endpoints, auth paths, or trust boundary changes introduced.

## Next Phase Readiness
- Wave 1 (07-01) complete — all 9 code smells resolved and committed
- Wave 2 (07-02) can proceed after the developer reviews these commits and approves the checkpoint
- The developer should run: `dotnet run --project ToyRobot.Tests` to confirm 70 tests pass, then `git log --oneline -4` to review the 4 commits

---
*Phase: 07-code-cleanup*
*Completed: 2026-06-28*
