---
name: dotnet-api
description: Layered ASP.NET Core Web API conventions (Controller → Service → Repository, EF Core Fluent configs, hand-mapped DTOs, Options pattern, AppException hierarchy, Identity + JWT in cookies, Serilog) with slash commands. Use /dotnet-api check to review code against the conventions, /dotnet-api fix to apply fixes, /dotnet-api audit for a deep scan, and /dotnet-api new|entity|endpoint|dto|options|exception to scaffold. Also use when adding or modifying controllers, services, repositories, entities, EF configs, DTOs, options, exceptions, auth or logging in a .NET API.
argument-hint: "[check|fix|audit|new|entity|endpoint|dto|options|exception|explain] [args]"
---

# dotnet-api: conventions + commands

User request: `$ARGUMENTS`

## How to run

1. **Detect the project first.** Read the `.csproj` (TargetFramework, packages), `Program.cs`, and one existing feature end to end. Learn the root namespace, folder names, DI registrations and connection-string key. Use the project's real names; examples in the references use `Product` as a stand-in.
2. **Existing code wins** when a project clearly follows a different pattern (MediatR, AutoMapper, FluentValidation, etc.). Mention the deviation once; don't rewrite the project to fit these rules unless asked.
3. **Parse the first word of the request as a command** (below). Empty request: apply the conventions to whatever the user asked for, and list the commands in one short block.
4. **Read only the references you need** (map below), not all of them.
5. After writing code, run `dotnet build` and fix compile errors before reporting. Never run migrations against a database; creating a migration file is fine.

## Always-on layer rules

- **L1** Layout: `Controllers/`, `Services/` + `Services/Interfaces/`, `Repositories/` + `Repositories/Interfaces/`, `Data/` + `Data/Configs/`, `Models/Entities/`, `Models/DTOs/Request|Response/`, `Exceptions/`, `Options/`, `Common/`.
- **L2** Controllers are thin: one service call, then `Ok`/`Created*`. Services own business rules, validation, logging and mapping. Repositories own EF queries only and return entities/null.
- **L3** Every service and repository has an `IX` interface in `Interfaces/`, registered `AddScoped<IX, X>()` in `Program.cs` (repositories first, then services, each group under a comment).
- **L4** Constructor injection into `private readonly _camelCase` fields (primary constructor only for `AppDbContext`). Logger typed on the interface: `ILogger<IProductService>`. Controllers inject service interfaces only.

## Reference map

| Working on                                                   | Read                                   |
| ------------------------------------------------------------ | -------------------------------------- |
| Controllers (C1–C8)                                          | `references/controllers.md`            |
| Services (S1–S9)                                             | `references/services.md`               |
| Repositories, DbContext, entities, EF configs (R1–R5, E1–E8) | `references/repositories-and-ef.md`    |
| DTOs, validation, file validation (D1–D6, V1)                | `references/dtos-and-validation.md`    |
| Exceptions, options, appsettings (X1–X3, O1–O4)              | `references/exceptions-and-options.md` |
| Identity, JWT, cookies, refresh tokens, roles (A1–A7)        | `references/auth-and-security.md`      |
| Serilog, code style (G1, T1)                                 | `references/style-and-logging.md`      |
| Known bugs/pitfalls (P1–P10)                                 | `references/pitfalls.md`               |
| Scaffolding commands                                         | `references/scaffolding.md`            |

## Commands

| Command                           | What to do                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `check [path\|staged\|all]`       | **Review only; do not edit.** Scope: files changed in `git diff` (working tree + staged) by default; if none, ask or use `all`. For each file, classify its layer, read the matching reference(s) plus `pitfalls.md`, and check every rule by ID. Output grouped by severity (**Error** = breaks a layer or security rule; **Warning** = convention drift; **Nit** = style), each as `Rule-ID  path:line  problem  → suggested fix`. End with counts per severity and the 3 most important fixes. If clean, say so in one line. |
| `fix [path\|staged]`              | Run `check`, then apply Error and Warning fixes that are safe and local. List anything skipped and why (needs a migration, changes API behavior, etc.). Run `dotnet build`.                                                                                                                                                                                                                                                                                                                                                     |
| `audit`                           | Whole-codebase scan for the pitfalls in `references/pitfalls.md`. Report with file:line evidence. Don't edit.                                                                                                                                                                                                                                                                                                                                                                                                                   |
| `new <Resource> [field:type ...]` | Scaffold a full vertical slice (see `scaffolding.md`).                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
| `entity <Name> [field:type ...]`  | Entity + EF config + DbSet + migration file only.                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| `endpoint <Resource> <action>`    | One endpoint through every layer, matching sibling endpoints.                                                                                                                                                                                                                                                                                                                                                                                                                                                                   |
| `dto <Name>`                      | Request/response DTO pair.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| `options <Name>`                  | Options class + registration + placeholder appsettings section.                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| `exception <Name> <status>`       | New sealed `AppException` subclass.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| `explain <topic>`                 | Summarize the relevant convention with a short example from the codebase.                                                                                                                                                                                                                                                                                                                                                                                                                                                       |

Details for the scaffolding commands are in `references/scaffolding.md`.
