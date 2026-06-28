# Phase 2 — Application tests via TextReader/TextWriter injection

## Goal
Make `Application` testable by injecting `TextReader` (input) and `TextWriter` (output), then write a test suite covering all command dispatch paths.

## Constraints
- Parameterless `new Application().Run()` must still work identically in production.
- External behaviour of the app is unchanged.
- Loop must exit cleanly when the input stream is exhausted (allows tests to terminate).

---

## Task 1 — Add I/O injection to Application

**File:** `ToyRobot/Application.cs`

Changes:
- Add two private fields: `private readonly TextReader _input;` and `private readonly TextWriter _output;`
- Add a primary constructor: `public Application(TextReader input, TextWriter output)` that assigns both fields.
- Add a parameterless constructor that delegates: `public Application() : this(Console.In, Console.Out) { }`
- Replace every `Console.ReadLine()` call with `_input.ReadLine()`.
- Replace every `Console.WriteLine(...)` call with `_output.WriteLine(...)`.
- Change the loop condition: when `_input.ReadLine()` returns `null` (end of stream), break instead of processing. Use a `string? line = _input.ReadLine();` variable at the top of the loop body.

**Resulting loop skeleton:**
```csharp
while (true)
{
    string? line = _input.ReadLine();
    if (line is null) break;

    var parsed = CommandParser.ParseCommand(line);
    switch (parsed.Type) { ... }
}
```

**Verify:** `dotnet build` — no errors. `dotnet run --project ToyRobot` — app still works interactively.

---

## Task 2 — Write ApplicationTests

**File:** `ToyRobot.Tests/ApplicationTests.cs`

Helper method to use throughout:
```csharp
private static string Run(params string[] lines)
{
    var input  = new StringReader(string.Join("\n", lines));
    var output = new StringWriter();
    new Application(input, output).Run();
    return output.ToString();
}
```

Test cases to implement:

### Header is printed on startup
- Run with no commands (empty input).
- Assert output contains `"Toy Robot app"`.
- Assert output contains `"PLACE X,Y,Z"`.

### PLACE with valid args places the robot
- Run: `PLACE 1,2,NORTH` then `REPORT`.
- Assert output contains `"1, 2, North"`.

### PLACE with invalid args prints error
- Run: `PLACE bad_input`.
- Assert output contains `"Invalid PLACE arguments"`.

### MOVE before PLACE does nothing (no crash)
- Run: `MOVE` then `REPORT` (robot not placed — REPORT should produce no output).
- Assert output does NOT contain any position string.

### MOVE advances position
- Run: `PLACE 1,1,NORTH` then `MOVE` then `REPORT`.
- Assert output contains `"1, 2, North"`.

### MOVE does not go out of bounds
- Run: `PLACE 0,4,NORTH` then `MOVE` then `REPORT`.
- Assert output contains `"0, 4, North"` (position unchanged).

### LEFT rotates direction
- Run: `PLACE 2,2,NORTH` then `LEFT` then `REPORT`.
- Assert output contains `"2, 2, West"`.

### RIGHT rotates direction
- Run: `PLACE 2,2,NORTH` then `RIGHT` then `REPORT`.
- Assert output contains `"2, 2, East"`.

### REPORT before PLACE produces no output
- Run: `REPORT`.
- Assert output does NOT contain any coordinate pattern (no `-1` or position).

### Unknown command prints invalid selection message
- Run: `FOOBAR`.
- Assert output contains `"Invalid selection"`.

### Full sequence (integration)
- Run: `PLACE 0,0,NORTH` / `MOVE` / `RIGHT` / `MOVE` / `REPORT`.
- Assert output contains `"1, 1, East"`.

**Verify:** `dotnet run --project ToyRobot.Tests` — all tests pass (existing 40 + new Application tests).

---

## Acceptance criteria
- [ ] `Application(TextReader, TextWriter)` constructor exists
- [ ] Parameterless `Application()` still works (delegates to Console.In/Out)
- [ ] Loop exits cleanly on null input (end of stream)
- [ ] All command paths are covered by tests
- [ ] All 40 existing tests still pass
- [ ] New tests pass
