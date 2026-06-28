---
phase: 08-domain-model-refactor
plan: "01"
subsystem: domain-model
tags: [refactor, record, immutability, sentinel-removal]
dependency_graph:
  requires: [07-02]
  provides: [08-02]
  affects: [ToyRobot/Direction.cs, ToyRobot/Robot.cs, ToyRobot/Simulator.cs, ToyRobot/CommandParser.cs]
tech_stack:
  added: []
  patterns: [C# record primary constructor, nullable-robot-state, with-expression mutation]
key_files:
  created: []
  modified:
    - ToyRobot/Direction.cs
    - ToyRobot/Robot.cs
    - ToyRobot/Simulator.cs
    - ToyRobot/CommandParser.cs
decisions:
  - Robot converted from mutable class to immutable record with primary constructor (REQ-08-01)
  - Direction.Unset sentinel removed; wildcard switch arms now throw ArgumentOutOfRangeException (REQ-08-02)
  - Simulator uses Robot? _robot (null = unplaced); public X/Y/Facing properties removed (REQ-08-03)
metrics:
  duration: "~5 minutes"
  completed: "2026-06-28"
  tasks_completed: 2
  tasks_total: 2
  files_modified: 4
status: complete
---

# Phase 08 Plan 01: Domain Model Refactor — Direction, Robot, Simulator, CommandParser Summary

**One-liner:** Eliminated Direction.Unset sentinel and mutable Robot class — Robot is now an immutable record, Simulator uses Robot? nullable state with with-expressions, and the X/Y/Facing public API surface is gone.

## Tasks Completed

| # | Task | Commit | Files |
|---|------|--------|-------|
| 1 | Remove Direction.Unset; convert Robot to immutable record | ba3a0da | Direction.cs, Robot.cs |
| 2 | Update Simulator to Robot? with with-expressions; remove sentinel guard from CommandParser | 76dc3c6 | Simulator.cs, CommandParser.cs |

## Changes by File

### ToyRobot/Direction.cs
- Removed `Unset` member — enum now has exactly four members: North, East, South, West
- Replaced `_ => direction` wildcard arm in both `TurnLeft` and `TurnRight` with `_ => throw new ArgumentOutOfRangeException(nameof(direction))`

### ToyRobot/Robot.cs
- Converted from `class` to `record` using primary constructor: `public record Robot(int XPos, int YPos, Direction Direction)`
- Removed parameterless constructor (previously initialised to sentinel state)
- Removed `PlaceAt`, `MoveTo`, `TurnLeft`, `TurnRight` mutation methods
- Replaced `GetNextPosition` wildcard fallback `_ => (XPos, YPos)` with `_ => throw new ArgumentOutOfRangeException(nameof(Direction))`

### ToyRobot/Simulator.cs
- Replaced `private Robot Robot { get; } = new Robot()` + `private bool RobotPlaced { get; set; }` with single `private Robot? _robot;`
- `Place`: `_robot = new Robot(x, y, direction)`
- `TurnLeft`/`TurnRight`: null-guarded with-expression — `_robot = _robot with { Direction = _robot.Direction.TurnLeft() }`
- `MoveForward`: null guard at top; with-expression — `_robot = _robot with { XPos = x, YPos = y }`
- `Report`: null check on `_robot` instead of bool flag; reads `_robot.XPos`, `_robot.YPos`, `_robot.Direction`
- `IsPlaced`: returns `_robot is not null`
- Removed public properties `X`, `Y`, and `Facing` entirely

### ToyRobot/CommandParser.cs
- Removed the four-line sentinel guard block (`if (direction == Direction.Unset) { ... return false; }`) from `TryParsePlaceArgs`
- Method now returns `true` immediately after the compound parse succeeds

## Deviations from Plan

None — plan executed exactly as written.

## Verification

- `dotnet build ToyRobot/ToyRobot.csproj` exits 0 with 0 warnings and 0 errors
- `ToyRobot/Robot.cs` contains `public record Robot`
- `ToyRobot/Simulator.cs` contains `private Robot? _robot` and does not contain `Facing`, `RobotPlaced`, or `Robot Robot`
- `ToyRobot/Direction.cs` does not contain `Unset`

## Known Stubs

None.

## Threat Flags

None — T-08-02 mitigation (removing X/Y/Facing) was applied as planned. No new security surface introduced.

## Self-Check: PASSED

- ToyRobot/Direction.cs — modified, committed ba3a0da
- ToyRobot/Robot.cs — modified, committed ba3a0da
- ToyRobot/Simulator.cs — modified, committed 76dc3c6
- ToyRobot/CommandParser.cs — modified, committed 76dc3c6
- dotnet build exits 0 — verified
