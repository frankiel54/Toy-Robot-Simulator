<!-- refreshed: 2026-06-27 -->
# Architecture

**Analysis Date:** 2026-06-27

## System Overview

```text
┌──────────────────────────────────────────────────────────────┐
│                    Console Entry Point                        │
│                   `ToyRobot/Program.cs`                       │
│          stdin loop → switch dispatch → stdout                │
└────────────────────┬─────────────────────────────────────────┘
                     │ calls
          ┌──────────▼──────────┐
          │    CommandParser     │
          │ `CommandParser.cs`   │
          │  (static, stateless) │
          └──────────┬──────────┘
                     │ parsed command/args
          ┌──────────▼──────────┐
          │      GameBoard       │
          │   `GameBoard.cs`     │
          │  (stateful facade)   │
          └──────────┬──────────┘
                     │ mutates
          ┌──────────▼──────────┐
          │        Robot         │
          │     `Robot.cs`       │
          │  (state container)   │
          └─────────────────────┘
```

## Component Responsibilities

| Component | Responsibility | File |
|-----------|----------------|------|
| Program | Entry point, REPL loop, command dispatch via switch | `ToyRobot/Program.cs` |
| CommandParser | Splits raw input into command name + args; parses PLACE args | `ToyRobot/CommandParser.cs` |
| GameBoard | Enforces board boundaries, owns all movement and turn logic | `ToyRobot/GameBoard.cs` |
| Robot | Plain data container for position (xPos, yPos) and direction | `ToyRobot/Robot.cs` |
| Direction | Enum: North=0, East=1, South=2, West=3, Unset=4 | `ToyRobot/Direction.cs` |
| Commands | Empty stub class — no behaviour implemented | `ToyRobot/Commands.cs` |

## Pattern Overview

**Overall:** Procedural console REPL backed by a facade object.

**Key Characteristics:**
- No command objects — dispatch is a raw `switch` in `Program.cs`
- `GameBoard` acts as a facade/service: it holds the `Robot` reference and enforces all rules
- `CommandParser` is a pure static utility (no state, no DI)
- `Robot` is a mutable data bag (public setters); it does not enforce any invariants

## Layers

**Presentation / I/O:**
- Purpose: Read stdin, print stdout, dispatch commands
- Location: `ToyRobot/Program.cs`
- Contains: REPL while-loop, switch statement, Console.WriteLine calls
- Depends on: CommandParser, GameBoard, Direction
- Used by: nothing (top-level entry point)

**Parsing:**
- Purpose: Convert raw string input to typed values
- Location: `ToyRobot/CommandParser.cs`
- Contains: `ParseCommand` (splits verb from args), `TryParsePlaceArgs` (parses X,Y,DIRECTION)
- Depends on: Direction enum
- Used by: Program.cs

**Domain / Logic:**
- Purpose: All game rules — boundary checks, movement, rotation, placement guard
- Location: `ToyRobot/GameBoard.cs`
- Contains: `Place`, `MoveForward`, `TurnLeft`, `TurnRight`, `Report`, `IsRobotPlaced`
- Depends on: Robot, Direction
- Used by: Program.cs

**State:**
- Purpose: Hold current robot position and facing direction
- Location: `ToyRobot/Robot.cs`
- Contains: `xPos`, `yPos`, `direction` (all public get/set)
- Depends on: Direction
- Used by: GameBoard

## Data Flow

### Primary Request Path

1. User types a line into stdin — `Console.ReadLine()` in `Program.cs` (line 21)
2. `CommandParser.ParseCommand` splits on the first space → `command` (uppercased) + `args` string (`CommandParser.cs` line 6)
3. `Program.cs` switch matches `command` and calls the appropriate `GameBoard` method (lines 26–64)
4. For PLACE: `CommandParser.TryParsePlaceArgs` further parses `args` into `int x, int y, Direction` (`CommandParser.cs` line 14)
5. `GameBoard` validates bounds and/or executes movement, mutating `Robot` fields directly
6. REPORT produces a formatted string returned to `Program.cs` and written to stdout

