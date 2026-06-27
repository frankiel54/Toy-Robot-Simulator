# Codebase Structure

**Analysis Date:** 2026-06-27

## Directory Layout

```
ToyRobot/                         # Solution root
├── ToyRobot/                     # Main console application project
│   ├── ToyRobot.csproj           # Project file (net10.0, OutputType=Exe)
│   ├── Program.cs                # Entry point — REPL loop and command dispatch
│   ├── Simulator.cs              # Facade/coordinator for all robot operations
│   ├── Robot.cs                  # Plain data object — position and direction state
│   ├── Table.cs                  # Grid boundary model — validates positions
│   ├── Direction.cs              # Enum: North, East, South, West, Unset
│   └── CommandParser.cs          # Static text parsing utilities
└── ToyRobot.Tests/               # xUnit test project
    ├── ToyRobot.Tests.csproj     # Project file (net10.0, references ToyRobot)
    ├── SimulatorTests.cs         # Tests for Simulator (Place, Move, Turn, Report)
    ├── CommandParserTests.cs     # Tests for CommandParser parsing methods
    └── TableTests.cs             # Tests for Table boundary validation
```

## Key File Responsibilities

| File | Responsibility |
|------|---------------|
| `ToyRobot/Program.cs` | Top-level statements; stdin REPL loop; switch dispatch to Simulator |
| `ToyRobot/Simulator.cs` | Place, MoveForward, TurnLeft, TurnRight, Report — all domain logic; owns Table |
| `ToyRobot/Robot.cs` | Data container for xPos, yPos, direction; no behaviour |
| `ToyRobot/Table.cs` | Holds grid dimensions; exposes `IsValidPosition(x, y)` predicate |
| `ToyRobot/Direction.cs` | Enum with compass directions plus Unset sentinel |
| `ToyRobot/CommandParser.cs` | `ParseCommand` splits raw input; `TryParsePlaceArgs` parses PLACE argument string |
| `ToyRobot.Tests/SimulatorTests.cs` | Fact and Theory tests for all Simulator operations |
| `ToyRobot.Tests/CommandParserTests.cs` | Theory tests for command tokenisation and PLACE arg parsing |
| `ToyRobot.Tests/TableTests.cs` | Theory tests for boundary edge cases and custom dimensions |

## Namespace Organization

**`ToyRobot`** — all production types:
- `ToyRobot/Robot.cs`
- `ToyRobot/Direction.cs`
- `ToyRobot/Table.cs`
- `ToyRobot/Simulator.cs`
- `ToyRobot/CommandParser.cs`
- `ToyRobot/Program.cs` (top-level statements, implicit namespace)

**`ToyRobot.Tests`** — all test types:
- `ToyRobot.Tests/SimulatorTests.cs`
- `ToyRobot.Tests/CommandParserTests.cs`
- `ToyRobot.Tests/TableTests.cs`

## Test Structure Relative to Source

Tests live in a separate project (`ToyRobot.Tests/`) that references `ToyRobot.csproj` via a `<ProjectReference>`. There are no co-located test files inside the main project. Test classes mirror source classes by name (e.g., `SimulatorTests` tests `Simulator`, `TableTests` tests `Table`).

## Naming Conventions

**Files:** PascalCase, one class/enum per file matching the type name (e.g., `Robot.cs` contains `class Robot`).
**Test files:** `{ClassName}Tests.cs` pattern.
**Namespaces:** Match project name (`ToyRobot`, `ToyRobot.Tests`).

## Where to Add New Code

**New domain behaviour (e.g., new robot command):**
- Add method to `ToyRobot/Simulator.cs`
- Add dispatch case in `ToyRobot/Program.cs`
- Add tests in `ToyRobot.Tests/SimulatorTests.cs`

**New parsing logic:**
- Add static method to `ToyRobot/CommandParser.cs`
- Add tests in `ToyRobot.Tests/CommandParserTests.cs`

**New domain type or value object:**
- Create `ToyRobot/{TypeName}.cs` in the `ToyRobot` namespace

**New test class:**
- Create `ToyRobot.Tests/{ClassName}Tests.cs` in the `ToyRobot.Tests` namespace

## Special Notes

- `Commands.cs` does not exist — there is no command-object abstraction despite the filename appearing in planning documents.
- The solution file (`.sln`) is not present at the root; projects are referenced directly.
- No `src/` subdirectory — project folders sit directly at the repo root.

---

*Structure analysis: 2026-06-27*
