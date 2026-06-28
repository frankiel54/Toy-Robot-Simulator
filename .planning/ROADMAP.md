# Roadmap: ToyRobot

## Overview

C# .NET 10 toy robot simulator. Phases 1–6 delivered the core simulation, CLI application, parser, direction extensions, naming cleanup, and Robot encapsulation. Phase 7 addresses code quality issues identified in the post-phase-6 review.

## Phases

- [x] **Phase 1** - Core Robot and simulation
- [x] **Phase 2** - Command parser and application loop
- [x] **Phase 3** - Direction extensions (TurnLeft/TurnRight)
- [x] **Phase 4** - Table boundary checking
- [x] **Phase 5** - Naming consistency fixes
- [x] **Phase 6** - Robot encapsulation (internal setters, InternalsVisibleTo)
- [x] **Phase 7: Code Cleanup** - Fix code smells and general quality issues from post-phase-6 review
- [x] **Phase 8: Domain Model Refactor** - Refactor Robot into an immutable record, remove Direction.Unset sentinel, remove raw X/Y/Facing properties from Simulator in favour of Report()-based verification (completed 2026-06-28)

## Phase Details

### Phase 7: Code Cleanup

**Goal**: Eliminate the 9 code smells and 14 general quality issues identified in the post-phase-6 code review. Delivered in two sequential waves: code smells first (pause for human review before committing), then general fixes.
**Depends on**: Phase 6
**Requirements**: REQ-07-01, REQ-07-02
**Success Criteria** (what must be TRUE):

  1. No unused `using` directives in any source file
  2. `CommandOptions` and `ParsedCommand` are immutable record types
  3. `TryParsePlaceArgs` is private and its direct test removed
  4. `Table` fields are `readonly`, `IsRobotPlaced()` replaced by `IsPlaced` property
  5. Sentinel magic values removed or properly guarded
  6. All correctness bugs fixed (UNSET direction accepted, Report without guard)
  7. All tests pass with no copy-paste errors

**Plans**: 2 plans

Plans:

- [x] 07-01: Fix 9 code smells (Wave 1 — checkpoint before commit)
- [x] 07-02: Fix 14 general quality issues (Wave 2 — after 07-01 committed)

### Phase 8: Domain Model Refactor

**Goal**: Refactor Robot into an immutable record type, eliminate the Direction.Unset sentinel value, and remove the raw X/Y/Facing property API from Simulator — tests verify behaviour exclusively via Report().
**Depends on**: Phase 7
**Requirements**: REQ-08-01, REQ-08-02, REQ-08-03
**Success Criteria** (what must be TRUE):

  1. `Robot` is a C# record with value semantics and no setters
  2. `Direction.Unset` is removed from the enum; `TryParsePlaceArgs` no longer guards against it
  3. Default `Robot()` constructor removed; `Simulator` uses `Robot?` (null = unplaced)
  4. `Simulator.X`, `Simulator.Y`, and `Simulator.Facing` properties removed
  5. All tests that previously used X/Y/Facing are rewritten to assert via `Report()`
  6. All 74 tests pass (adjusted count if sentinel test cases removed)

**Plans**: 2/2 plans complete

Plans:
**Wave 1**

- [x] 08-01-PLAN.md — Refactor Direction enum and Robot to record; update Simulator and CommandParser (Wave 1)

**Wave 2** *(blocked on Wave 1 completion)*

- [x] 08-02-PLAN.md — Rewrite DirectionExtensionsTests, RobotTests, SimulatorTests for new API (Wave 2)
