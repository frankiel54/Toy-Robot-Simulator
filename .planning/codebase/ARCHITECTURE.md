<!-- refreshed: 2026-06-27 -->
# Architecture

**Analysis Date:** 2026-06-27

## System Overview

```text
┌──────────────────────────────────────────────────────────────┐
│                    Console Entry Point                        │
│                  `ToyRobot/Program.cs`                        │
│   stdin loop → CommandParser → switch dispatch               │
└────────────┬─────────────────────────────────────────────────┘
             │ calls
             ▼
┌──────────────────────────────────────────────────────────────┐
│                      Simulator                                │
│                  `ToyRobot/Simulator.cs`                      │
│   Place / MoveForward / TurnLeft / TurnRight / Report        │
└────────┬───────────────────────────┬─────────────────────────┘
         │ mutates                   │ validates via
         ▼                           ▼
┌─────────────────┐        ┌──────────────────────┐
│      Robot      │        │        Table          │
│  `Robot.cs`     │        │    `Table.cs`         │
│  xPos, yPos,    │        │  IsValidPosition(x,y) │
│  direction      │        │  default 5x5          │
└─────────────────┘        └──────────────────────┘
```

## Component Responsibilities

| Component | Responsibility | File |
|-----------|----------------|------|
| `Program` | Entry point — REPL loop, reads stdin, dispatches to Simulator | `ToyRobot/Program.cs` |
| `CommandParser` | Parses raw text into command name + args; parses PLACE args into typed values | `ToyRobot/CommandParser.cs` |
| `Simulator` | Orchestrates all robot operations; enforces placement guard; owns Table instance | `ToyRobot/Simulator.cs` |
| `Robot` | Plain data object holding current position (xPos, yPos) and facing direction | `ToyRobot/Robot.cs` |
| `Table` | Boundary model — validates whether a coordinate pair is within the grid | `ToyRobot/Table.cs` |
| `Direction` | Enum: North, East, South, West, Unset | `ToyRobot/Direction.cs` |

## Pattern Overview

**Overall:** Layered console application with a Facade/Coordinator pattern.

**Key Characteristics:**
- `Simulator` acts as a facade: callers never touch `Robot` or `Table` directly.
- `CommandParser` is a pure-static parsing utility with no side effects.
- `Program.cs` contains the REPL loop and a flat `switch` dispatch — commands are not first-class objects (no Command Pattern).
- `Robot` is a mutable plain-data object (no behaviour); `Simulator` owns all mutation logic.
- `Table` is a pure predicate (no state change), always consulted before mutating `Robot` position.

## Domain Model

**Robot (`ToyRobot/Robot.cs`):**
- Properties: `xPos` (int), `yPos` (int), `direction` (Direction)
- Sentinel initial state: `xPos = -1`, `yPos = -1`, `direction = Direction.Unset`
- Mutated exclusively by `Simulator`

**Table (`ToyRobot/Table.cs`):**
- Represents a configurable grid (default 5x5, via `width`/`height` constructor params)
- Valid positions: `0 <= x < Width`, `0 <= y < Height`
- Instantiated inside `Simulator`; not exposed to callers

**Direction (`ToyRobot/Direction.cs`) — enum:**
- Ordered: `North=0, East=1, South=2, West=3, Unset=4`
- `TurnLeft`/`TurnRight` exploit integer ordering with wrap-around edge cases handled explicitly

**Simulator (`ToyRobot/Simulator.cs`):**
- Owns one `Robot` (injected via constructor) and one `Table` (created internally)
- Tracks `RobotPlaced` boolean; the placement guard is enforced by `Program.cs` (not inside `Simulator` methods)

## Data Flow

### Primary Request Path

