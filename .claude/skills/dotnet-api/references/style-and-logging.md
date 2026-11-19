# Logging (G1) and style (T1)

- **G1** Serilog: create a bootstrap logger first; wrap the whole of `Program.cs` in `try / catch(HostAbortedException) / catch / finally Log.CloseAndFlush()` (the `HostAbortedException` catch stops EF tooling from logging a false fatal error); configure from appsettings (`ReadFrom.Configuration`); enable `UseSerilogRequestLogging()`. Log levels: Information for steps, Warning for client mistakes, Error for failures.
- **T1** Style:
  - File-scoped namespaces (`namespace X;`); `using`s not grouped by system vs project.
  - Auto-properties `{get; set;}` with no inner spaces; fields `_camelCase`.
  - Explicit types (`Product? product = ...`) preferred over `var`.
  - Allman braces; write `if(` with no space.
  - Target-typed `new()` and collection expressions (`[]`, `[".jpg", ...]`).
  - Enum type names ALL CAPS (`CATEGORY`, `GENDER`, `STATUS`), members mixed case; serialized as strings in the API via `JsonStringEnumConverter`.
  - Frequent comments that explain *why* (`// XSS prevention`, `// store token hash instead of plain Refresh Tokens`).
