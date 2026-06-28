# Phase 4 — Direction extension methods for turn and movement

## Goal
- Replace order-dependent enum arithmetic in `TurnLeft`/`TurnRight` with explicit switch lookups.
- Move turn and forward-movement delta calculations into `Direction` as extension methods.
- `Simulator` delegates to those extension methods — its own methods become one-liners.

## Constraints
- External behaviour of `Simulator` is unchanged.
- All existing tests must remain green.
- `Direction.Unset` must be handled gracefully in all extension methods (no-op / zero delta).

---

## Task 1 — Add DirectionExtensions to Direction.cs

**File:** `ToyRobot/Direction.cs`

Add a `public static class DirectionExtensions` in the same file (same namespace). Do not modify the `Direction` enum itself.

### TurnLeft extension
```csharp
public static Direction TurnLeft(this Direction direction) => direction switch
{
    Direction.North => Direction.West,
    Direction.West  => Direction.South,
    Direction.South => Direction.East,
    Direction.East  => Direction.North,
    _               => direction,
};
```

### TurnRight extension
```csharp
public static Direction TurnRight(this Direction direction) => direction switch
{
    Direction.North => Direction.East,
    Direction.East  => Direction.South,
    Direction.South => Direction.West,
    Direction.West  => Direction.North,
    _               => direction,
};
```

### GetMoveDelta extension
Returns the `(dx, dy)` step for one forward move in the given direction.
```csharp
public static (int dx, int dy) GetMoveDelta(this Direction direction) => direction switch
{
    Direction.North => ( 0,  1),
    Direction.South => ( 0, -1),
    Direction.East  => ( 1,  0),
    Direction.West  => (-1,  0),
    _               => ( 0,  0),
};
```

**Verify:** `dotnet build` — no errors.

---

## Task 2 — Simplify Simulator using the extension methods

**File:** `ToyRobot/Simulator.cs`

Replace the bodies of `TurnLeft`, `TurnRight`, and `MoveForward` with calls to the new extension methods.

### TurnLeft
```csharp
public void TurnLeft() => Robot.direction = Robot.direction.TurnLeft();
```

### TurnRight
```csharp
public void TurnRight() => Robot.direction = Robot.direction.TurnRight();
```

### MoveForward
```csharp
public bool MoveForward()
{
    var (dx, dy) = Robot.direction.GetMoveDelta();
    var x = Robot.xPos + dx;
    var y = Robot.yPos + dy;

    if (!Table.IsValidPosition(x, y)) return false;

    Robot.xPos = x;
    Robot.yPos = y;
    return true;
}
```

Remove the unused `switch` block that was previously inside `MoveForward`.

**Verify:** `dotnet build` — no errors.

---

## Task 3 — Add DirectionExtensionsTests

**File:** `ToyRobot.Tests/DirectionExtensionsTests.cs` (new file)

Test the extension methods directly, independent of `Simulator`.

### TurnLeft tests (Theory)
| Input   | Expected |
|---------|----------|
| North   | West     |
| West    | South    |
| South   | East     |
| East    | North    |
| Unset   | Unset    |

### TurnRight tests (Theory)
| Input   | Expected |
|---------|----------|
| North   | East     |
| East    | South    |
| South   | West     |
| West    | North    |
| Unset   | Unset    |

### GetMoveDelta tests (Theory)
| Input   | Expected dx | Expected dy |
|---------|-------------|-------------|
| North   | 0           | 1           |
| South   | 0           | -1          |
| East    | 1           | 0           |
| West    | -1          | 0           |
| Unset   | 0           | 0           |

**Verify:** `dotnet run --project ToyRobot.Tests` — all tests pass (existing + new).

---

## Acceptance criteria
- [ ] `Direction.cs` contains `DirectionExtensions` with `TurnLeft`, `TurnRight`, `GetMoveDelta`
- [ ] No integer arithmetic on `Direction` values anywhere in the codebase
- [ ] `Simulator.TurnLeft` and `TurnRight` are single expression-body lines
- [ ] `Simulator.MoveForward` switch is replaced with `GetMoveDelta`
- [ ] All existing tests pass
- [ ] `DirectionExtensionsTests` covers all 4 directions + `Unset` for each method
