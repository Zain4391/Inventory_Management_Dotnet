# Scaffolding commands

Before scaffolding: detect the project (namespaces, folder names, an existing sibling feature to copy) and state assumptions (fields, roles, listable or not) in one short block. Follow the rule IDs in the other references. Finish with `dotnet build` and a short list of files created. Create migration files only; never update a database.

## `new <Resource> [field:type ...]`: full vertical slice
1. **Entity** in `Models/Entities/` (enums in the same file, ALL-CAPS type names) → **`XConfig`** in `Data/Configs/` → `DbSet<T>` in `AppDbContext` → migration file. (E1–E8)
2. **DTOs**: `XxxRequestDto` (+ update request with nullable fields if needed), `XxxResponseDto`, and `XxxQueryRequest` if listable. (D1–D6, R5)
3. **Repository**: `IXRepository` + `XRepository` (entities only, `AsNoTracking` reads, `SaveChangesAsync` inside writes, paged list query). (R1–R5)
4. **Service**: `IXService` + `XService` using the 5-step shape; manual mapping; throws `AppException`s; `ConflictException` for duplicate unique values; pagination bounds checks. (S1–S9)
5. **Controller**: explicit lowercase plural route, typed bindings, roles (Admin for writes, anonymous or Staff for reads as the project does), `CreatedAtAction` for POST. (C1–C8)
6. **DI**: register repository then service in the correct groups in `Program.cs`. (L3)
7. `dotnet build`; summarize files and assumptions.

## `entity <Name> [field:type ...]`
Steps 1 only (entity, config, DbSet, migration file). Ask nothing; state assumptions about keys, max lengths, delete behavior.

## `endpoint <Resource> <action>`
Open the sibling endpoints first and match them. Add, in order: DTOs (if new) → repository method + interface entry → service method + interface entry (5-step shape) → controller action (attribute order, return type, correct `Ok`/`Created*`, roles). Don't touch unrelated code.

## `dto <Name>`
Create `NameRequestDto` and `NameResponseDto` in the correct folders with annotations on the request only, `required` on non-nullable references, nullable optionals, `= []` collections, and error messages that match their rules. Check P4–P7 as you write.

## `options <Name>`
Create `NameOptions` in `Options/` with `SectionName`, `[Required(AllowEmptyStrings = false)]` strings; register with `AddOptions<NameOptions>().BindConfiguration(...).ValidateDataAnnotations().ValidateOnStart()`; add a placeholder section to `appsettings.json` (never real secrets, mention user-secrets for sensitive values).

## `exception <Name> <status>`
Create a `sealed` subclass of `AppException` in `Exceptions/` with a fixed `HttpStatusCode`. `GlobalExceptionHandler` needs no change (it maps by `statusCode`); say so.
