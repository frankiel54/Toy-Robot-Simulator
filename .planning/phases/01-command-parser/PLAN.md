# Phase 1 — Refactor CommandParser to return ParsedCommand

## Goal
Replace the `out`-parameter API on `CommandParser` with a method that returns a `ParsedCommand` object. Update `Program.cs` to consume `ParsedCommand` instead of raw strings and loose variables.

## Constraints
- Do NOT change `ParsedCommand.cs` beyond making members public (user-approved change).
- Do NOT change the external behaviour of the program — commands must work identically after the refactor.
- Keep `CommandParserTests.cs` green; update test signatures where needed.

---

## Task 1 — Make ParsedCommand members public

**File:** `ToyRobot/ParsedCommand.cs`

Changes:
- `ParsedCommand.Type` → `public CommandType Type { get; }` (read-only property)
- `ParsedCommand.Options` → `public CommandOptions? Options { get; }` (read-only property)
- `CommandOptions.x` → `public int X { get; set; }` (PascalCase, public)
- `CommandOptions.y` → `public int Y { get; set; }`
- `CommandOptions.facing` → `public Direction Facing { get; set; }`
- Update `ParsedCommand` constructor to assign via property initializer or backing field pattern.

**Verify:** Project compiles with no errors after this step.

---

## Task 2 — Refactor CommandParser.ParseCommand to return ParsedCommand

**File:** `ToyRobot/CommandParser.cs`

Changes:
- Replace `public static void ParseCommand(string input, out string command, out string args)` with `public static ParsedCommand ParseCommand(string input)`.
- Internal logic:
  1. Split `input` on `' '` (max 2 parts) to get command name and raw args.
  2. Map command name (uppercase) to `CommandType`:
     - `"PLACE"` → `CommandType.Place`
     - `"MOVE"` → `CommandType.Move`
     - `"LEFT"` → `CommandType.Left`
     - `"RIGHT"` → `CommandType.Right`
     - `"REPORT"` → `CommandType.Report`
     - anything else → `CommandType.Unknown`
  3. For `CommandType.Place`: call the existing `TryParsePlaceArgs` helper; if it succeeds, attach a `CommandOptions`; if it fails, still return `CommandType.Place` with `Options = null` (caller can treat null Options as invalid PLACE).
  4. For all other types: return `new ParsedCommand(commandType)` with no options.
- Keep `TryParsePlaceArgs` as a `private static` helper (no longer needs to be public — but leave public if `CommandParserTests` tests it directly, see Task 3).

**Verify:** Project compiles. `CommandParser.ParseCommand` returns a `ParsedCommand` for all inputs.

---

## Task 3 — Update CommandParserTests

**File:** `ToyRobot.Tests/CommandParserTests.cs`

Changes:
- `ParseCommand_Should_Parse` — rewrite to call `CommandParser.ParseCommand(rawCommand)` and assert on `result.Type` (CommandType enum value) instead of separate string outputs.
- `TryParsePlaceArgs_Should_Parse` — keep if `TryParsePlaceArgs` remains public; otherwise replace with a PLACE-path test through `ParseCommand` that asserts `result.Options.X`, `result.Options.Y`, `result.Options.Facing`.
- Update `ParseCommandData` theory rows: replace `(string raw, string parsedCommand, string parsedArgs)` with `(string raw, CommandType expectedType)` (or richer tuple if also asserting Options).

**Verify:** `dotnet run --project ToyRobot.Tests` — all tests pass.

---

## Task 4 — Refactor Program.cs to use ParsedCommand

**File:** `ToyRobot/Program.cs`

Changes:
- Replace:
  ```csharp
  CommandParser.ParseCommand(Console.ReadLine() ?? string.Empty, out string command, out string commandArgs);
  ```
  With:
  ```csharp
  var parsed = CommandParser.ParseCommand(Console.ReadLine() ?? string.Empty);
  ```
- Replace `switch (command)` with `switch (parsed.Type)`:
  - `case "PLACE":` → `case CommandType.Place:`
  - `case "MOVE":` → `case CommandType.Move:`
  - `case "LEFT":` → `case CommandType.Left:`
  - `case "RIGHT":` → `case CommandType.Right:`
  - `case "REPORT":` → `case CommandType.Report:`
  - `default:` stays
- In the `CommandType.Place` case, replace:
  ```csharp
  if (CommandParser.TryParsePlaceArgs(commandArgs, out int x, out int y, out Direction direction))
  ```
  With a null-check on `parsed.Options`:
  ```csharp
  if (parsed.Options is { } opts)
  {
      Simulator.Place(opts.X, opts.Y, opts.Facing);
  }
  ```
  (If `Options` is null, PLACE args were invalid — print the existing error message.)
- Remove the `commandArgs` variable entirely.

**Verify:** Run the app manually (`dotnet run --project ToyRobot`) and test each command: PLACE 1,1,NORTH / MOVE / LEFT / RIGHT / REPORT / an invalid command.

---

## Acceptance criteria
- [ ] `ParsedCommand.Type` and `ParsedCommand.Options` are publicly readable
- [ ] `CommandParser.ParseCommand` returns `ParsedCommand` (no `out` parameters)
- [ ] `Program.cs` switches on `CommandType` enum, not strings
- [ ] All existing tests pass
- [ ] App behaves identically to before the refactor
