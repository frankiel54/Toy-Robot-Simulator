# Coding Conventions

**Analysis Date:** 2026-06-27

## Naming Patterns

**Classes:**
- PascalCase throughout: `Robot`, `GameBoard`, `CommandParser`, `Commands`
- Class names are clear nouns describing their responsibility

**Methods:**
- PascalCase: `Place`, `TurnLeft`, `TurnRight`, `MoveForward`, `Report`, `IsRobotPlaced`, `ParseCommand`, `TryParsePlaceArgs`
- Boolean-returning methods use `Is` prefix (`IsRobotPlaced`) or `Try` prefix (`TryParsePlaceArgs`) following .NET conventions

**Properties:**
- PascalCase for private properties on `GameBoard`: `XBoundary`, `YBoundary`, `Robot`, `RobotPlaced`
- **Inconsistency:** Public properties on `Robot` use camelCase (`xPos`, `yPos`, `direction`) — violates .NET property naming conventions; they should be PascalCase (`XPos`, `YPos`, `Direction`)

**Fields / Local Variables:**
- camelCase: `x`, `y`, `parts`, `command`, `commandArgs`, `report`

**Enums:**
- Enum type: PascalCase (`Direction`)
- Enum members: PascalCase (`North`, `East`, `South`, `West`, `Unset`)
- `Unset` sentinel value used to represent "not yet placed" state

**Parameters:**
- camelCase: `x`, `y`, `direction`, `input`, `args`, `robot`

## Code Style

**Brace placement:**
- Inconsistent across the codebase. `GameBoard` mixes Allman-style (opening brace on new line) and K&R-style (opening brace on same line) within the same file:
  - `TurnLeft()` — K&R: `public void TurnLeft() {`
  - `TurnRight()` — Allman: `public void TurnRight()\n{`
- `CommandParser` uses Allman consistently
- No `.editorconfig` or formatter config detected to enforce a style

**Expression bodies:**
- Not used; all methods use block bodies even for trivial one-liners (e.g., `IsRobotPlaced`)

**`var` usage:**
- Used consistently for local variables where type is apparent: `var parts`, `var x`, `var y`, `var report`

**`out` parameters:**
- Used in `ParseCommand` and `TryParsePlaceArgs` following the .NET `Try*` pattern

**String interpolation:**
- Used in `Report()`: `$"{Robot.xPos}, {Robot.yPos}, {Robot.direction.ToString()}"`
- `.ToString()` call on the enum is redundant inside an interpolated string

**Top-level statements:**
- `Program.cs` uses C# 9+ top-level statements (no explicit `Main` method)

**Unused scaffolding:**
- `Commands.cs` is an empty class — a placeholder with no implementation
- Several unused `using` directives appear in most files (`System.Collections.Generic`, `System.Text`)

## Error Handling

**Current approach:**
- Methods return `bool` to signal success/failure (`Place`, `MoveForward`, `TryParsePlaceArgs`)
- No exceptions are thrown by domain logic
- `Program.cs` handles invalid commands with `Console.WriteLine` messages in the `default` switch case
- Null input from `Console.ReadLine()` is handled with the null-coalescing operator: `Console.ReadLine() ?? string.Empty`

**Gaps (annotated with TODO comments):**
- `CommandParser` has two `// TODO: Error handling` and `// TODO: Add error handling` comments indicating error handling is acknowledged but not yet implemented
- `Program.cs` has `// TODO: Add some more error messages for when things dont go right`
- `TurnLeft` and `TurnRight` have no guard for the `Direction.Unset` sentinel — arithmetic on `Unset` would produce an out-of-range enum value silently

## Null Handling

- Null handled defensively at the entry point (`Console.ReadLine() ?? string.Empty`)
- No nullable reference type annotations (`#nullable enable`) detected — the project does not opt in to C# 8+ nullable analysis
- `Robot` initializes fields to sentinel values (`xPos = -1`, `yPos = -1`, `direction = Direction.Unset`) rather than using nullable types

## Notable Inconsistencies

| Issue | Location |
|-------|----------|
| camelCase public properties instead of PascalCase | `ToyRobot/Robot.cs` lines 9-11 |
| Brace style mixed (K&R vs Allman) | `ToyRobot/GameBoard.cs` |
| `Commands` class is an empty stub | `ToyRobot/Commands.cs` |
| Redundant `.ToString()` in string interpolation | `ToyRobot/GameBoard.cs` line 86 |
| Unused `using` directives in most files | All files under `ToyRobot/` |
| No formatter or linter config enforcing any of the above | Project root |

---

*Convention analysis: 2026-06-27*
