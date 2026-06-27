# Technology Stack

**Analysis Date:** 2026-06-27

## Languages

**Primary:**
- C# 12.0 — all application and test code
  - Nullable reference types enabled (`<Nullable>enable</Nullable>`)
  - Implicit usings enabled (`<ImplicitUsings>enable</ImplicitUsings>`)

## Runtime

**Environment:**
- .NET 10.0 (`net10.0` TargetFramework in both projects)

**Package Manager:**
- NuGet (via `dotnet` CLI)
- Lockfile: not present

## Project Structure

**Solution:**
- `ToyRobot.slnx` — solution file
- `ToyRobot/ToyRobot.csproj` — main console application
- `ToyRobot.Tests/ToyRobot.Tests.csproj` — xUnit test project

## Frameworks

**Application:**
- None beyond the .NET base class library; pure console I/O via `System.Console`

**Testing:**
- xunit.v3 3.2.2 — test runner and assertion library
  - Uses `[Fact]` for single-case tests
  - Uses `[Theory]` + `[MemberData]` for parameterised tests
  - Uses `TheoryData<T...>` typed data sources

## Key NuGet Packages

| Package | Version | Project | Purpose |
|---------|---------|---------|---------|
| `xunit.v3` | 3.2.2 | ToyRobot.Tests | Test framework (runner + assertions) |

No third-party packages in the main `ToyRobot` project — zero external dependencies.

## Build System

- **dotnet CLI / MSBuild** — standard `dotnet build`, `dotnet run`, `dotnet test`
- Output type: `Exe` (both projects declare `<OutputType>Exe</OutputType>`)
- No custom build targets or props files detected

## Run Commands

```bash
dotnet run --project ToyRobot/ToyRobot.csproj      # Start the interactive console app
dotnet test ToyRobot.Tests/ToyRobot.Tests.csproj   # Run all tests
dotnet build                                         # Build solution
```

## Notable Design Choices

- No dependency injection framework — objects are instantiated directly in `Program.cs`
- No logging library — uses `Console.WriteLine` throughout
- No configuration library — no `appsettings.json`, no `IConfiguration`
- Enum `Direction` uses integer arithmetic for rotation logic (`GameBoard.cs`)
- `CommandParser` is a `static` class with no instance state

---

*Stack analysis: 2026-06-27*
