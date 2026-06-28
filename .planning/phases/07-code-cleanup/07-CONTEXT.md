# Phase 7: Code Cleanup — Context

**Gathered:** 2026-06-28
**Status:** Ready for planning
**Source:** Post-phase-6 code review (two reviews: code smells + general quality)

<domain>
## Phase Boundary

Two sequential waves of code quality fixes. Wave 1 fixes 9 code smells and pauses with a checkpoint so the user can review and commit before Wave 2 begins. Wave 2 fixes 14 general quality issues (correctness bugs, test bugs, robustness, API design, idioms, coverage gaps, naming).

Files in scope:
- `ToyRobot/ParsedCommand.cs`
- `ToyRobot/CommandParser.cs`
- `ToyRobot/Robot.cs`
- `ToyRobot/Table.cs`
- `ToyRobot/Simulator.cs`
- `ToyRobot/Application.cs`
- `ToyRobot/Direction.cs`
- `ToyRobot.Tests/CommandParserTests.cs`
- `ToyRobot.Tests/SimulatorTests.cs`

Out of scope: architecture changes, new features, changes to `ToyRobot.Tests/ApplicationTests.cs`, `RobotTests.cs`, `TableTests.cs`, `DirectionExtensionsTests.cs` (except where a test bug is explicitly listed).

</domain>

<decisions>
## Implementation Decisions

### Wave 1 — Code Smells (07-01)

All 9 fixes are locked. The checkpoint at the end of 07-01 MUST leave changes uncommitted so the user can review the diff before committing.

#### Smell 1: Unused `using` directives
- Remove `using System`, `using System.Collections.Generic`, `using System.Text` from `ParsedCommand.cs`
- Remove `using System`, `using System.Collections.Generic`, `using System.ComponentModel.Design`, `using System.Text` from `CommandParserTests.cs`

#### Smell 2: Mutable `CommandOptions` setters
- Change `CommandOptions.X`, `.Y`, `.Facing` from `{ get; set; }` to `{ get; init; }`
- LOCKED: do NOT convert to record yet — that is Smell 6

#### Smell 3: `TryParsePlaceArgs` visibility
- Change `public static bool TryParsePlaceArgs` to `private static bool TryParsePlaceArgs` in `CommandParser.cs`
- Remove the `TryParsePlaceArgs_Should_Parse` test method and its `TryParsePlaceArgsData` theory data from `CommandParserTests.cs` — this is covered by `ParseCommand_Should_Parse_Place_Options`

#### Smell 4: Bool return ignored (subsumed by Smell 3)
- Removed with the test in Smell 3 — no separate change needed

#### Smell 5: Sentinel magic values
- Add a guard check in `Simulator.Report()`: if `!RobotPlaced` throw `InvalidOperationException("Robot has not been placed.")` — this is minimal; full removal of sentinels is deferred to Wave 2

#### Smell 6: `ParsedCommand` and `CommandOptions` as records
- Convert `ParsedCommand` to: `public record ParsedCommand(CommandType Type, CommandOptions? Options = null);`
- Convert `CommandOptions` to: `public record CommandOptions(int X, int Y, Direction Facing);`
- Remove the now-redundant hand-written constructor from `ParsedCommand`
- Update `CommandParser.ParseCommand` — replace object-initialiser `new CommandOptions { X = x, Y = y, Facing = facing }` with positional constructor `new CommandOptions(x, y, facing)`

#### Smell 7: `Table` fields missing `readonly`
- Change `private int _width` and `private int _height` in `Table.cs` to `private readonly int _width` and `private readonly int _height`

#### Smell 8: `IsRobotPlaced()` should be a property
- Rename `public bool IsRobotPlaced()` to `public bool IsPlaced { get; private set; }` using the existing `RobotPlaced` backing field — or expose via `=> RobotPlaced`
- Update all call sites: `_simulator.IsRobotPlaced()` → `_simulator.IsPlaced` in `Application.cs` and `SimulatorTests.cs`

#### Smell 9: Leading `\n` in `WriteLine` strings
- `Application.cs` line with `"\nInvalid PLACE arguments..."` → split into `_output.WriteLine()` then `_output.WriteLine("Invalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION")`
- `Application.cs` line with `"\nInvalid selection..."` → split into `_output.WriteLine()` then `_output.WriteLine("Invalid selection. Please try again.")`

#### Checkpoint at end of 07-01
- Leave ALL changes from Smell 1–9 uncommitted (staged or unstaged — do not `git commit`)
- Pause with `checkpoint:human-verify` so the user can review the diff and commit