### Boundary Guard Flow (MOVE)

1. `GameBoard.MoveForward` computes candidate x/y from current direction
2. If candidate is outside `[0, XBoundary]` or `[0, YBoundary]`, returns `false` without updating `Robot`
3. Otherwise updates `Robot.xPos` / `Robot.yPos` and returns `true`

**State Management:**
- All mutable state lives in the single `Robot` instance held by `GameBoard`
- `GameBoard.RobotPlaced` is a private bool flag; commands other than PLACE are silently ignored until it is `true`

## Key Abstractions

**Direction Enum:**
- Purpose: Compass facing; also used as arithmetic for rotation (enum integer arithmetic ±1 with wrap-around at North/West boundary)
- File: `ToyRobot/Direction.cs`
- Pattern: Ordered enum (North=0 … West=3) so `TurnLeft`/`TurnRight` can decrement/increment with manual wrap

**GameBoard:**
- Purpose: Single point of truth for all game rules; shields `Robot` state from illegal mutations
- File: `ToyRobot/GameBoard.cs`
- Pattern: Facade — callers never touch `Robot` directly

## Entry Points

**Console REPL:**
- Location: `ToyRobot/Program.cs` (top-level statements, no explicit `Main`)
- Triggers: `dotnet run` from `ToyRobot/` directory
- Responsibilities: Print welcome, loop forever reading stdin, dispatch to GameBoard

## Architectural Constraints

- **Threading:** Single-threaded; no async, no background workers
- **Global state:** `GameBoard` and its `Robot` are instantiated once as a local variable in `Program.cs` top-level statements (line 4); effectively a singleton for the process lifetime
- **Circular imports:** None detected
- **Placement guard:** All non-PLACE commands silently no-op if `IsRobotPlaced()` is false — no error is surfaced to the user

## Anti-Patterns

### Empty Commands class

**What happens:** `ToyRobot/Commands.cs` exists but contains only an empty class body.
**Why it's wrong:** Suggests an intended Command pattern (one class per command) that was never implemented; it is dead code and misleads future readers.
**Do this instead:** Either delete the file or implement the Command pattern — one `ICommand` interface with `Execute()` and a concrete class per command, removing the switch from `Program.cs`.

### Robot with public mutable setters

**What happens:** `Robot.xPos`, `Robot.yPos`, and `Robot.direction` all have public `set` accessors (`Robot.cs` lines 9–11).
**Why it's wrong:** Any code outside `GameBoard` can mutate robot state and bypass boundary validation.
**Do this instead:** Make setters `internal` or `private set`; expose mutation only through `GameBoard` methods.

### ParseCommand does not validate empty input

**What happens:** If the user presses Enter with no input, `parts[0]` is an empty string; the switch falls through to the default "Invalid selection" branch with no special message.
**Why it's wrong:** Silent partial failure; TODO comments acknowledge missing error handling (`CommandParser.cs` lines 5, 13).
**Do this instead:** Return early or surface a typed parse result (e.g., `Result<ParsedCommand, string>`) so callers can distinguish parse failure from unknown command.

## Error Handling

**Strategy:** Best-effort silent ignore. Commands issued before PLACE are dropped. Out-of-bounds MOVE returns `false` (ignored by caller). Invalid PLACE args print one error line.

**Patterns:**
- `TryParse` pattern used in `CommandParser.TryParsePlaceArgs` — returns `bool`, out params set to defaults on failure
- `GameBoard.Place` and `MoveForward` return `bool` but callers in `Program.cs` do not always check the return value

## Cross-Cutting Concerns

**Logging:** None — output is via `Console.WriteLine` only
**Validation:** Boundary validation in `GameBoard`; argument parsing validation in `CommandParser`
**Authentication:** Not applicable (local console app)

---

*Architecture analysis: 2026-06-27*
