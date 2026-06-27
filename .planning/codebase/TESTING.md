# Testing Patterns

**Analysis Date:** 2026-06-27

## Test Framework

**Runner:**
- xUnit (referenced as `Xunit` and `Xunit.v3` in `ToyRobot.Tests/GameBoardTests.cs`)
- Config: no `xunit.runner.json` detected; default settings assumed

**Assertion Library:**
- xUnit built-in: `Assert.Equal`, `Assert.True`, `Assert.False`

**Run Commands:**
```bash
dotnet test          # Run all tests
dotnet test --watch  # Watch mode (if dotnet-watch installed)
dotnet test --collect:"XPlat Code Coverage"  # Coverage
```

## Test File Organization

**Location:**
- Separate test project: `ToyRobot.Tests/`
- Test files sit at the project root, not mirroring source sub-folders

**Naming:**
- Test files: `{SourceClass}Tests.cs`
  - `ToyRobot.Tests/GameBoardTests.cs` tests `GameBoard`
  - `ToyRobot.Tests/CommandParserTests.cs` tests `CommandParser`

**Namespace:**
- `namespace ToyRobot.Tests` — matches test project name

## Test Structure

**Suite Organization:**
```csharp
public class GameBoardTests
{
    [Fact]
    public void MethodName_Should_ExpectedBehavior() { ... }

    [Theory]
    [MemberData(nameof(DataMethod))]
    public void MethodName_Should_ExpectedBehavior(Type param, ...) { ... }

    public static TheoryData<T1, T2> DataMethod() => new() { { val1, val2 }, ... };
}
```

**Pattern:**
- Arrange / Act / Assert (AAA) — consistently followed in all tests
- Each test creates its own `Robot` and `GameBoard` instances (no shared setup; no `IClassFixture` or constructor injection)
- No `[SetUp]` equivalent — setup is inline per test

## Test Naming Convention

Format: `SubjectOrAction_Should_Outcome`

Examples:
- `Place_Should_Successfully_Execute`
- `Place_Should_Return_False_And_Not_Set_Robot`
- `Move_Should_Move_Position_Forward`
- `Move_Should_Not_Move_If_Going_Out_Of_Bounds`
- `Turn_Left_Should_Turn_Left`
- `ParseCommand_Should_Parse`
- `TryParsePlaceArgs_Should_Parse`

**Inconsistency:** `Turn_Left_Should_Turn_Right` (line 96 of `GameBoardTests.cs`) is a copy-paste naming error — the method name says "Turn_Left" but it tests `TurnRight`.

## Mocking

**Framework:** None — no mocking library detected (no Moq, NSubstitute, etc.)

**Approach:** Direct instantiation of all collaborators. `GameBoard` is tested by passing a real `Robot` instance and asserting against that `Robot`'s public properties after the fact.

## Fixtures and Factories

**Test Data:**
- `TheoryData<T...>` static methods used for parameterised cases:

```csharp
public static TheoryData<Direction, Direction> TurnLeftData() => new()
{
    { Direction.East, Direction.North },
    { Direction.North, Direction.West },
    { Direction.West, Direction.South },
    { Direction.South, Direction.East },
};
```

- Data providers are defined as `public static` methods on the same test class, referenced via `[MemberData(nameof(...))]`

**Location:** Inline in each test class; no shared fixture file

## Coverage

**Requirements:** None enforced (no coverage threshold in project files)

**Assessed coverage by class:**

| Class | Coverage | Notes |
|-------|----------|-------|
| `GameBoard` | Good | `Place`, `MoveForward`, `TurnLeft`, `TurnRight`, `Report` all tested; `IsRobotPlaced` not directly tested but exercised indirectly via `Program.cs` |
| `CommandParser` | Partial | `ParseCommand` and `TryParsePlaceArgs` happy-path tested; no tests for malformed input (wrong arg count, non-integer coords, invalid direction string) |
| `Robot` | None | No dedicated tests; only verified as a data carrier through `GameBoard` tests |
| `Direction` | None | Enum values exercised indirectly; no edge-case tests |
| `Commands` | None | Empty stub — nothing to test |
| `Program.cs` | None | Entry-point / I/O loop; not unit-testable in current form |

## Test Types

**Unit Tests:**
- All tests are unit tests scoped to a single class
- No integration tests, no end-to-end tests

**E2E Tests:**
- Not present. The main game loop in `Program.cs` reads from `Console` directly, making it untestable without refactoring to accept an injectable `TextReader`/`TextWriter`

## Coverage Gaps

**`CommandParser` invalid input:**
- `TryParsePlaceArgs` with fewer than 3 comma-separated parts
- `TryParsePlaceArgs` with non-integer coordinate values
- `TryParsePlaceArgs` with an unrecognised direction string
- `ParseCommand` with an empty string input

**`GameBoard` boundary edge cases:**
- `Place` with negative coordinates (currently not guarded — `x < 0` or `y < 0` are not checked in `Place`, only in `MoveForward`)
- `MoveForward` when robot has not been placed (`direction == Direction.Unset` causes silent arithmetic on an out-of-range enum value)
- `TurnLeft` / `TurnRight` when direction is `Unset`

**`Robot` initialisation:**
- No test asserts the sentinel defaults (`xPos = -1`, `yPos = -1`, `direction = Direction.Unset`)

---

*Testing analysis: 2026-06-27*
