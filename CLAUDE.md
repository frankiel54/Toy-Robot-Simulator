# Claude guidance for ToyRobot

## Planning documents

The `.planning/` directory contains structured documents that should be consulted before making decisions about the codebase.

### Codebase map — `.planning/codebase/`
Read these before exploring or changing code:
- `ARCHITECTURE.md` — domain model, data flow, design patterns
- `STRUCTURE.md` — project layout and file responsibilities
- `STACK.md` — language, runtime, NuGet packages, build tooling
- `CONVENTIONS.md` — naming, code style, error handling patterns
- `TESTING.md` — test framework, how to run tests, coverage state
- `CONCERNS.md` — known tech debt, open questions, coverage gaps
- `INTEGRATIONS.md` — external dependencies (currently none)

### Phase plans — `.planning/phase-N/`
Each phase directory contains a `PLAN.md` with the goal, tasks, and acceptance criteria for that phase. Consult the relevant plan before implementing changes that fall within its scope.

## Running tests

This project uses xUnit v3 with a self-hosted runner (`OutputType=Exe`). Use:

```
dotnet run --project ToyRobot.Tests
```

`dotnet test` does not work for this project.