1. `Console.ReadLine()` returns raw string (`Program.cs` line 21)
2. `CommandParser.ParseCommand()` splits on first space → uppercased `command` + `args` (`CommandParser.cs` lines 6-11)
3. `switch (command)` in `Program.cs` dispatches to the appropriate `Simulator` method
4. **PLACE:** `CommandParser.TryParsePlaceArgs()` parses `"X,Y,DIRECTION"` → `Simulator.Place()` validates with `Table.IsValidPosition()` then mutates `Robot` (`Simulator.cs` lines 19-31)
5. **MOVE:** `Simulator.MoveForward()` computes candidate position, validates via `Table.IsValidPosition()`, mutates `Robot` only if valid (`Simulator.cs` lines 51-78)
6. **LEFT/RIGHT:** `Simulator.TurnLeft()` / `TurnRight()` adjust `Robot.direction` using enum arithmetic (`Simulator.cs` lines 33-48)
7. **REPORT:** `Simulator.Report()` returns formatted string `"{x}, {y}, {direction}"` written to stdout (`Simulator.cs` line 83)

### Boundary Validation Path

- Every position change (Place or MoveForward) calls `Table.IsValidPosition(x, y)` before committing
- Invalid moves return `false` and leave `Robot` state unchanged
- An invalid PLACE also leaves `RobotPlaced = false`

## Entry Points

**Console REPL:**
- Location: `ToyRobot/Program.cs`
- Triggers: Process start (`dotnet run` or compiled executable)
- Responsibilities: Print usage banner, run infinite stdin loop, parse input, guard unplaced-robot commands, dispatch to `Simulator`

## Architectural Constraints

- **Threading:** Single-threaded synchronous REPL; no concurrency.
- **Global state:** `Simulator` instance is a local variable in top-level statements; `Robot` is injected and shared between `Simulator` and tests via direct property access.
- **Placement guard:** The `IsRobotPlaced()` check is the caller's responsibility (`Program.cs`), not enforced inside `Simulator` methods.
- **Commands.cs:** Does not exist in the repository; there is no Command Pattern or command-object abstraction.
- **Direction wrap-around:** `TurnLeft` and `TurnRight` handle `North<->West` wrap explicitly; intermediate directions use `direction +/- 1` enum arithmetic, which is fragile if enum order changes.

## Anti-Patterns

### Placement guard lives in the caller

**What happens:** `Program.cs` checks `Simulator.IsRobotPlaced()` before calling `MoveForward`, `TurnLeft`, `TurnRight`, and `Report`. `Simulator` methods themselves do not guard against an unplaced robot.
**Why it's wrong:** Any future caller that omits the check will silently operate on a robot at `(-1, -1, Unset)`.
**Do this instead:** Move the guard into each `Simulator` method (throw `InvalidOperationException` or return a failure result when `RobotPlaced == false`).

### Robot properties have public setters

**What happens:** `Robot.xPos`, `Robot.yPos`, and `Robot.direction` have public `set` accessors (`Robot.cs` lines 9-11).
**Why it's wrong:** Any code can bypass `Simulator` and mutate robot state without boundary validation.
**Do this instead:** Use `internal set` or `private set`; expose mutation only through `Simulator`.

### Direction enum arithmetic

**What happens:** `TurnLeft`/`TurnRight` rely on `Direction` enum integer values being contiguous in a specific order (North=0, East=1, South=2, West=3) (`Simulator.cs` lines 33-48).
**Why it's wrong:** Adding or reordering enum members silently breaks rotation logic.
**Do this instead:** Use an explicit lookup table or circular array of directions.

## Error Handling

**Strategy:** Return-value signalling (`bool` returns from `Place` and `MoveForward`); invalid commands print an error message to stdout.

**Patterns:**
- `Place` and `MoveForward` return `false` on failure; no exceptions thrown for domain errors.
- `TryParsePlaceArgs` returns `false` on parse failure.
- No structured error types; error messages are plain strings written to stdout.
- TODO comments in `Program.cs` and `CommandParser.cs` mark known gaps in error messaging and the missing "must PLACE first" prompt.

---

*Architecture analysis: 2026-06-27*
