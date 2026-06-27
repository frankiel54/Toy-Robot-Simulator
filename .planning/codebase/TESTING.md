# Testing Patterns

**Analysis Date:** 2026-06-27

## Test Framework

**Runner:**
- xUnit v3 (`xunit.v3` version 3.2.2)
- The test project is compiled as a self-hosted executable (`<OutputType>Exe</OutputType>`), which is required by xUnit v3's runner model.
- Config: `ToyRobot.Tests/ToyRobot.Tests.csproj`
- Target framework: `net10.0`, language version `12.0`

**Assertion Library:**
- xUnit built-in assertions (`Assert.*`)

**Run Commands:**
```bash
# Run all tests (NOT dotnet test — the project is OutputType=Exe)
dotnet run --project ToyRobot.Tests

# There is no watch mode or coverage command configured
```

> **Important:** `dotnet test` does NOT work for this project. Tests must be run with `dotnet run --project ToyRobot.Tests`.

## Test File Organization

**Location:** Separate project `ToyRobot.Tests/`, mirroring the main `ToyRobot/` project via a `<ProjectReference>`.

**Naming:** Test file names match the class under test with a `Tests` suffix:
- `ToyRobot.Tests/SimulatorTests.cs` tests `ToyRobot/Simulator.cs`
- `ToyRobot.Tests/CommandParserTests.cs` tests `ToyRobot/CommandParser.cs`
- `ToyRobot.Tests/TableTests.cs` tests `ToyRobot/Table.cs`

**Namespace:** `ToyRobot.Tests`

## Test Naming Convention

Test method names use `PascalCase` with underscores separating logical segments:

```
{MethodUnderTest}_{Condition}_{ExpectedOutcome}
```

Examples:
- `Place_Should_Successfully_Execute`
- `Place_Should_Return_False_And_Not_Set_Robot`
- `Move_Should_Not_Move_If_Going_Out_Of_Bounds`
- `IsValidPosition_Should_ReturnTrue_ForPositionsInsideBounds`
- `TryParsePlaceArgs_Should_Parse`

One test has a copy-paste naming error: `Turn_Left_Should_Turn_Right` (`SimulatorTests.cs` line 96) — the test body exercises `TurnRight` but the method name says "Turn Left".

## Test Structure

**Pattern:** Arrange / Act / Assert (AAA), consistently applied.

```csharp
// Arrange
var robot = new Robot();
var gameBoard = new Simulator(robot);

// Act
var result = gameBoard.Place(1, 2, Direction.North);

// Assert
Assert.Equal(Direction.North, robot.direction);
Assert.Equal(1, robot.xPos);
Assert.Equal(2, robot.yPos);
Assert.True(result);
```

**`[Fact]`** is used for single-scenario tests.

**`[Theory]` with `[MemberData]`** is used for parameterised tests. Data is provided via `static` methods returning `TheoryData<T...>` with the `new()` collection initialiser syntax:

```csharp
public static TheoryData<Direction, Direction> TurnLeftData() => new()
{
    { Direction.East, Direction.North },
    { Direction.North, Direction.West },
    { Direction.West, Direction.South },
    { Direction.South, Direction.East },
};
```

## Test Coverage by Class

**`Simulator` (`SimulatorTests.cs`):**
- `Place` — valid position, out-of-bounds position
- `MoveForward` — forward move, boundary prevention
- `TurnLeft` — all four cardinal directions (via Theory)
- `TurnRight` — all four cardinal directions (via Theory)
- `Report` — output format for multiple positions/directions (via Theory)
- `IsRobotPlaced` — not directly tested in isolation

**`CommandParser` (`CommandParserTests.cs`):**
- `ParseCommand` — basic splitting, command uppercasing, no-arg commands
- `TryParsePlaceArgs` — valid comma-separated inputs

**`Table` (`TableTests.cs`):**
- `IsValidPosition` — all four corners and centre (valid), negative and out-of-range values (invalid), custom dimensions

**`Robot` (`Robot.cs`):** No tests. The constructor initialisation values (`xPos = -1`, `yPos = -1`, `Direction.Unset`) are implicitly verified by `Simulator` tests that check state before placement.

## Coverage Gaps

**`CommandParser` — unhappy paths not tested:**
- `TryParsePlaceArgs` with invalid input (non-numeric coordinates, wrong number of parts, unknown direction) — no tests exist. The method returns `false` but this is never verified.
- `ParseCommand` with empty string or whitespace-only input.
- Case-insensitive command parsing (lowercase input) — the method uppercases `command` but this is untested.

**`Simulator` — `IsRobotPlaced`:**
- Not tested in isolation; its behavior is exercised indirectly via `Program.cs` (untested) logic.

**`Program.cs`:**
- Not tested at all. The main application loop, command dispatch, and user-facing error messages have no test coverage.

**Integration / end-to-end:**
- No integration or end-to-end tests exist. The full command-loop flow (parsing a raw string through to robot state change) is not covered as a unit.

## Mocking

No mocking framework is used. Tests instantiate real `Robot`, `Table`, and `Simulator` objects directly. There is no interface abstraction layer in the production code, so mocking is not possible without refactoring.

## Fixtures and Factories

No shared fixtures or factory helpers. Each test method constructs its own objects inline:
```csharp
var robot = new Robot();
var gameBoard = new Simulator(robot);
```

The variable name `gameBoard` is used consistently across `SimulatorTests.cs` to refer to the `Simulator` instance.

## Coverage Tooling

No code coverage tooling is configured in the project. There is no `coverlet` package reference and no coverage scripts.

---

*Testing analysis: 2026-06-27*
