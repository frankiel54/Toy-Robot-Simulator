# Phase 6 — Fix Robot encapsulation

## Goal

Remove external access to `Robot`'s mutable state and make `Simulator` the sole owner of its `Robot` instance.

Two problems addressed:
- **Inverted ownership:** `Robot` is constructed outside `Simulator` and passed in, leaving callers free to hold a reference and read (or write) state directly.
- **Public setters:** `Robot.XPos`, `Robot.YPos`, and `Robot.Direction` have fully public setters, allowing any code to mutate robot state without going through `Simulator`'s boundary checks.

## Approach

1. Add `[assembly: InternalsVisibleTo("ToyRobot.Tests")]` so test code can still access `internal` members.
2. Change Robot setters to `internal set` — Simulator (same assembly) can still write; external consumers cannot.
3. Add read-only state accessors to `Simulator` (`X`, `Y`, `Facing`) so tests verify state through `Simulator`'s API instead of a raw `Robot` reference.
4. Remove the `Robot robot` constructor parameter from `Simulator` — `Robot` is constructed internally.
5. Update `Application.cs` (no longer passes `new Robot()`).
6. Rewrite `SimulatorTests.cs` — replace the external `robot` variable with `gameBoard.X`, `gameBoard.Y`, `gameBoard.Facing`.

## Cascade impact

Removing `Robot` from the `Simulator` constructor affects every file that calls `new Simulator(robot, ...)`:
- `ToyRobot/Application.cs` — `new Simulator(new Robot())`
- `ToyRobot.Tests/SimulatorTests.cs` — every test method creates `var robot = new Robot()` and passes it to `new Simulator(robot)`

`RobotTests.cs` uses object-initialiser syntax (`new Robot { XPos = startX, ... }`) which requires write access; covered by `InternalsVisibleTo` from Task 1.

---

## Task 1 — Add InternalsVisibleTo

**File:** `ToyRobot/AssemblyInfo.cs` (new file)

```csharp
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ToyRobot.Tests")]
```

This lets the test assembly access `internal` members of `ToyRobot` — needed so `RobotTests.cs` can continue using object-initialiser syntax after Robot setters become `internal set` in Task 2.

**Verify:** `dotnet build` — no errors.

---

## Task 2 — Restrict Robot property setters to `internal set`

**File:** `ToyRobot/Robot.cs`

Change all three property declarations:

```csharp
public int XPos      { get; internal set; }
public int YPos      { get; internal set; }
public Direction Direction { get; internal set; }
```

`Simulator` (same assembly) can still assign these. `RobotTests` can still use object-initialiser syntax via the `InternalsVisibleTo` grant from Task 1. No external consumer can mutate robot state directly.

**Verify:** `dotnet build` — no errors.

---

## Task 3 — Add read-only state accessors to Simulator

**File:** `ToyRobot/Simulator.cs`

Add three public read-only properties after the existing `IsRobotPlaced()` method:

```csharp
public int X         => Robot.XPos;
public int Y         => Robot.YPos;
public Direction Facing => Robot.Direction;
```

These expose the robot's current state through `Simulator`'s API. Tests will use these instead of accessing `robot.XPos` etc. directly.

**Verify:** `dotnet build` — no errors. This is an additive change; no existing code breaks.

---

## Task 4 — Internalize Robot ownership in Simulator

**File:** `ToyRobot/Simulator.cs`

Remove the `Robot robot` constructor parameter and initialise `Robot` as a property initialiser:

```csharp
private Robot Robot { get; } = new Robot();
```

Update the constructor signature and body:

```csharp
public Simulator(int width = 5, int height = 5)
{
    Table = new Table(width, height);
}
```

(`RobotPlaced` defaults to `false`; no explicit assignment needed.)

**Verify:** `dotnet build` — expect compile errors in `Application.cs` and `SimulatorTests.cs` (they pass a `Robot` argument to `Simulator`); fixed in Tasks 5 and 6.

---

## Task 5 — Update Application.cs

**File:** `ToyRobot/Application.cs`

Remove `new Robot()` from the Simulator construction:

```csharp
private readonly Simulator _simulator = new();
```

**Verify:** `dotnet build` — `Application.cs` errors resolve; `SimulatorTests.cs` errors remain until Task 6.

---

## Task 6 — Rewrite SimulatorTests.cs

**File:** `ToyRobot.Tests/SimulatorTests.cs`

For every test method:
- Remove `var robot = new Robot();`
- Replace `new Simulator(robot)` with `new Simulator()`
- Replace `robot.Direction` with `gameBoard.Facing`
- Replace `robot.XPos` with `gameBoard.X`
- Replace `robot.YPos` with `gameBoard.Y`

No test logic changes — only the source of state assertions shifts from an external `Robot` reference to `Simulator`'s own accessors.

**Verify:** `dotnet run --project ToyRobot.Tests` — all tests pass.

---

## Acceptance criteria

- [ ] `ToyRobot/AssemblyInfo.cs` exists with `InternalsVisibleTo("ToyRobot.Tests")`
- [ ] `Robot.XPos`, `Robot.YPos`, `Robot.Direction` all use `internal set`
- [ ] `Simulator` has no `Robot robot` constructor parameter
- [ ] `Simulator.X`, `Simulator.Y`, `Simulator.Facing` are public read-only properties
- [ ] `Application.cs` uses `new Simulator()` with no Robot argument
- [ ] `SimulatorTests.cs` contains no `new Robot()` and no direct `robot.` property access
- [ ] All existing tests pass
