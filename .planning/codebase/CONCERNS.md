# Codebase Concerns

**Analysis Date:** 2026-06-27

---

## TODO / FIXME Comments

**`ToyRobot/CommandParser.cs` lines 5 and 13:**
- Two `// TODO: Error handling` / `// TODO: Add error handling` comments are present on both public methods.
- Neither `ParseCommand` nor `TryParsePlaceArgs` guard against a null or empty `input` string. Passing `null` directly (rather than `string.Empty`) to `ParseCommand` would throw a `NullReferenceException` on the `.Split` call.
- `TryParsePlaceArgs` returns `false` on bad input but gives no indication of _why_ parsing failed (wrong number of parts, bad integer, unrecognised direction), which makes it impossible to surface a useful error message to the user.

**`ToyRobot/Program.cs` lines 16–19:**
- Commented-out block for showing a first-run prompt to the user (`// TODO: Make some initial message that only shows once`) was never completed. No guidance is given when the robot has not yet been placed; commands other than PLACE are silently ignored.

**`ToyRobot/Program.cs` line 24:**
- `// TODO: Add some more error messages for when things don't go right` — silent failure on `MOVE`, `LEFT`, `RIGHT`, and `REPORT` when the robot is not placed. The user receives no feedback.

---

## Incomplete / Empty Implementation

**`ToyRobot/Commands.cs`:**
- The `Commands` class (lines 8–9) is entirely empty. It appears to have been scaffolded as the destination for command logic that was never moved out of `Program.cs`. Its presence adds confusion without contributing behaviour.
- Files: `ToyRobot/Commands.cs`
- Fix approach: Either populate the class with command handler methods and refactor `Program.cs` to delegate to it, or delete the file.

---

## Boundary / Off-by-One Bugs

**`ToyRobot/GameBoard.cs` — `Place` method, line 23:**
```csharp
if (x > XBoundary || y > YBoundary) {
```
- The check does not guard against negative coordinates. `Place(-1, 2, Direction.North)` succeeds and puts the robot at an invalid position. The `Robot` constructor initialises `xPos` and `yPos` to `-1` specifically to signal "unplaced", so a placed robot at `(-1, -1)` is indistinguishable from the sentinel state.
- Fix approach: Change condition to `x < 0 || y < 0 || x > XBoundary || y > YBoundary`.

**`ToyRobot/GameBoard.cs` — board size semantics:**
- `GameBoard` is constructed with `(4, 4, robot)` in `Program.cs` (line 4) and in every test. The boundary value `4` is used as an inclusive maximum (`x > XBoundary`), so the valid range is `0–4`, giving a 5×5 grid. The problem specification typically describes a 5×5 board (0–4), so this is technically correct, but the constructor parameter name `x` / `y` is misleading — it reads as "width/height" (i.e., 4) rather than "max index" (i.e., 4 = index of the 5th cell). A caller passing `5` expecting a 5×5 board would get a 6×6 board instead.
- Fix approach: Rename parameters to `xMax`/`yMax` and document the semantics, or subtract 1 internally (`XBoundary = x - 1`).

---

## Design Concerns

**Arithmetic turn logic in `GameBoard.cs` lines 36–51:**
- `TurnLeft` and `TurnRight` rely on the numeric order of the `Direction` enum values (`North=0, East=1, South=2, West=3, Unset=4`). Adding, reordering, or renaming enum members would silently break rotation. The `Unset` value occupying index 4 sits just past `West`, so `TurnRight` from `West` would overflow to `Unset` if the `West` special-case guard were accidentally removed.
- Fix approach: Use an explicit lookup (a small array or dictionary) for direction cycling rather than integer arithmetic.

**`Direction.Unset` in the rotation methods:**
- `TurnLeft` and `TurnRight` do not guard against being called when `Robot.direction == Direction.Unset`. `TurnLeft` would compute `Unset - 1 = West` (integer underflow of the enum), which is an incorrect but non-crashing silent mutation. `TurnRight` would set direction to `Unset + 1`, which maps to integer `5` — an undefined enum value.
- Files: `ToyRobot/GameBoard.cs` lines 35–51
- Fix approach: Guard both methods with `if (Robot.direction == Direction.Unset) return;` or enforce that turns are only reachable after placement (already partially done in `Program.cs` via `IsRobotPlaced`, but the `GameBoard` methods themselves have no internal guard).

