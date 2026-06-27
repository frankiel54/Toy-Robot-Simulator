# Codebase Concerns

**Analysis Date:** 2026-06-27

---

## TODO / FIXME Comments

**`ToyRobot/CommandParser.cs` lines 5 and 13:**
- `// TODO: Error handling` on `ParseCommand`
- `// TODO: Add error handling` on `TryParsePlaceArgs`
- Neither method has any input validation. `ParseCommand` does not crash on empty input (Split on an empty string is safe), but `TryParsePlaceArgs` returns `false` on bad input with no indication of what failed (wrong number of parts, bad integer, unrecognised direction). Callers cannot surface a useful error message to the user.

**`ToyRobot/Program.cs` lines 16–19 and line 23:**
- Lines 16–19: Commented-out block intended to prompt the user when the robot is not yet placed. The feature was planned but never completed. The comment reads `// TODO: Make some initial message that only show once`.
- Line 23: `// TODO: Add some more error messages for when things dont go right`. MOVE, LEFT, RIGHT, and REPORT silently do nothing when the robot is unplaced; no feedback is given to the user.

---

## Empty / Stub File

**`ToyRobot/Commands.cs`:**
- The file path was provided as a source to analyse but the file does not exist on disk. The previous version of this document referenced a `Commands` class; that class appears to have been removed or never created. Its absence means all command dispatch logic lives inline in `Program.cs` with no dedicated home.

---

## Encapsulation Concern — Public Setters on `Robot`

**File:** `ToyRobot/Robot.cs` lines 9–11

```csharp
public int xPos { get; set; }
public int yPos { get; set; }
public Direction direction { get; set; }
```

All three state fields have fully public setters. Any code holding a `Robot` reference can mutate position and direction without going through `Simulator`'s boundary checks. `SimulatorTests.cs` exploits this directly — tests assert state by reading `robot.direction`, `robot.xPos`, and `robot.yPos` (lines 18–21, 32–35, 48–51, 82–84), coupling tests to internal representation rather than observable behaviour.

- Fix approach: Change setters to `internal set` or `private set`. Expose state externally only through `Simulator.Report()` or dedicated read-only properties on `Simulator`.

---

## Naming Convention — camelCase Properties on `Robot`

**File:** `ToyRobot/Robot.cs` lines 9–11

`xPos`, `yPos`, and `direction` use camelCase, violating C# PascalCase property naming conventions. Every other property in the codebase (`RobotPlaced`, `Width`, `Height`) follows the convention correctly. This inconsistency also exists in test assertions (`robot.direction`, `robot.xPos`, `robot.yPos`) throughout `SimulatorTests.cs`.

---

## `Direction.Unset` — Silent Corruption in Rotation Methods

**File:** `ToyRobot/Simulator.cs` lines 33–49

`TurnLeft` and `TurnRight` do not guard against `Robot.direction == Direction.Unset`:

- `TurnLeft` computes `Unset - 1`. `Unset` has enum value `4`, so this yields `3 = West` — silently setting a direction without a valid placement.
- `TurnRight` computes `Unset + 1`, yielding integer `5` — an undefined enum value, which C# allows without throwing.

`Program.cs` guards `LEFT` and `RIGHT` calls with `IsRobotPlaced()` (lines 42–48), so the bug does not surface through normal CLI usage. However, `Simulator` itself has no internal guard, meaning any direct consumer of `Simulator` (future callers, tests) can trigger this without warning.

- Fix approach: Add `if (!RobotPlaced) return;` (or `if (Robot.direction == Direction.Unset) return;`) at the top of both methods.

---

## `Direction` Enum — Arithmetic Coupling in Turn Logic

**File:** `ToyRobot/Simulator.cs` lines 33–49

Turn logic uses integer arithmetic on enum values (`Robot.direction - 1`, `Robot.direction + 1`). This works only because the enum is declared in the exact order `North=0, East=1, South=2, West=3`. Reordering, renaming, or inserting values in `Direction.cs` would silently break rotation without a compile error.

- Fix approach: Use an explicit lookup array or dictionary: `Direction[] clockwise = { North, East, South, West };` and index by position.

---

## `Simulator.Report()` — No Unplaced Guard

**File:** `ToyRobot/Simulator.cs` lines 82–85

`Report()` returns raw robot state regardless of `RobotPlaced`. Calling it before a valid `PLACE` produces the string `"-1, -1, Unset"`. `Program.cs` guards this correctly (line 54), but the method itself makes no contract guarantee. Any future caller must know to check `IsRobotPlaced()` first or it will output nonsense.

- Fix approach: Return `null` or throw `InvalidOperationException` when `!RobotPlaced`, and document the contract.

---

## `TryParsePlaceArgs` Accepts Out-of-Bounds Coordinates

**File:** `ToyRobot/CommandParser.cs` lines 14–22

The parser successfully parses coordinates like `99,12,EAST` (used as a test case in `CommandParserTests.cs` line 43). Range validation is deferred entirely to `Simulator.Place()`. This split is architecturally defensible, but the test asserting `99,12,EAST` parses successfully could mislead a reader into thinking those are valid board positions.