### Wave 2 — General Quality Issues (07-02)

Depends on 07-01 being committed before Wave 2 runs.

#### Correctness bugs
- **`PLACE 1,2,UNSET` accepted as valid**: In `CommandParser.TryParsePlaceArgs`, after `Enum.TryParse<Direction>` succeeds, add: `if (direction == Direction.Unset) { x = 0; y = 0; direction = default; return false; }` — reject Unset as a valid facing
- **`Simulator.Report()` no guard**: Already fixed in Wave 1 (Smell 5)

#### Test bugs
- **`Turn_Left_Should_Turn_Right` copy-paste error**: In `SimulatorTests.cs`, rename the test method to `Turn_Right_Should_Turn_Right` — the body correctly calls `TurnRight()` and uses `TurnRightData`; only the method name is wrong

#### Robustness
- **`CommandParser` doesn't trim input**: Add `input = input.Trim();` at top of `ParseCommand` before splitting

#### C# idioms
- **`while(true)` loop**: Rewrite `Application.Run()` loop from `while (true) { var line = _input.ReadLine(); if (line is null) break; ... }` to `while (_input.ReadLine() is { } line) { ... }`

#### Test coverage gaps (new tests to add in `CommandParserTests.cs` and `SimulatorTests.cs`)
- `PLACE` with `Unset` direction → `ParsedCommand` with null Options
- `CommandParser` with leading whitespace input `" MOVE"` → `CommandType.Move`
- `Simulator.Report()` when robot not placed → throws `InvalidOperationException`
- Re-placing robot at a new valid position → position updates correctly

#### Naming
- Leave `CommandOptions.Facing` vs `Robot.Direction` as-is — the split (Facing = user-facing term, Direction = enum/type name) is intentional and acceptable; document with a comment in `CommandParser.cs` if desired, but no rename

### Claude's Discretion
- Order of fixes within each plan
- Whether to stage or leave unstaged for the checkpoint (either is fine — the key is nothing is committed)
- Test method naming style (keep existing conventions)

</decisions>

<canonical_refs>
## Canonical References

Downstream agents MUST read these before planning or implementing.

### Source files to read before planning
- `ToyRobot/ParsedCommand.cs` — CommandType enum, ParsedCommand class, CommandOptions class
- `ToyRobot/CommandParser.cs` — ParseCommand, TryParsePlaceArgs
- `ToyRobot/Robot.cs` — sentinel values, constructor, properties
- `ToyRobot/Table.cs` — _width, _height fields
- `ToyRobot/Simulator.cs` — IsRobotPlaced(), Report(), RobotPlaced field
- `ToyRobot/Application.cs` — while loop, \n-prefixed WriteLine calls
- `ToyRobot.Tests/CommandParserTests.cs` — TryParsePlaceArgs_Should_Parse test to remove
- `ToyRobot.Tests/SimulatorTests.cs` — Turn_Left_Should_Turn_Right copy-paste error

### Project conventions
- `CLAUDE.md` — run tests with `dotnet run --project ToyRobot.Tests` (NOT `dotnet test`)
- `.planning/codebase/CONVENTIONS.md` — naming and code style

</canonical_refs>

<specifics>
## Specific Implementation Notes

- `CommandOptions` record constructor: `public record CommandOptions(int X, int Y, Direction Facing);` — this replaces the class with mutable setters
- `ParsedCommand` record: `public record ParsedCommand(CommandType Type, CommandOptions? Options = null);` — eliminates the hand-written constructor
- After record conversion, `CommandParser.ParseCommand` line 23–24 must change from object-initialiser to positional: `new CommandOptions(x, y, facing)`
- `IsPlaced` property on Simulator: `public bool IsPlaced => RobotPlaced;` (read-only expression body, backed by existing `private bool RobotPlaced`)
- The `Application.cs` null-pattern while loop: `while (_input.ReadLine() is { } line) { ... }` — `line` is non-nullable inside the block

</specifics>

<deferred>
## Deferred Items

- Full removal of sentinel values from `Robot` constructor (complex — deferred beyond Phase 7)
- Restructuring the dual `Report()` / raw-property API on `Simulator` (intentional tradeoff from Phase 6)
- `TurnLeft`/`TurnRight` return-void documentation (low priority)
- Help text coupling to `CommandParser` (robustness — out of scope for this phase)

</deferred>

---

*Phase: 07-code-cleanup*
*Context gathered: 2026-06-28 from post-phase-6 code review*