**`Robot` properties are fully public setters (`ToyRobot/Robot.cs` lines 9–11):**
- `xPos`, `yPos`, and `direction` are all `public set`. Any caller can mutate robot state without going through `GameBoard`'s boundary validation. This breaks encapsulation and makes it trivial to corrupt state.
- Fix approach: Change setters to `internal set` or `private set` and expose mutation only via `GameBoard` methods.

**`GameBoard` holds a `Robot` reference but exposes no `Robot` property:**
- `GameBoard` controls the robot through a private field, which is correct. However, tests in `GameBoardTests.cs` reach into the `Robot` instance directly (e.g., `Assert.Equal(Direction.North, robot.direction)`) rather than going through `GameBoard.Report()`. This couples tests to internal representation.
- Files: `ToyRobot.Tests/GameBoardTests.cs` lines 18–21, 48–51, etc.

**`Program.cs` is a top-level statement file with no separation of concerns:**
- All input reading, parsing, dispatch, and output live in `Program.cs`. This makes unit testing the CLI loop impossible without mocking `Console`, and makes it difficult to add features (e.g., batch/file input, a test harness) without restructuring the file.
- Fix approach: Extract a `GameLoop` or `Application` class that accepts an `ICommandParser`, `IGameBoard`, and `TextReader`/`TextWriter`, allowing injection in tests.

---

## Test Coverage Gaps

**`MOVE` called before `PLACE` is never tested:**
- `GameBoard.MoveForward()` can be called with `RobotPlaced == false`, at which point `Robot.xPos` and `Robot.yPos` are `-1`. The move boundary check (`x < 0`) would catch this and return `false`, but the behaviour is untested.
- Files: `ToyRobot.Tests/GameBoardTests.cs`
- Priority: Medium

**`TurnLeft` / `TurnRight` called before `PLACE` are never tested:**
- As noted above, calling these when `direction == Direction.Unset` can produce silent corruption.
- Priority: High

**`Place` with negative coordinates is not tested:**
- The off-by-one gap in the boundary check (missing `x < 0 || y < 0`) is not covered by any test.
- Files: `ToyRobot.Tests/GameBoardTests.cs`
- Priority: High

**`CommandParser` failure cases are not tested:**
- `TryParsePlaceArgs` is only tested with valid inputs. No test covers malformed strings (missing comma, non-integer, unknown direction, empty string).
- Files: `ToyRobot.Tests/CommandParserTests.cs`
- Priority: Medium

**`ParseCommand` with lower-case input is not tested:**
- The method calls `.ToUpper()` on the command token. Tests only supply upper-case input; the normalisation is untested.
- Priority: Low

**`PLACE` called multiple times is not tested:**
- Re-placing the robot at a new valid position should update state. Re-placing at an out-of-bounds position should leave state unchanged. Neither scenario has a test.
- Priority: Medium

**`Report` called before `PLACE` is not tested:**
- `Program.cs` guards this with `IsRobotPlaced()`, but `GameBoard.Report()` itself would return `"-1, -1, Unset"` if called directly without placement. No test documents this expectation.
- Priority: Low

**Integration / end-to-end tests are absent:**
- There are no tests that exercise the full command parsing → game board → output pipeline. All tests operate at the unit level on individual methods.
- Priority: Low (acceptable for scope, but worth noting for future extension)

---

## Open Domain Questions

**What should happen when `PLACE` is called multiple times?**
- The current implementation simply overwrites the robot's position. This is likely correct per spec, but it is undocumented and untested.

**Should `REPORT` output go to `Console.Out` or be returned as a value?**
- `GameBoard.Report()` returns a `string`, and `Program.cs` calls `Console.WriteLine(report)`. This is clean, but if multiple output streams were needed (e.g., test capture), there is no abstraction.

**What happens on an empty/whitespace-only input line?**
- `ParseCommand` splits on space and takes `parts[0].ToUpper()`. An empty string produces `command = ""`, which hits the `default` case in the switch and prints "Invalid selection." — mildly confusing for the user hitting Enter on an empty line.

**Is the board 0-indexed or 1-indexed in the problem domain?**
- The code uses 0-based indexing consistently, but the user-facing help text in `Program.cs` (`PLACE X,Y,Z`) does not clarify the valid range. A user may not know the board goes from `0,0` to `4,4`.

---

*Concerns audit: 2026-06-27*
