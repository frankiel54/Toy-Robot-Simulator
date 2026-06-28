---
phase: 07-code-cleanup
plan: "02"
subsystem: CommandParser, Application, Tests
tags: [correctness, robustness, idioms, test-quality, coverage]
dependency_graph:
  requires: ["07-01"]
  provides: ["clean-phase-7"]
  affects: ["ToyRobot/CommandParser.cs", "ToyRobot/Application.cs", "ToyRobot.Tests/CommandParserTests.cs", "ToyRobot.Tests/SimulatorTests.cs"]
tech_stack:
  added: []
  patterns: ["C# pattern-matching while loop", "Enum guard for sentinel member rejection"]
key_files:
  created: []
  modified:
    - ToyRobot/CommandParser.cs
    - ToyRobot/Application.cs
    - ToyRobot.Tests/CommandParserTests.cs
    - ToyRobot.Tests/SimulatorTests.cs
decisions:
  - "TryParsePlaceArgs refactored from single chained return to two-step parse-then-guard to allow Unset rejection"
  - "Report_Should_Throw_When_Robot_Not_Placed was already present from Wave 1 (Smell 5); only 3 new tests added"
metrics:
  duration: "~5 minutes"
  completed: "2026-06-28"
  tasks_completed: 4
  files_modified: 4
status: complete
---

# Phase 07 Plan 02: Code Cleanup Wave 2 Summary

Rejected Direction.Unset as a valid facing in TryParsePlaceArgs, trimmed ParseCommand input, replaced while(true) null-break with idiomatic pattern-matching loop, fixed a copy-paste test name, and added three new test cases covering previously uncovered paths.

## Tasks Completed

| Task | Name | Commit | Files |
|------|------|--------|-------|
| 1 | Reject Unset direction in TryParsePlaceArgs | b3282b2 | ToyRobot/CommandParser.cs |
| 2 | Rename Turn_Left_Should_Turn_Right to Turn_Right_Should_Turn_Right | 1a384b4 | ToyRobot.Tests/SimulatorTests.cs |
| 3 | Trim input in ParseCommand; pattern-matching while loop in Run() | 64d55bb | ToyRobot/CommandParser.cs, ToyRobot/Application.cs |
| 4 | Add missing test cases | 031e59a | ToyRobot.Tests/CommandParserTests.cs, ToyRobot.Tests/SimulatorTests.cs |

## Test Results

- Before: 71 tests, 0 failures
- After: 74 tests, 0 failures
- New tests added: 3 (one was already present from Wave 1)

## Deviations from Plan

### Auto-handled: Report_Should_Throw_When_Robot_Not_Placed already present

**Found during:** Task 4
**Issue:** The plan listed four new tests to add including `Report_Should_Throw_When_Robot_Not_Placed`, but Wave 1 (Smell 5) had already added this test to SimulatorTests.cs.
**Fix:** Added only the three genuinely missing tests. All verification criteria (grep counts) still satisfied since the test is present.
**Impact:** None — all acceptance criteria met.

## Verification Results

| Check | Result |
|-------|--------|
| `grep -c "Direction.Unset" ToyRobot/CommandParser.cs` | 1 |
| `grep -c "input.Trim()" ToyRobot/CommandParser.cs` | 1 |
| `grep -c "ReadLine() is { } line" ToyRobot/Application.cs` | 1 |
| `grep -c "while (true)" ToyRobot/Application.cs` | 0 |
| `grep -c "Turn_Right_Should_Turn_Right" ToyRobot.Tests/SimulatorTests.cs` | 1 |
| `grep -c "Turn_Left_Should_Turn_Right" ToyRobot.Tests/SimulatorTests.cs` | 0 |
| All tests pass | 74/74 |

## Self-Check: PASSED

- ToyRobot/CommandParser.cs: modified and committed (b3282b2, 64d55bb)
- ToyRobot/Application.cs: modified and committed (64d55bb)
- ToyRobot.Tests/SimulatorTests.cs: modified and committed (1a384b4, 031e59a)
- ToyRobot.Tests/CommandParserTests.cs: modified and committed (031e59a)
- All 4 commits verified in git log
