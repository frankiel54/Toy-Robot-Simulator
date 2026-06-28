# Phase 3 — Refactor Application loop: guard before switch

## Goal
Eliminate repeated `IsRobotPlaced()` checks in every switch case by handling PLACE first (if + continue) and applying a single shared guard for all other placement-required commands. The switch cases become simple single-purpose blocks with no guard logic.

## Constraints
- External behaviour is identical except: REPORT before placement now prints "Robot has not been placed yet." instead of silently doing nothing (consistent with Move/Left/Right behaviour — an improvement).
- All existing tests must stay green or be updated to reflect the above behaviour change.

---

## Task 1 — Refactor Application.Run loop

**File:** `ToyRobot/Application.cs`

Replace the current switch block inside the `while` loop with the following structure:

```csharp
var parsed = CommandParser.ParseCommand(line);

// Handle PLACE separately — valid before robot is placed
if (parsed.Type == CommandType.Place)
{
    if (parsed.Options is { } opts)
    {
        if (_simulator.Place(opts.X, opts.Y, opts.Facing))
            _output.WriteLine("Robot placed.");
        else
            _output.WriteLine("Invalid position — robot was not placed.");
    }
    else
    {
        _output.WriteLine("\nInvalid PLACE arguments. Expected format: PLACE X,Y,DIRECTION");
    }
    continue;
}

// All other placement-required commands share a single guard
if (parsed.Type != CommandType.Unknown && !_simulator.IsRobotPlaced())
{
    _output.WriteLine("Robot has not been placed yet.");
    continue;
}

switch (parsed.Type)
{
    case CommandType.Move:
        if (_simulator.MoveForward())
            _output.WriteLine("Moved forward.");
        else
            _output.WriteLine("Move blocked — robot is at the edge of the table.");
        break;
    case CommandType.Left:
        _simulator.TurnLeft();
        _output.WriteLine("Turned left.");
        break;
    case CommandType.Right:
        _simulator.TurnRight();
        _output.WriteLine("Turned right.");
        break;
    case CommandType.Report:
        _output.WriteLine(_simulator.Report());
        break;
    default:
        _output.WriteLine("\nInvalid selection. Please try again.");
        break;
}
```

**Verify:** `dotnet build` — no errors.

---

## Task 2 — Update ApplicationTests

**File:** `ToyRobot.Tests/ApplicationTests.cs`

One test reflects the behaviour change:

- Rename `Report_Before_Place_Should_Produce_No_Output` → `Report_Before_Place_Should_Print_Not_Placed_Message`
- Replace its assertions: assert `Contains("Robot has not been placed yet.")` instead of asserting the absence of `-1`/`Unset`.

No other test changes are needed — the "Robot has not been placed yet." message is unchanged for Move/Left/Right, and all other paths are unaffected.

**Verify:** `dotnet run --project ToyRobot.Tests` — all tests pass.

---

## Acceptance criteria
- [ ] No `IsRobotPlaced()` call inside any switch case
- [ ] PLACE is handled before the switch with `if + continue`
- [ ] A single shared guard covers Move, Left, Right, Report before placement
- [ ] Unknown command still reaches `default` regardless of placement state
- [ ] All tests pass