---

## `Program.cs` — No Exit Command

**File:** `ToyRobot/Program.cs` line 14

The main loop is `while (true)` with no break condition. There is no `EXIT`, `QUIT`, or `EOF` handling. The only way to terminate the program is Ctrl+C. An empty input line (user presses Enter) falls to the `default` case and prints "Invalid selection."

---

## `Program.cs` — No Separation of Concerns

**File:** `ToyRobot/Program.cs`

All input reading, parsing, dispatch, and output live in a top-level statement file. The command dispatch switch (lines 24–63) cannot be unit tested without mocking `Console`. Adding batch/file input, a scripted test harness, or a second frontend (e.g., GUI) requires restructuring the entire file.

- Fix approach: Extract a `CommandDispatcher` or `Application` class accepting a `TextReader`, `TextWriter`, and `Simulator`, allowing injection in tests.

---

## `Robot` Passed Into `Simulator` By Reference — Inverted Ownership

**File:** `ToyRobot/Simulator.cs` line 13; `ToyRobot/Program.cs` line 12

`Robot` is constructed externally and passed into `Simulator`, which then mutates it. The external caller retains a live reference and can observe or corrupt `Robot` state independently of `Simulator`. The natural ownership is the reverse: `Simulator` should own `Robot` and expose state through its own API.

- Fix approach: Construct `Robot` internally in `Simulator` and remove the constructor parameter.

---

## Test Coverage Gaps

**`Report()` when robot is unplaced:**
- No test covers `Report()` output before `Place()` is called. The current output is `"-1, -1, Unset"` but this is unspecified, unverified, and arguably wrong.
- Files: `ToyRobot.Tests/SimulatorTests.cs`
- Priority: Medium

**`TurnLeft` / `TurnRight` when robot is unplaced:**
- No test guards or documents the behaviour of rotation before placement. Silent corruption is possible (see Direction.Unset concern above).
- Priority: High

**`MoveForward` when robot is unplaced:**
- `Robot.xPos` and `Robot.yPos` are `-1`, so `Table.IsValidPosition(-1, ...)` returns `false` and the move is silently blocked — but this is incidental behaviour, not a tested contract.
- Priority: Medium

**Multiple `PLACE` calls:**
- No test verifies that a second valid `Place()` overwrites the first correctly, or that an invalid second `Place()` leaves the robot at its previously valid position.
- Priority: Medium

**`IsRobotPlaced()` not asserted after a failed `Place`:**
- `Place_Should_Return_False_And_Not_Set_Robot` checks that robot state is unchanged but does not assert `gameBoard.IsRobotPlaced() == false`.
- Files: `ToyRobot.Tests/SimulatorTests.cs` lines 25–36
- Priority: Low

**`TryParsePlaceArgs` failure path not tested:**
- `CommandParserTests.cs` tests only valid inputs. No test covers malformed args (wrong number of parts, non-integer, invalid direction, empty string).
- Files: `ToyRobot.Tests/CommandParserTests.cs`
- Priority: Medium

**`ParseCommand` test uses space-separated args, not comma-separated:**
- `CommandParserTests.cs` line 23 uses `"PLACE 1 1 NORTH"` as raw input (space-separated after PLACE), but the actual expected format is `"PLACE 1,1,NORTH"` (comma-separated). The test is exercising the split on the first space correctly, but the args token it produces (`"1 1 NORTH"`) would fail `TryParsePlaceArgs`. This is not a test failure but is confusing and misleading.
- Files: `ToyRobot.Tests/CommandParserTests.cs` line 23
- Priority: Low

**No integration / end-to-end tests:**
- No test exercises the full pipeline: raw string input → `ParseCommand` → `TryParsePlaceArgs` → `Simulator` → `Report()` output. All tests are isolated unit tests.
- Priority: Low

---

## Open Domain Questions

**What should happen when `PLACE` is called multiple times?**
- Current implementation overwrites the robot's position. This is likely correct per spec, but it is undocumented and untested. The prior placement is fully discarded, even if the new placement is invalid (which is guarded — invalid re-placement leaves old position intact).

**What should `REPORT` output when the robot is unplaced?**
- The spec typically requires all commands before the first valid `PLACE` to be ignored. `Simulator.Report()` does not enforce this. `Program.cs` does, but only for the CLI path.

**What happens on an empty/whitespace-only input line?**
- `ParseCommand` splits on space and uppercases `parts[0]`. An empty string produces `command = ""`, which hits `default` in the switch and prints "Invalid selection." This is mildly confusing for a user pressing Enter on an empty line.

**Is the board 0-indexed or 1-indexed from the user's perspective?**
- Code uses 0-based indexing. The help text in `Program.cs` shows `PLACE X,Y,Z` but does not clarify valid range. A user may not know the board runs from `0,0` to `4,4`.

---

*Concerns audit: 2026-06-27*
