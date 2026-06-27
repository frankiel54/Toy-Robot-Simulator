# Phase 5 — Fix naming inconsistencies

## Goal
Apply consistent C# naming conventions across the codebase:
- Public properties → PascalCase
- Private fields → `_camelCase`
- Constructor parameters → meaningful names

## Issues to fix

| File | Current | Correct | Rule |
|---|---|---|---|
| `Robot.cs` | `xPos`, `yPos`, `direction` | `XPos`, `YPos`, `Direction` | public properties → PascalCase |
| `Table.cs` | `Width`, `Height` (private fields) | `_width`, `_height` | private fields → `_camelCase` |
| `Simulator.cs` constructor | `int x, int y` params | `int width, int height` | params should reflect meaning |

## Cascade impact
Renaming `Robot` properties affects every file that reads or writes them:
- `Simulator.cs` — `Place`, `MoveForward`, `TurnLeft`, `TurnRight`, `Report`
- `Robot.cs` — `GetNextPosition` and constructor
- `ToyRobot.Tests/SimulatorTests.cs` — direct property access assertions
- `ToyRobot.Tests/RobotTests.cs` — property initialiser in test data

---

## Task 1 — Fix Robot.cs property names

**File:** `ToyRobot/Robot.cs`

- Rename `xPos` → `XPos`
- Rename `yPos` → `YPos`
- Rename `direction` → `Direction`

Update constructor assignments and `GetNextPosition` body to use the new names.

**Verify:** `dotnet build` — expect compile errors in Simulator and tests (they reference old names); fix those in subsequent tasks.

---

## Task 2 — Fix Table.cs private field names

**File:** `ToyRobot/Table.cs`

- Rename private field `Width` → `_width`
- Rename private field `Height` → `_height`
- Update `IsValidPosition` body and constructor assignments to use `_width` / `_height`

This change is self-contained — `Table` fields are private and not referenced outside the class.

**Verify:** `dotnet build` (partial — Robot rename errors still present from Task 1 cascade).

---

## Task 3 — Update Simulator.cs

**File:** `ToyRobot/Simulator.cs`

- Constructor: rename params `int x, int y` → `int width, int height`; update `new Table(x, y)` → `new Table(width, height)`
- `Place`: `Robot.yPos` → `Robot.YPos`, `Robot.xPos` → `Robot.XPos`, `Robot.direction` → `Robot.Direction`
- `TurnLeft`: `Robot.direction` → `Robot.Direction` (appears twice)
- `TurnRight`: `Robot.direction` → `Robot.Direction` (appears twice)
- `MoveForward`: `Robot.xPos` → `Robot.XPos`, `Robot.yPos` → `Robot.YPos`
- `Report`: `Robot.xPos` → `Robot.XPos`, `Robot.yPos` → `Robot.YPos`, `Robot.direction` → `Robot.Direction`

**Verify:** `dotnet build` — Simulator errors resolved; test errors may remain.

---

## Task 4 — Update SimulatorTests.cs

**File:** `ToyRobot.Tests/SimulatorTests.cs`

Replace all direct robot property accesses:
- `robot.direction` → `robot.Direction`
- `robot.xPos` → `robot.XPos`
- `robot.yPos` → `robot.YPos`

**Verify:** `dotnet build` — no errors.

---

## Task 5 — Update RobotTests.cs

**File:** `ToyRobot.Tests/RobotTests.cs`

Update property initialisers in `GetNextPositionData`:
- `xPos = startX` → `XPos = startX`
- `yPos = startY` → `YPos = startY`
- `direction = direction` → `Direction = direction`

**Verify:** `dotnet run --project ToyRobot.Tests` — all tests pass.

---

## Acceptance criteria
- [ ] All public properties on `Robot` are PascalCase (`XPos`, `YPos`, `Direction`)
- [ ] All private fields on `Table` are `_camelCase` (`_width`, `_height`)
- [ ] `Simulator` constructor params are `width`/`height`
- [ ] No camelCase public property names remain in the codebase
- [ ] All 69 existing tests pass
