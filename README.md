# Toy Robot Simulator

## Introduction

This is the Toy Robot Simulator console application.

To run it, either load the project in an IDE and run it in debug mode, or run the `.exe` in the `/publish` directory. 

> **Note:** The `publish` directory is included in this repo for the sake of the interview. In a normal scenario it would be gitignored, with the exe produced from CI/CD.

## Development Process

I chose to build an interactive console app that takes user input, rather than an app that ingests a file. This app could be extended to support file input by modifying `Application` to accept alternate input sources, but I left that out of scope for this challenge.

The app is built on .NET 10 with xUnit v3 for testing.

### Approach

My approach was a hybrid of hand-written code and AI-assisted code generation. I started by building a skeleton mostly by hand with minimal AI involvement — I wanted a solid grasp of the problem space first. Building a working MVP manually helped me get my head into the problem before iterating with AI to refactor, fix bugs, and improve the solution.

During the skeleton phase, I used Claude Code with Sonnet 4.6 for simple questions and small refactors, but roughly 90% of the initial code was written by hand. Once I had a working MVP with a basic test suite, I introduced the GSD (Get Shit Done) framework for Claude to map out the solution using `/gsd-map-codebase`. This generated structured `.md` files under `/.planning/codebase/` to inform later decisions and provide context — a standard part of the GSD workflow.

After mapping the codebase, I used `/gsd-plan-phase` to plan further changes. This takes a prompt and produces an implementation plan (found in `/.planning/phases/` directories), which I could review and approve before handing off to a Claude subagent via `/gsd-execute-phase` to carry out the implementation. This approach keeps context lean, leaves a documentation trail, and keeps the AI working from a well-defined scope.

From that point, roughly 90% of changes were handled by Claude via the Plan → Execute workflow, covering:

- Command parser rebuild to create ParsedCommand instead of strings
- Refactoring `Application` to be independent from `Program`
- Refactoring `Application` to accommodate unit tests
- Tests for the application layer
- Adding user feedback to console messages
- Refactoring `Application` for readability
- Refactoring direction calculation and the `Direction` enum
- Refactoring property names to be consistent in format
- Refactoring `Simulator` to own the `Robot`
- Refactoring `Robot` field encapsulation
- Code review to identify code smells and bugs
- Refactoring `Robot` from a class to a record

### Example Prompts

Some examples of prompts I gave Claude during the planning phase:

> I have created a `ParsedCommand` class. I want `CommandParser` to only return `ParsedCommand` instances, and I want `Program.cs` to use `ParsedCommand` rather than separate variables. If you feel `ParsedCommand` needs to be edited, check with me first.

> I want all commands to give user feedback when executed. Add feedback output to Move, Place, Left, and Right — for Move, give conditional feedback based on the boolean returned.

> The `Application` class is getting hard to read. Can we refactor the Place case outside the switch statement so we don't need to check `isRobotPlaced` in each case? If you have better ideas, you are free to suggest them.

> I want to refactor the `Direction` enum so that enum order doesn't break the implementation. I also want to extract the TurnLeft and TurnRight calculations into `Direction`, and move the forward movement calculation out of `Simulator` and into `Direction`.

Some changes were also suggested by Claude during the planning phase, and I would decide whether to accept them.

### Review Process

No change Claude made was committed without my review. I instructed Claude to stage changes and pause before committing, so I could inspect every diff. I would sometimes request modifications before approving. Once satisfied, I would either commit manually or let Claude commit and continue. This workflow ensured I was aware of every line of code going into the repository, with Claude acting as an assistant rather than a sole decision maker.
