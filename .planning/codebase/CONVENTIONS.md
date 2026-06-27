# Coding Conventions

**Analysis Date:** 2026-06-27

## Naming Patterns

**Classes:**
- PascalCase: `Robot`, `Table`, `Simulator`, `CommandParser`

**Methods:**
- PascalCase: `Place`, `MoveForward`, `TurnLeft`, `TurnRight`, `Report`, `IsRobotPlaced`, `IsValidPosition`, `ParseCommand`, `TryParsePlaceArgs`
- `Try`-prefix pattern used for methods that return a bool and populate `out` parameters: `TryParsePlaceArgs`

**Properties (on `Robot`):**
- camelCase — inconsistent with C# conventions: `xPos`, `yPos`, `direction`
- Private class-level fields on `Table` use PascalCase: `Width`, `Height` (non-standard; convention is `_camelCase`)
- Private auto-properties on `Simulator` use PascalCase correctly: `Robot`, `RobotPlaced`, `Table`

**Enum:**
- Type name PascalCase: `Direction` (`ToyRobot/Direction.cs`)
- Members PascalCase: `North`, `East`, `South`, `West`, `Unset`
- `Unset` is a sentinel/default value representing an unplaced robot

**Local Variables:**
- camelCase: `x`, `y`, `parts`, `command`, `commandArgs`, `direction`

**Naming inconsistencies to be aware of:**
- `Robot` public properties (`xPos`, `yPos`, `direction`) use camelCase instead of the C# standard PascalCase. This is the primary naming inconsistency in the codebase.
- `Table` private fields (`Width`, `Height`) use PascalCase instead of the conventional underscore-prefixed camelCase (`_width`, `_height`).

## Code Style Patterns

**Expression-bodied members:**
Used selectively. `Table.IsValidPosition` (`ToyRobot/Table.cs`) uses an expression body:
```csharp
public bool IsValidPosition(int x, int y) =>
    x >= 0 && x < Width && y >= 0 && y < Height;
```
`Simulator.IsRobotPlaced` (`ToyRobot/Simulator.cs`) is a one-liner block body that could be expression-bodied:
```csharp
public bool IsRobotPlaced () { return RobotPlaced; }
```
This is inconsistent; prefer `=> RobotPlaced;`.

**Brace style:**
Mixed within the same file. `Simulator.cs` uses K&R (opening brace same line) and Allman (opening brace next line) styles interchangeably.

**`var` usage:**
Used consistently for local variables where the type is apparent from the right-hand side.

**`out` parameters:**
Used in `CommandParser` (`ToyRobot/CommandParser.cs`) for multi-value returns rather than tuples or records.

**Default parameter values:**
Used in `Table` (`width = 5`, `height = 5`) and `Simulator` constructors (`x = 5`, `y = 5`).

**Static utility class:**
`CommandParser` is a `static` class with only `static` methods — appropriate since it holds no state.

**Top-level statements:**
`ToyRobot/Program.cs` uses C# top-level statements (no explicit `Program` class or `Main` method).

**Unused imports:**
`Robot.cs`, `Direction.cs`, `Table.cs`, and `Simulator.cs` all include `using System.Collections.Generic` and `using System.Text` that are not used.

## Error Handling

**Approach:** Return-value based. Methods that can fail return `bool` (`Place`, `MoveForward`, `TryParsePlaceArgs`); callers check the result. No exceptions are thrown for invalid input.

**`Program.cs` behavior:**
- Invalid PLACE arguments: prints a message to the console.
- Commands issued before robot is placed: silently ignored (no user feedback).
- Unknown commands: prints a generic invalid selection message.

**No `try`/`catch` blocks exist anywhere in the codebase.**

**Outstanding TODOs in `ToyRobot/CommandParser.cs`:**
- `// TODO: Error handling` on `ParseCommand`
- `// TODO: Add error handling` on `TryParsePlaceArgs`

**Outstanding TODO in `ToyRobot/Program.cs`:**
- Commented-out code for an initial placement prompt that was never completed.

## Null Handling

**Null context:** `<Nullable>enable</Nullable>` is set in `ToyRobot.Tests/ToyRobot.Tests.csproj`.

**Null coalescing:**
The only null guard in the codebase is in `Program.cs`:
```csharp
Console.ReadLine() ?? string.Empty
```

No null checks exist on method arguments. Methods assume valid, non-null inputs.

## Notable Observations

- `Commands.cs` was listed as a source file but does not exist on disk. It appears planned but never created.
- Turn rotation in `Simulator.TurnLeft`/`TurnRight` (`ToyRobot/Simulator.cs`) relies on the enum's underlying integer ordering (`direction - 1`, `direction + 1`). Reordering `Direction` enum members would silently break rotation logic.
- `Direction.Unset` and the `RobotPlaced` bool in `Simulator` are redundant representations of the same state (whether the robot has been placed).
- The `Report` output format (`"1, 2, North"`) is an inline interpolated string with no named constant, making format changes fragile.

---

*Convention analysis: 2026-06-27*
